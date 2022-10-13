using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//인게임에서 사용되는 UI들의 동작을 담당
//"UICanvas" 게임 오브젝트에 붙여서 사용

public class CanvasCtrl : MonoBehaviour
{
    public static CanvasCtrl inst = null;

    [Header("------ ChildrenObjects ------")]       
    //---------"UICanvas" 하위 오브젝트들
    public GameObject m_crossHair = null;           //"Crosshair" 오브젝트
    public Text m_explainMessage;                   //"ExplainText" 오브젝트
    public GameObject m_playerState = null;         //"PlayerState" 오브젝트
    public GameObject m_chrText = null;             //"ChrText" 오브젝트
    public GameObject m_exitPanel = null;           //"EndPanel" 오브젝트
    GameObject m_ddGO = null;               //[Drag&DropPanel] 프리팹 생성 후 담을 게임오브젝트
    //---------"UICanvas" 하위 오브젝트들

    [Header("--------- Player State ---------")]
    //-------- "PlayerState" 하위 오브젝트들
    public Image m_hpBar = null;                    //HP바 이미지
    public Text m_hpText = null;                    //현재 남은 HP 표시해주는 텍스트
    public Image m_staminaBar = null;               //스테미너 바 이미지

    public GameObject m_weaponImage = null;         //현재 장착한 무기 이미지
    public GameObject m_helmetImage = null;         //현재 장착한 헬멧 이미지
    public GameObject m_armourImage = null;         //현재 장착한 아머 이미지
    public Text m_wMagazineText = null;             //무기의 장탄 수 표시해 줄 텍스트 변수
    public Text m_hMagazineText = null;             //헬멧의 내구도 표시해 줄 텍스트 변수
    public Text m_aMagazineText = null;             //아머의 내구도 표시해 줄 텍스트 변수
    //-------- "PlayerState" 하위 오브젝트들

    //------------ Config Box(환경설정 박스 관련)
    [Header ("--------- Config Box ---------")]
    public GameObject m_configBox = null;           //[ConfigBox] 프리팹
    [HideInInspector] public bool m_cfbOnOff = false;   //현재 ConfigBox가 열려있는지 확인하기 위한 변수
    GameObject m_go = null;                 //생성된 [ConfigBox] 프리팹을 담을 변수
    //------------ Config Box(환경설정 박스 관련)

    //---------- Drag&DropPanel(드래그 앤 드랍 관련)
    [Header ("------ Drag&DropPanel ------")]
    public GameObject m_dragdropPanel = null;       //[Drag&DropPanel] 프리팹
    //---------- Drag&DropPanel(드래그 앤 드랍 관련)

    //---------- 플레이어 피격 시 나타나는 피 이펙트 관련
    [Header ("------ BloodPrefab ------")]
    public GameObject m_bloodPrefab = null;        //[CBlood(Canvas)] 프리팹
    //---------- 플레이어 피격 시 나타나는 피 이펙트 관련

    //--------- 게임 오버
    [Header("--------- Game Over ---------")]
    public GameObject m_gameoverPanel = null;       //게임오버판넬 프리팹
    //--------- 게임 오버

    private void Awake()
    {
        inst = this;
    }

    // Start is called before the first frame update
    //void Start()
    //{
        
    //}

    // Update is called once per frame
    void Update()
    {
        ExplainMsg();   //드래그앤드랍 창(F키) 오픈 시 메시지 출력관련 함수
        ConfigBox();    //configBox 동작 관련 함수
        PlayerState();  //플레이어 상태창 관련 함수
    }

    public void ExplainMsg()   //아이템 줍기 키 텍스트 표시
    {
        if (PlayerCtrl.inst.m_itemList[0].m_itType != ItemType.Null && m_explainMessage.gameObject.activeSelf == false)        //플레이어 주위에 아이템이 있다면
            m_explainMessage.gameObject.SetActive(true);         //활성화
        else if (PlayerCtrl.inst.m_itemList[0].m_itType == ItemType.Null && m_explainMessage.gameObject.activeSelf == true)   //플레이어 주위에 아이템이 없다면
            m_explainMessage.gameObject.SetActive(false);          //비활성화
    }

