using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

//십자선 표시 및 조절 담당 스크립트
//"Crosshair" 게임 오브젝트에 붙여서 사용

public class CrosshairCtrl : MonoBehaviour
{
    [HideInInspector] public ItemInfo m_itemInfo = null;         //현재 적용된 무기의 정보 받아오기
    public GameObject m_border = null;                           //총기를 들었을 때와 아닐때의 십자선에 차이를 주기위해 온오프 되는 게임오브젝트
    public Animator m_defMaskAnimator = null;                    //"DefaultCross"의 마스크 애니메이터 (일반소총 줌인아웃 시 애니메이션 용)
    [HideInInspector] public bool m_zoomInOut = false;           //줌인아웃 상태 체크용 변수 (true 일 때 온)

    //---- 일반소총과 저격총의 십자선 구분
    [Header("----- CrossHairs -----")]    
    public GameObject m_defaultCross = null;                    //일반소총(줌인아웃) & 저격총 줌아웃일 때 십자선
    public GameObject m_sniperCross = null;                     //저격총 줌인일 때 십자선
    //---- 일반소총과 저격총의 십자선 구분

    //------"DefaultCross"의 하위 이미지
    [Header("----- DefaultCross Images -----")]
    public RawImage m_curDot;        //"DefaultCross"의 dot 이미지
    public RawImage m_curInner;      //"DefaultCross"의 inner 이미지
    public RawImage m_curExpanding;  //"DefaultCross"의 expanding 이미지
    public Image m_curReload;        //"DefaultCross"의 reload 이미지
    //------"DefaultCross"의 하위 이미지

    //-----CrossHair 설정 관련
    float m_reloadSpeed;                                      //재장전 속도
    float m_shrinkSpeed;                                      //expanding의 scale 감소 속도
    Vector3 m_crosshairOriginalScale;                         //"Crosshair"의 "expanding"의 기본 scale ("expanding" 초기화 할 때 사용)
    bool m_isShrinking;                                       //"Crosshair"의 "expanding"이 감소 중인지 체크 (이미 코루틴을 통해서 감소중일 때 중복 방지)
    [HideInInspector] public Vector3 m_crosshairMaxScale;     //"Crosshair"의 "expanding"의 maxScale  (기본 십자선이 최대로 늘어날 수 있는 크기)   
    [HideInInspector] public float m_expandSize;              //발당 늘어나는 "expanding"의 scale 사이즈 (매 사격 당 늘어나는 십자선 크기)
    [HideInInspector] public bool m_isReloading;              //재장전 중인지 여부(재장전 중일 때 발사 제한 용)
    //-----CrossHair 설정 관련

    void Start()
    {
        m_curReload.enabled = false;     //"Crosshair"의 재장전 이미지 끄기
        m_crosshairOriginalScale = m_curExpanding.rectTransform.localScale; //"Crosshair"의 "expanding"의 기본 scale 저장
    }

    private void Update()
    {
        if (PlayerCtrl.inst.m_isRun == false || PlayerCtrl.inst.m_animController.GetBool("Roll") == true) //일부 상태에서 동작 제한
            return;

        if (Input.GetKeyDown(KeyCode.R))     //재장전 버튼 클릭
            if (0 < m_itemInfo.m_maxMagazine
                && m_itemInfo.m_curMagazine < m_itemInfo.m_reMagazine)       //남은 총알이 있고, 풀차징이 아닐 때만 실행
                DoReload();
    }

    public void DoReload()    //재장전
    {        
        StartCoroutine(CrossHairChange(false));   //십자선 변경 코루틴 실행
        if (!m_isReloading)                       
            StartCoroutine(ReloadTheGun());       //재장전 실행
    }

    public void ExpandCrosshair()     //"Crosshair"의 "expanding"의 scale 확장시키는 함수 (총 발사할때마다 실행)
    {
        if (m_curExpanding.rectTransform.localScale.x < m_crosshairMaxScale.x)      //현재 "expanding"의 크기가 최대가 아닐경우에만 실행
            m_curExpanding.rectTransform.localScale += new Vector3(m_expandSize, m_expandSize, m_expandSize);   //정해진 크기만큼 증가
        else
            m_curExpanding.rectTransform.localScale = m_crosshairMaxScale;          //"expanding"이 최대 크기까지만 늘어나게 하기

        StartCoroutine(ShrinkCrosshair());          //"expanding"의 크기 감소 코루틴 실행
    }

