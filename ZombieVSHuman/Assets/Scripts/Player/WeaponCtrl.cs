using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//현재 착용한 무기의 동작들을 담당하는 스크립트
//무기 프리팹들에 붙여서 사용

public class WeaponCtrl : MonoBehaviour
{
    [HideInInspector] public ItemInfo m_itemInfo = null;             //현재 무기의 정보를 저장할 변수
    [HideInInspector] public CrosshairCtrl m_crossCtrl = null;       //"UICanvas"의 "CrossHair"에 붙은 CrossHairCtrl스크립트를 담을 변수

    [Header("----- Guns -----")]
    //----- 무기가 총일 때 필요한 변수
    public GameObject m_bulletObj = null;       //총알 리소스 담을 변수
    public MeshRenderer m_muzzleFlash = null;   //총구 불빛 이펙트의 Meshrenderer
    public Transform m_firePos = null;          //총구 transform 담을 변수
    //----- 무기가 총일 때 필요한 변수

    //----- 무기가 배트나 발차기일 때 필요한 변수
    [HideInInspector] public bool m_closeAtk = false;    //공격이 들어갔는지 판별하는 변수
    //----- 무기가 배트나 발차기일 때 필요한 변수

    [HideInInspector] public bool m_misFire = false;     //공격 불가능 상태

    //---- 총의 타격점을 잡기위한 변수들
    RaycastHit m_hitInfo;                                //광선에 맞은 대상
    Vector3 m_targetPos = Vector3.zero;                  //타격점을 담는 변수
    [HideInInspector] public float m_aimRange = 15.0f;   //정확한 조준이 가능한 거리
    //---- 총의 타격점을 잡기위한 변수들

    private void Awake()
    {
        m_crossCtrl = CanvasCtrl.inst.GetComponentInChildren<CrosshairCtrl>();
    }
    // Start is called before the first frame update
    //void Start()
    //{
    //}

    // Update is called once per frame
    void Update()
    {
        if (InGameMgr.s_gameState != GameState.GameIng) //게임 진행중 상태가 아니면 중지
            return;

        if (PlayerCtrl.inst.m_isRun == false)           //플레이어가 움직일 수 없는 상태면 중지
            return;

        if (m_itemInfo.m_itName == ItemName.Bat || m_itemInfo.m_itName == ItemName.Kick)  //무기가 야구배트와 발차기일 경우
        {
            if (m_crossCtrl.m_border.activeSelf == true)
                m_crossCtrl.m_border.SetActive(false);          //십자선 변경
        }
        else   //무기가 총일 경우
        {
            if (m_crossCtrl.m_border.activeSelf == false)
                m_crossCtrl.m_border.SetActive(true);           //십자선 변경

            if (m_itemInfo.m_curMagazine == 0 && 0 < m_itemInfo.m_maxMagazine
                && m_crossCtrl.m_isReloading == false &&
                PlayerCtrl.inst.m_animController.GetCurrentAnimatorStateInfo(1).IsName("Fire") == false
                && PlayerCtrl.inst.m_animController.IsInTransition(1) == false)      //총알을 다썼다면 자동으로 재장전(쏘는 애니메이션이 끝나면)
                m_crossCtrl.DoReload();
            
            if (Input.GetMouseButtonDown(1))        //마우스 우측버튼 클릭 시 줌인아웃
            {
                StartCoroutine(m_crossCtrl.CrossHairChange(!m_crossCtrl.m_zoomInOut));
            }
        }
    }

    //---- 초기 설정
    public void Init()      
    {
        PlayerCtrl.inst.m_nowWeapon = this;             //현재 이 무기가 플레이어가 착용한 무기임

        if (m_itemInfo.m_itName == ItemName.Bat || m_itemInfo.m_itName == ItemName.Kick)        //아이템이 야구배트나 기본무기일경우
            PlayerCtrl.inst.m_getGun = false;           //총을 안들었을 때의 애니메이션을 재생하기 위함
        else
            PlayerCtrl.inst.m_getGun = true;            //총을 들었을 때의 애니메이션을 재생하기 위함

        if (m_muzzleFlash != null)                      //야구방망이, 발차기일 경우 muzzleFlash 없음
            m_muzzleFlash.enabled = false;              //처음에 총구 불빛 이펙트 꺼주기

        m_crossCtrl.m_itemInfo = m_itemInfo;                   //현재무기의 정보 "CrossHair"에 연동
        m_crossCtrl.SetShrinkSpeed(m_itemInfo.m_shrinkSpeed);  //"CrossHair"의 "expanding"의 scale 감소 속도 설정
        m_crossCtrl.SetReloadSpeed(0.2f);                      //재장전 속도 설정        
        m_crossCtrl.m_expandSize = m_itemInfo.m_expandScale;   //발당 증가하는 "expanding"의 scale 사이즈

        StartCoroutine(m_crossCtrl.CrossHairChange(false));
    }

