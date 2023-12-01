using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//[ConfigBox]의 동작 담당 스크립트
//"ConfigBox"에 붙여서 사용

public class ConfigBoxCtrl : MonoBehaviour
{
    //----- 취소
    [Header("----- Cancel -----")]    
    public Button m_cancelBtn = null;           //"Cancel"
    //----- 취소

    //------ 로그아웃 & 게임종료
    [Header("----- Logout & GameEnd -----")]
    public Button m_logoutBtn = null;           //"GoTitle"
    public Button m_exitGameBtn = null;         //"ExitGame"
    public GameObject m_confirmBox = null;      //게임종료 확인 판넬(현재 사용 X)
    //------ 로그아웃 & 게임종료

    //---- 버튼 이미지
    [Header("----- Btn Image -----")]
    public Sprite[] m_buttonSlideImg = null;
    public Sprite m_buttonOffImg = null;
    //---- 버튼 이미지

    [Header("----- Sound Controller-----")]
    public Button m_bgBtn = null;                    //배경음 버튼
    public Slider m_bgSlider = null;                 //배경음 슬라이더

    public Button m_effBtn = null;                   //효과음 버튼
    public Slider m_effSlider = null;                //효과음 슬라이더

    // Start is called before the first frame update
    void Start()
    {    
        if (m_cancelBtn != null)                     //취소 버튼 클릭 시
            m_cancelBtn.onClick.AddListener(() =>
            {
                Destroy(gameObject);                 //오브젝트 삭제
                CanvasCtrl.inst.m_cfbOnOff = false;  //환경설정 박스 오프인 상태로 변경
            });

        if (m_logoutBtn != null)                     //로그아웃 버튼 클릭 시
            m_logoutBtn.onClick.AddListener(() =>
            {
                NetworkMgr.inst.PushPacket(PacketType.ItemChange);      //로그아웃 전에 아이템 정보 서버에 저장
                NetworkMgr.inst.PushPacket(PacketType.ConfigSet);       //로그아웃 전에 환경설정 정보 서버에 저장
                NetworkMgr.inst.PushPacket(PacketType.CharPosSet);      //로그아웃 전에 캐릭터 위치 서버에 저장
                InGameMgr.s_gameState = GameState.GoTitle;              //로그아웃 상태로 변경
            });

        if (m_exitGameBtn != null)
            m_exitGameBtn.onClick.AddListener(() =>
            {
                NetworkMgr.inst.PushPacket(PacketType.ItemChange);      //게임종료 전에 아이템 정보 서버에 저장
                NetworkMgr.inst.PushPacket(PacketType.ConfigSet);       //게임종료 전에 환경설정 정보 서버에 저장
                NetworkMgr.inst.PushPacket(PacketType.CharPosSet);      //게임종료 전에 캐릭터 위치 서버에 저장
                InGameMgr.s_gameState = GameState.GameEnd;              //게임종료 상태로 변경
            });

        if (m_bgBtn != null)                //배경음 버튼 클릭 시
            m_bgBtn.onClick.AddListener(() =>
            {
                
            });

        if (m_effBtn != null)               //효과음 버튼 클릭 시
            m_effBtn.onClick.AddListener(() =>
            {
                Image a_btnImg = m_effBtn.GetComponent<Image>();
                if (a_btnImg.sprite == m_buttonOffImg)              //음소거 상태 -> 음소거 해제
                    ButtonImgChange(a_btnImg, m_effSlider);
                else                                                //음소거 X -> 음소거 상태
                    a_btnImg.sprite = m_buttonOffImg;

                VolumeChange(a_btnImg.sprite);                      //볼륨 변경 함수
            });        
    }

    // Update is called once per frame
    //void Update()
    //{
        
    //}    

    public void SlideValueChange(Image a_btnImg)       //배경음 또는 효과음 슬라이더가 변경될 때 실행
    {
        if (EventSystem.current.currentSelectedGameObject == null)     //슬라이더를 직접 조작한게 아니라면 중지
            return;

        Slider a_Slider = EventSystem.current.currentSelectedGameObject.GetComponent<Slider>();     //현재 클릭 된 슬라이더 정보 가져오기
        ButtonImgChange(a_btnImg, a_Slider);           //사운드 이미지 변경 함수
        VolumeChange(a_btnImg.sprite);                 //볼륨 변경 함수
    }

    void ButtonImgChange(Image a_btnImg, Slider a_slider)
    {
        //현재 슬라이더의 value값에 따라 이미지 변경
        if (a_slider.value <= 0.0f)
            a_btnImg.sprite = m_buttonSlideImg[0];
        else if (0.0f < a_slider.value && a_slider.value <= 0.33f)
            a_btnImg.sprite = m_buttonSlideImg[1];
        else if (0.33f < a_slider.value && a_slider.value <= 0.66f)
            a_btnImg.sprite = m_buttonSlideImg[2];
        else if (0.66f < a_slider.value)
            a_btnImg.sprite = m_buttonSlideImg[3];
    }

    public void VolumeChange(Sprite a_btnSprite)
    {
        //----------- 효과음 설정
        if (a_btnSprite != m_buttonOffImg)                          //음소거 이미지가 아니라면
            SoundMgr.inst.m_audioSource.volume = 1.0f;              //오디오의 기본 볼륨 최대
        else
            SoundMgr.inst.m_audioSource.volume = 0.0f;              //오디오의 기본 볼륨 0
        //----------- 효과음 설정
    }

    public void OnEnable()
    {
        if (GlobalValue.g_cfBGImg != null && GlobalValue.g_cfEffImg != null)    //유저 정보를 불러올 때 오류가 나지 않았다면
        {
            m_bgBtn.GetComponent<Image>().sprite = GlobalValue.g_cfBGImg;       //배경음 이미지 변경
            m_effBtn.GetComponent<Image>().sprite = GlobalValue.g_cfEffImg;     //효과음 이미지 변경
            m_bgSlider.value = GlobalValue.g_cfBGValue;                         //배경음 값 변경
            m_effSlider.value = GlobalValue.g_cfEffValue;                       //배경음 값 변경
        }
    }

    private void OnDestroy()
    {
        //---글로벌 변수에 정보 저장
        GlobalValue.g_cfBGImg = m_bgBtn.GetComponent<Image>().sprite;
        GlobalValue.g_cfBGValue = m_bgSlider.value;
        GlobalValue.g_cfEffImg = m_effBtn.GetComponent<Image>().sprite;
        GlobalValue.g_cfEffValue = m_effSlider.value;
        //---글로벌 변수에 정보 저장

        NetworkMgr.inst.PushPacket(PacketType.ConfigSet);          //서버에 환경설정 정보 저장

        DragDropPanelCtrl a_ddPanel = CanvasCtrl.inst.GetComponentInChildren<DragDropPanelCtrl>();  //"UICanvas"의 "Drag&DropPanel"정보 가져오기

        if (a_ddPanel == null)                              //열려있지 않은 상태였다면
            Cursor.lockState = CursorLockMode.Locked;       //마우스 커서 잠그기

        Time.timeScale = 1.0f;                              //일시정지 해제
        InGameMgr.s_gameState = GameState.GameIng;          //게임 진행 중 상태로 변경
    }

    
}