    public void SetReloadSpeed(float ReloadSpeed)       //재장전 속도 설정 함수
    {
        this.m_reloadSpeed = ReloadSpeed;
    }

    public void SetShrinkSpeed(float ShrinkSpeed)       //"expanding"의 감소 속도 설정 함수
    {
        this.m_shrinkSpeed = ShrinkSpeed;
    }

    public void SetMaxScale(float MaxScale)             //"expanding"의 maxscale 설정 함수
    {
        this.m_crosshairMaxScale = new Vector3(MaxScale, MaxScale, MaxScale);
    }

    public IEnumerator ShrinkCrosshair()    //"expanding"의 감소 동작 실행함수
    {
        if (m_isShrinking == true)          //이미 감소중 상태면 또 실행 안함
            yield break;

        m_isShrinking = true;               //감소중인 상태로 변경
                
        do
        {
            m_curExpanding.rectTransform.localScale = new Vector3(m_curExpanding.rectTransform.localScale.x - Time.deltaTime * m_shrinkSpeed,
                                                             m_curExpanding.rectTransform.localScale.y - Time.deltaTime * m_shrinkSpeed,
                                                             m_curExpanding.rectTransform.localScale.z - Time.deltaTime * m_shrinkSpeed);
            yield return Time.deltaTime;      
        }
        while (m_crosshairOriginalScale.x < m_curExpanding.rectTransform.localScale.x);    //"expanding"의 원래 scale로 돌아갈 때 까지 반복

        m_isShrinking = false;              //감소 중이 아닌 상태로 변경
        yield return new WaitForEndOfFrame();
    }

    IEnumerator ReloadTheGun()   //재장전 실행 함수
    {
        m_isReloading = true;    //재장전 중으로 변경

        PlayerCtrl.inst.m_animController.SetTrigger("Reload");  //플레이어의 재장전 애니메이션 재생

        SoundMgr.inst.SoundPlay(SoundList.Reload);    //재장전 사운드 재생

        //------ 이미지 온오프
        m_curReload.enabled = true;            //"reload" 이미지 켜기
        m_curReload.fillAmount = 0;            //"reload" 진행 정도 체크
        m_curInner.enabled = false;            //"inner" 이미지 끄기
        m_curDot.enabled = false;              //"dot" 이미지 끄기
        m_curExpanding.enabled = false;        //"expanding" 이미지 끄기
        //------ 이미지 온오프

        do
        {
            m_curReload.fillAmount += (Time.deltaTime * m_reloadSpeed);
            yield return Time.deltaTime;
        }
        while (m_curReload.fillAmount < 1f);       //재장전 완료까지 반복


        //----- 총알 계산
        int a_Calcint = m_itemInfo.m_reMagazine - m_itemInfo.m_curMagazine;    //재장전 시 장전개수 - 현재 남아있는 총알 개수

        if (a_Calcint <= m_itemInfo.m_maxMagazine)          //재장전 해야할 총알 수보다 남아있는 총알 수가 크거나 같으면
        {
            m_itemInfo.m_maxMagazine -= a_Calcint;          //남은 총알을 재장전한 개수만큼 줄이기
            m_itemInfo.m_curMagazine += a_Calcint;          //재장전한 총알 개수만큼 현재 총알 충전
        }
        else  //재장전 해야할 총알 수보다 남아있는 총알 수가 적다면
        {
            m_itemInfo.m_curMagazine = m_itemInfo.m_maxMagazine;    //남아있는 총알 수 만큼만 충전
            m_itemInfo.m_maxMagazine = 0;                           //남아있는 총알 개수 0        
        }
        //----- 총알 계산

        //------ 이미지 온오프
        m_curReload.enabled = false;           //"reload" 이미지 끄기
        m_curInner.enabled = true;             //"inner" 이미지 켜기
        m_curDot.enabled = true;               //"dot" 이미지 켜기
        m_curExpanding.enabled = true;         //"expanding" 이미지 켜기
        //------ 이미지 온오프

        m_isReloading = false;                 //재장전 중이 아님으로 변경

        m_curExpanding.rectTransform.localScale = m_crosshairOriginalScale; //"CrossHair"의 "expanding"의 scale을 기본크기로 변경

        yield return new WaitForEndOfFrame();
    }

