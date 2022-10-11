using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using SimpleJSON;

//TitleScene
//로그인, 회원가입, 로그인 시 서버에 저장된 유저정보 받아오는 용도
//"Title_Mgr"에 붙여서 사용

public class TitleMgr : MonoBehaviour
{
    //---- 로그인 판넬
    [Header("LogInPanel")]
    public GameObject m_logInPanel = null;              //로그인 판넬 오브젝트를 담을 변수
    public InputField m_id_InputField = null;           //로그인 판넬의 id inputfield
    public InputField m_pw_InputField = null;           //로그인 판넬의 pw inputfield
    public Button m_logIn_Btn = null;                   //로그인 버튼
    public Button m_signUp_Btn = null;                  //회원가입 버튼
    //---- 로그인 판넬

    //---- 회원가입 판넬
    [Header("SignUpPanel")]
    public GameObject m_signUpPanel = null;             //회원가입 판넬 오브젝트를 담을 변수
    public InputField m_newid_InputField = null;        //회원가입 판넬의 new id inputfield
    public InputField m_newpw_InputField = null;        //회원가입 판넬의 new pw inputfield
    public InputField m_newnick_InputField = null;      //회원가입 판넬의 new nick inputfield
    public Button m_create_Btn = null;                  //create 버튼
    public Button m_cancel_Btn = null;                  //취소 버튼
    //---- 회원가입 판넬

    //---- 오류메세지 출력용
    [Header("Message")]
    public Text m_messageText = null;                   //메세지를 보여주기 위한 텍스트
    float m_msTimer = 0.0f;                     //메세지 지속 타이머
    //---- 오류메세지 출력용

    //---- Url
    string m_logInUrl;                                  //닷홈의 로그인 php url
    string m_signUpUrl;                                 //닷홈의 회원가입 php url
    //---- Url

    [Header("Sound Img")]
    public Sprite[] m_soundSprite = null;               //환경설정의 사운드 이미지를 적용시키기 위한 이미지 목록

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;         //마우스커서 열기(인게임에서 로그아웃 시 필요)
        
        m_id_InputField.Select();                       //처음에 로그인 판넬 id inputfield에 커서 놓기

        if (m_logIn_Btn != null)
            m_logIn_Btn.onClick.AddListener(LogIn);        

        if (m_signUp_Btn != null)
            m_signUp_Btn.onClick.AddListener(SignUpPanelOn);

        if (m_cancel_Btn != null)
            m_cancel_Btn.onClick.AddListener(LogInPanelOn);

        if (m_create_Btn != null)
            m_create_Btn.onClick.AddListener(SignUp);