    public void DDPanelSet(bool a_bool)     //드래그 앤 드랍 판넬 온 오프(F키 누를 시 PlayerCtrl 스크립트에서 작동 시킴)
    {
        if (a_bool == true)     //온
        {
            PlayerCtrl.inst.m_isRun = !a_bool;                              //플레이어 움직임 정지
            m_ddGO = Instantiate(m_dragdropPanel, gameObject.transform);    //드래그 앤 드랍 판넬 생성
        }
        else  //오프
        {
            NetworkMgr.inst.PushPacket(PacketType.ItemChange);  //서버에 정보 저장
            Destroy(m_ddGO);                                    //생성했던 드래그 앤 드랍 판넬 제거
        }            
    }

    void ConfigBox() //환경설정 박스 온 오프
    {
        if (Input.GetKeyDown(KeyCode.Escape))                   //esc키 누르면 동작
        {
            m_cfbOnOff = !m_cfbOnOff;                           //온->오프 or 오프->온 변경

            if (m_cfbOnOff == true)  //온
            {
                m_go = Instantiate(m_configBox, gameObject.transform);  //환경설정 박스 생성
                Cursor.lockState = CursorLockMode.None;                 //마우스 커서 나타나게 하기
                Time.timeScale = 0.0f;                                  //게임 일시정지
                InGameMgr.s_gameState = GameState.GamePaused;           //게임 상태를 일시정지로 변경
            }
            else //오프
                Destroy(m_go);                                  //생성된 환경설정 박스 제거
        }
    }

    private void PlayerState()  //플레이어 상태창 정보
    {
        if (PlayerCtrl.inst.m_groggy == true)       //플레이어가 그로기 상태일 경우(스테미나를 끝까지 다쓰면 그로기 상태) 색상(회색)설정
            m_staminaBar.color = new Color32(90, 90, 90, 255);
        else                                        //그로기 상태가 아닐 때 색상(노란색)설정
            m_staminaBar.color = new Color32(244, 255, 0, 255);

        m_hpBar.fillAmount = PlayerCtrl.inst.m_curHp / PlayerCtrl.inst.m_maxHp;         //플레이어의 현재 HP표시
        m_staminaBar.fillAmount = PlayerCtrl.inst.m_curSt / PlayerCtrl.inst.m_maxSt;    //플레이어의 현재 스테미나 표시
        m_hpText.text = ((int)PlayerCtrl.inst.m_curHp).ToString();

        //----현재 장착된 헬멧, 아머, 무기의 내구도 표시
        m_hMagazineText.text = string.Format("{0:D2} / {1:D2}",
            GlobalValue.g_equippedItem[0].m_curMagazine,
            GlobalValue.g_equippedItem[0].m_maxMagazine);

        m_aMagazineText.text = string.Format("{0:D2} / {1:D2}",
            GlobalValue.g_equippedItem[1].m_curMagazine,
            GlobalValue.g_equippedItem[1].m_maxMagazine);

        m_wMagazineText.text = string.Format("{0:D2} / {1:D2}",
            GlobalValue.g_equippedItem[2].m_curMagazine,
            GlobalValue.g_equippedItem[2].m_maxMagazine);
        //----현재 장착된 헬멧, 아머, 무기의 내구도 표시
    }

    public void SetBlood()  //플레이어 피격 시 피 효과
    {
        GameObject a_go = Instantiate(m_bloodPrefab, gameObject.transform);     //피 이펙트 생성
        a_go.transform.SetAsFirstSibling();                                     //"UICanvas"의 제일 처음의 하위오브젝트로 옮기기
    }

    public void PlayerDie() //플레이어 사망 시
    {        
        Instantiate(m_gameoverPanel,transform);             //게임오버판넬 생성
        InGameMgr.s_gameState = GameState.GamePaused;       //게임 상태를 일시정지로 변경
        Cursor.lockState = CursorLockMode.None;             //마우스 잠금 해제
    }
}