    private void OnDisable()    //시네머신 카메라 동작 시 크로스헤어 초기화
    {
        m_isShrinking = false;
        m_curExpanding.rectTransform.localScale = m_crosshairOriginalScale;
    }

    public IEnumerator CrossHairChange(bool a_zoom)     //십자선 변경 동작 함수
    {
        m_zoomInOut = a_zoom;     //요청 된 상태 받아오기

        yield return new WaitForEndOfFrame();

        if (m_zoomInOut == true)   //줌인 상태
            if (m_itemInfo.m_itName == ItemName.SniperRifle)       //저격총일 경우
            {
                m_defaultCross.gameObject.SetActive(false);        //"DefaultCross" 끄기
                m_sniperCross.gameObject.SetActive(true);          //"SniperCross" 켜기
                Camera.main.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView = 15.0f; //카메라 시야 앞으로
                Camera.main.GetComponent<CameraCtrl>().m_maxX = 15.0f;  //위아래 최대 움직임 15
                PlayerCtrl.inst.m_nowWeapon.m_aimRange = 50.0f;         //정확한 조준이 가능한 거리 50 (해당 거리 안에 타겟이 있을경우 반드시 명중)
            }
            else    //일반 총일 경우
            {
                m_defMaskAnimator.SetTrigger("ZoomIn");             //십자선 줌인 애니메이션 재생
                SetMaxScale(1.3f);                                  //"CrossHair" "expanding"의 최대 scale 설정
                PlayerCtrl.inst.m_nowWeapon.m_aimRange = 25.0f;     //정확한 조준이 가능한 거리 25 (해당 거리 안에 타겟이 있을경우 반드시 명중)
            }

        else  //(m_zoomInOut == false)   //줌아웃 상태 (줌인상태에서 재장전 할 경우)
        {
            if(0 < m_defMaskAnimator.gameObject.GetComponent<RawImage>().color.a)       //줌인 마스크가 보이는 상태라면
                m_defMaskAnimator.SetTrigger("ZoomOut");                                //십자선 줌아웃 애니메이션 재생

            if (m_itemInfo.m_itName == ItemName.SniperRifle)      //저격총일 경우
            {
                m_defaultCross.gameObject.SetActive(true);                      //"DefaultCross" 켜기
                m_sniperCross.gameObject.SetActive(false);                      //"SniperCross" 끄기
                Camera.main.GetComponent<CinemachineVirtualCamera>().m_Lens.FieldOfView = 60.0f; //카메라 시야 뒤로
                Camera.main.GetComponent<CameraCtrl>().m_maxX = 60.0f;          //위아래 최대 움직임 60
            }

            SetMaxScale(m_itemInfo.m_maxScale);                 //"CrossHair"의 "expanding"의 최대 scale 설정
            PlayerCtrl.inst.m_nowWeapon.m_aimRange = 15.0f;     //정확한 조준이 가능한 거리 15 (해당 거리 안에 타겟이 있을경우 반드시 명중)
        }
    }

    public void RollCrossCtrl()     //플레이어가 구르기 할 때의 십자선 변경 함수
    {
        if (PlayerCtrl.inst.m_animController.GetBool("Roll") == true
            && m_border.activeSelf == true)                 //플레이어가 구르기를 하면
        {
            m_curExpanding.rectTransform.localScale = m_crosshairMaxScale;      //"expanding"의 scale을 maxScale 값으로
            StartCoroutine(ShrinkCrosshair());                                  //"expanding의" scale 감소 코루틴 시작
            StopCoroutine(ShrinkCrosshair());                                   //"expanding의" scale 감소 코루틴 정지
        }
    }
}