        m_logInUrl = "http://zombiehuman.dothome.co.kr/LogIn.php";
        m_signUpUrl = "http://zombiehuman.dothome.co.kr/SignUp.php";
    }

    // Update is called once per frame
    void Update()
    {
        if (0.0f < m_msTimer)                   //일정 시간동안 메세지 출력 후 삭제
        {
            m_msTimer -= Time.deltaTime;
            if (m_msTimer < 0.0f)
                MessageOnOff("", false);
        }

        //----- 커서 이동
        if (Input.GetKeyDown(KeyCode.Tab) ||        //tab키, enter키로 커서 옮기기
            (Input.GetKeyDown(KeyCode.Return) && !EventSystem.current.currentSelectedGameObject.name.Contains("Btn")))
        {            
            Selectable nextSelect = EventSystem.current.currentSelectedGameObject.          //현재 선택되어있는 게임오브젝트의 Select On Down 부분 가져오기
                                    GetComponent<Selectable>().FindSelectableOnDown();

            if (nextSelect != null)            
                nextSelect.Select();         //Select On Down에 등록된 오브젝트 선택하기
        }
        //----- 커서 이동
    }

    //--- 로그인 동작 함수
    void LogIn()
    {
        string a_idStr = m_id_InputField.text.Trim();       //입력된 텍스트 가져오기
        string a_pwStr = m_pw_InputField.text.Trim();       //입력된 텍스트 가져오기

        //--- 오류 발생 시
        if(a_idStr == "" || a_pwStr == "")
        {
            MessageOnOff("ID와 PW를 빈칸없이 입력해야 합니다.");
            return;
        }

        if(!(3 <= a_idStr.Length && a_idStr.Length <= 10))
        {
            MessageOnOff("ID는 3글자 이상 10글자 이하로 입력해 주세요.");
            return;
        }

        if(!(4 <= a_pwStr.Length && a_pwStr.Length <= 15))
        {
            MessageOnOff("PW는 4글자 이상 15글자 이하로 입력해 주세요.");
            return;
        }
        //--- 오류 발생 시

        StartCoroutine(LoginCo(a_idStr, a_pwStr));      //유저가 입력한 ID와 PW를 이용해 로그인 시도
    }

    IEnumerator LoginCo(string idStr, string pwStr)     //로그인 실행 함수
    {
        GlobalValue.g_Unique_ID = "";                   //글로벌 변수 초기화


        //------ Url을 통해 데이터베이스에 접근 후 일치하는 유저정보가 있다면 로그인 허락과 인게임 정보 불러오기
        WWWForm wForm = new WWWForm();
        wForm.AddField("Input_user", idStr, System.Text.Encoding.UTF8);
        wForm.AddField("Input_pass", pwStr, System.Text.Encoding.UTF8);

        UnityWebRequest wRequest = UnityWebRequest.Post(m_logInUrl, wForm);
        yield return wRequest.SendWebRequest();

        if (wRequest.error == null)
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string sz = enc.GetString(wRequest.downloadHandler.data);

            if (!sz.Contains("Login-Success!!") || !sz.Contains("{\""))      //로그인에 실패했거나 JSON형식이 아닐 경우
            {
                ErrorMsg(sz);
                yield break;
            }

            string a_getStr = sz.Substring(sz.IndexOf("{\""));

            var N = JSON.Parse(a_getStr);
            if (N == null)
                yield break;

            GlobalValue.g_Unique_ID = idStr;    //글로벌 변수에 ID저장
            GlobalValue.InitData();             //기본 아이템 목록 생성 (기존 유저일 경우 아래에서 불러온 목록으로 덮어씌우고, 신규유저라면 그대로 사용)

            if (N["nick_name"] != null)
                GlobalValue.g_NickName = N["nick_name"];

            //----- 장비 아이템 불러오기
            if (N["info1"] != null && N["info2"] != null && N["info3"] != null)         //기존 유저일 경우
            {
                string[] a_strJson = new string[3];
                a_strJson[0] = N["info1"];          //아이템 이름 받아오기
                a_strJson[1] = N["info2"];          //장전된 총알 수 받아오기
                a_strJson[2] = N["info3"];          //남은 총알 수 받아오기

                if (a_strJson[0] != "" && a_strJson[1] != "" && a_strJson[2] != "")     //오류가 없을 시 실행
                {
                    //----- 글로벌 변수에 불러온 유저 아이템 정보 저장
                    JSONNode[] a_JS = new JSONNode[3];
                    for (int i = 0; i < a_JS.Length; i++)
                        a_JS[i] = JSON.Parse(a_strJson[i]);

                    for (int j = 0; j < a_JS[0].Count; j++)
                        if (j < GlobalValue.g_equippedItem.Count)
                        {
                            GlobalValue.g_equippedItem[j].SetType((ItemName)System.Enum.Parse(typeof(ItemName), a_JS[0][j]));
                            GlobalValue.g_equippedItem[j].m_curMagazine = a_JS[1][j];
                            GlobalValue.g_equippedItem[j].m_maxMagazine = a_JS[2][j];
                        }
                        else
                        {
                            GlobalValue.g_userItem[j - 3].SetType((ItemName)System.Enum.Parse(typeof(ItemName), a_JS[0][j]));
                            GlobalValue.g_userItem[j - 3].m_curMagazine = a_JS[1][j];
                            GlobalValue.g_userItem[j - 3].m_maxMagazine = a_JS[2][j];
                        }
                    //----- 글로벌 변수에 불러온 유저 아이템 정보 저장
                }
            }
            //----- 장비 아이템 불러오기

            //----- 환경설정 불러오기
            if (N["config"] != null)            //기존 유저라면
            {
                JSONNode a_JS = JSON.Parse(N["config"]);

                foreach (var a_img in m_soundSprite)
                {
                    if (a_img.name == a_JS[0])              //이미지 목록에서 맞는 이름을 찾아
                        GlobalValue.g_cfBGImg = a_img;      //글로벌 변수에 저장
                    if (a_img.name == a_JS[2])              //이미지 목록에서 맞는 이름을 찾아
                        GlobalValue.g_cfEffImg = a_img;     //글로벌 변수에 저장
                }
                GlobalValue.g_cfBGValue = float.Parse(a_JS[1]);     //저장된 볼륨값 글로벌 변수에 저장
                GlobalValue.g_cfEffValue = float.Parse(a_JS[3]);    //저장된 볼륨값 글로벌 변수에 저장
            }
            else //if (N["config"] == null)     //신규 유저라면
            {
                GlobalValue.g_StandUpAnim = true;               //플레이어 스탠딩 애니메이션 재생
                GlobalValue.g_cfBGImg = m_soundSprite[2];       //최대 음량 이미지
                GlobalValue.g_cfEffImg = m_soundSprite[2];      //최대 음량 이미지
            }
            //----- 환경설정 불러오기

            //----- 캐릭터 위치 불러오기
            if (N["Char_pos"] != null)              //기존 유저라면
            {
                JSONNode a_JS = JSON.Parse(N["Char_pos"]);
                GlobalValue.g_CharPos = new Vector3(float.Parse(a_JS[0]), 100.1f, float.Parse(a_JS[1]));    //불러온 위치 적용
            }
            else               //신규 유저라면
                GlobalValue.g_CharPos = new Vector3(-838.7f, 100.1f, 983.72f);   //플레이어 위치를 시작지점으로 설정
            //----- 캐릭터 위치 불러오기

            SceneManager.LoadScene("InGameScene");
        }
        else
        {
            ErrorMsg(wRequest.error);      //에러 표시
        }
        //------ Url을 통해 데이터베이스에 접근 후 일치하는 유저정보가 있다면 로그인 허락과 인게임 정보 불러오기
    }

    //---회원가입
    void SignUp()
    {
        string a_idStr = m_newid_InputField.text.Trim();
        string a_pwStr = m_newpw_InputField.text.Trim();
        string a_nickStr = m_newnick_InputField.text.Trim();

        //---- 오류 발생 시
        if (a_idStr == "" || a_pwStr == "" || a_nickStr == "")
        {
            MessageOnOff("ID와 PW, NickName을 빈칸없이 입력해야 합니다.");
            return;
        }

        if (!(3 <= a_idStr.Length && a_idStr.Length <= 10))
        {
            MessageOnOff("ID는 3글자 이상 10글자 이하로 입력해 주세요.");
            return;
        }

        if (!(4 <= a_pwStr.Length && a_pwStr.Length <= 15))
        {
            MessageOnOff("PW는 4글자 이상 15글자 이하로 입력해 주세요.");
            return;
        }

        if (!(2 <= a_nickStr.Length && a_nickStr.Length <= 6))
        {
            MessageOnOff("NickName은 2글자 이상 6글자 이하로 입력해 주세요.");
            return;
        }
        //---- 오류 발생 시

        StartCoroutine(CreateCo(a_idStr, a_pwStr, a_nickStr));        //유저가 입력한 정보로 회원가입 시도
    }

    //------ Url을 통해 데이터베이스에 접근 후 동일한 유저정보가 없다면 회원가입 성공
    IEnumerator CreateCo(string idStr, string pwStr, string nickStr)
    {
        WWWForm wForm = new WWWForm();
        wForm.AddField("Input_user", idStr, System.Text.Encoding.UTF8);
        wForm.AddField("Input_pass", pwStr);
        wForm.AddField("Input_nick", nickStr, System.Text.Encoding.UTF8);

        UnityWebRequest wRequest = UnityWebRequest.Post(m_signUpUrl, wForm);
        yield return wRequest.SendWebRequest();
              
        if (wRequest.error == null)
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string sz = enc.GetString(wRequest.downloadHandler.data);

            if (sz.Contains("Create Success."))
            {
                MessageOnOff("계정이 생성되었습니다.");
                LogInPanelOn();
            }
            else
            {
                ErrorMsg(sz);       //동일한 ID, NickName이 있을 경우 오류 메시지 표시
                yield break;
            }
        }
        else
            ErrorMsg(wRequest.error);
    }
    //------ Url을 통해 데이터베이스에 접근 후 동일한 유저정보가 없다면 회원가입 성공


    #region 판넬 변경
    void LogInPanelOn()         //로그인 판넬 온
    {
        m_signUpPanel.SetActive(false);
        m_logInPanel.SetActive(true);
        InputFieldClear();
        m_id_InputField.Select();        
    }
    
    void SignUpPanelOn()        //회원가입 판넬 온
    {
        m_logInPanel.SetActive(false);
        m_signUpPanel.SetActive(true);
        InputFieldClear();
        m_newid_InputField.Select();        
    }

    void InputFieldClear()      //입력창 초기화
    {
        m_id_InputField.text = "";
        m_pw_InputField.text = "";
        m_newid_InputField.text = "";
        m_newpw_InputField.text = "";
        m_newnick_InputField.text = "";
    }
    #endregion    

    void MessageOnOff(string Message = "", bool isOn = true)    //메세지 온오프 담당 함수
    {
        if (m_messageText == null)
            return;

        if(isOn == true)        //메세지 온 상태면 3초간 출력
        {
            m_messageText.text = Message;
            m_msTimer = 3.0f;
        }
        else        
            m_messageText.text = "";        

        m_messageText.gameObject.SetActive(isOn);
    }

    //--- 에러 메세지 목록
    void ErrorMsg(string str)
    {
        if (str.Contains("ID does not exist."))
            MessageOnOff("ID가 존재하지 않습니다.");
        else if (str.Contains("Pass does not Match."))
            MessageOnOff("PW가 일치하지 않습니다.");
        else if (str.Contains("ID does exist."))
            MessageOnOff("같은 ID가 이미 존재합니다.");
        else if (str.Contains("Nickname does exist."))
            MessageOnOff("같은 NickName이 이미 존재합니다.");        
        else
            MessageOnOff(str);
    }
}