    public void Fire()         //총기로 공격할 때 
    {
        if (0 < m_itemInfo.m_curMagazine)              //총알이 있다면
            m_itemInfo.m_curMagazine -= 1;             //한발 감소
        else          
            return;

        PlayerCtrl.inst.m_animController.SetTrigger("Fire");           //총쏘는 애니메이션 재생
        SoundMgr.inst.SoundPlay(SoundList.Weapon, m_itemInfo);         //무기에 맞는 사운드 재생
                
        //카메라의 정면으로부터 조준거리 안에 광선에 맞는 대상이 있을경우 그 대상을 타격점으로 잡음
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out m_hitInfo, m_aimRange, 128))      //좀비 Layer만
            m_targetPos = m_hitInfo.point;

        //없을 경우 조준 가능한 위치까지를 타격점으로 잡음
        else
            m_targetPos = Camera.main.transform.position + (Camera.main.transform.forward * m_aimRange);

        //----- 총알 발사 각도 계산
        float a_RndRange = (m_crossCtrl.m_curExpanding.transform.localScale.x - 1) * 6; //현재 "CrossHair"의 늘어난 scale값 * 6
        float a_RndX = (int)Random.Range(-a_RndRange, a_RndRange);  //총알이 빗나갈 각도를 저장할 변수(늘어난 scale값 * 6의 범위)
        float a_RndY = (int)Random.Range(-a_RndRange, a_RndRange);  //총알이 빗나갈 각도를 저장할 변수(늘어난 scale값 * 6의 범위)  

        GameObject a_bullet = Instantiate(m_bulletObj, m_firePos.position, Quaternion.identity);   //총구위치에 총알 생성
        a_bullet.GetComponent<BulletCtrl>().m_bulletDmg = m_itemInfo.m_damage;      //총알의 대미지 설정
        a_bullet.transform.LookAt(m_targetPos);     //총알이 타겟을 바라보도록 설정        
        Vector3 a_rot = a_bullet.transform.rotation.eulerAngles;        //현재 총알의 회전값 가져오기
        a_rot.x += a_RndX;                                              //총알의 회전값에 빗나갈 각도 더하기
        a_rot.y += a_RndY;                                              //총알의 회전값에 빗나갈 각도 더하기
        a_rot.z = 0.0f;
        a_bullet.transform.rotation = Quaternion.Euler(a_rot);          //총알 각도 변경
        //----- 총알 발사 각도 계산

        if (m_crossCtrl.m_zoomInOut == true && m_itemInfo.m_itName == ItemName.SniperRifle)
            return;

        m_crossCtrl.ExpandCrosshair();                       //"CrossHair"의 "expanding" 확장

        if (m_muzzleFlash != null)
            StartCoroutine(ShowMuzzleFlash());               //총구 불빛 이펙트 출력        
    }

    //--- 총구 불빛 이펙트
    IEnumerator ShowMuzzleFlash()
    {
        //MuzzleFlash를 Z축을 기준으로 불규칙하게 회전
        Quaternion rot = Quaternion.Euler(0, 0, Random.Range(0, 360));
        m_muzzleFlash.transform.localRotation = rot;

        //활성화해서 보이게 함
        m_muzzleFlash.enabled = true;

        //불규칙적인 시간 동안 Delay한 다음 MeshRenderer를 비활성화
        yield return new WaitForSeconds(Random.Range(0.01f, 0.03f));

        //비활성화해서 보이지 않게 함
        m_muzzleFlash.enabled = false;
    }
    //--- 총구 불빛 이펙트

    //---- 근접공격 시 공격 판단
    private void OnTriggerStay(Collider other)              //야구방망이와 발차기는 얘를 통해서 피해입힘
    {
        if (other.gameObject.tag.Contains("Zombie") && m_closeAtk == true)  //타겟이 좀비고 공격을 확인했다면
        {
            SoundMgr.inst.SoundPlay(SoundList.Weapon, m_itemInfo);          //무기에 맞는 사운드 재생
            ZombieCtrl a_ZCtrl = other.GetComponent<ZombieCtrl>();
            a_ZCtrl.TakeDamage(other.bounds.max, m_itemInfo.m_damage, a_ZCtrl.m_attackDist / 2.0f);      //좀비의 공격거리의 절반만큼 밀림
        }
    }
    //---- 근접공격 시 공격 판단
}
