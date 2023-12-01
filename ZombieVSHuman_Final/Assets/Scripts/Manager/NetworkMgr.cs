using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

//서버에 저장할 값을 전달해주는 스크립트
//"NetworkMgr"에 붙여서 사용

public enum PacketType      //서버에 보낼 패킷종류
{
    ItemChange,
    ConfigSet,
    CharPosSet,
}

public class NetworkMgr : MonoBehaviour
{
    public static NetworkMgr inst = null;

    bool isNetworkLock = false;             //현재 서버에 전송중인 정보가 있는지 판별 하기 위한 변수 (먼저 요청된 것부터 처리하기 위함)
    List<PacketType> m_packetBuff = new List<PacketType>();     //요청된 패킷종류를 담을 리스트

    //--- Url
    string m_saveItemUrl = "";
    string m_saveConfigUrl = "";
    string m_saveCharPosUrl = "";
    //--- Url

    // Start is called before the first frame update
    void Awake()
    {
        inst = this;
        m_saveItemUrl = "http://dhosting.dothome.co.kr/ZombieHuman/SaveItem.php";
        m_saveConfigUrl = "http://dhosting.dothome.co.kr/ZombieHuman/SaveConfig.php";
        m_saveCharPosUrl = "http://dhosting.dothome.co.kr/ZombieHuman/SaveCharPos.php";
    }

    // Update is called once per frame
    void Update()
    {
        if (isNetworkLock == false)          //현재 패킷 처리중이 아니고
            if (0 < m_packetBuff.Count)      //리스트에 처리해야할 패킷이 있다면
                ReqNetwork();
            else                             //리스트에 처리해야할 패킷이 없다면
                ExitGame();                  //게임 종료, 재시작, 로비로 가기 상태일 때 동작을 실행시켜주는 함수
    }

    void ReqNetwork()       //서버에 정보를 저장하는 코루틴 실행 요청 함수
    {
        if (m_packetBuff[0] == PacketType.ItemChange)           //아이템 저장
        {
            StartCoroutine(UpdateItemCo());
        }
        else if (m_packetBuff[0] == PacketType.ConfigSet)       //환경설정 저장
        {
            StartCoroutine(UpdateConfigCo());
        }
        else if (m_packetBuff[0] == PacketType.CharPosSet)      //플레이어 위치 저장
        {
            StartCoroutine(UpdateCharPosCo());
        }

        m_packetBuff.RemoveAt(0);           //처리된 패킷 삭제
    }

    void ExitGame()
    {
        if (InGameMgr.s_gameState == GameState.GameEnd)          //게임 종료 상태
        {
            Application.Quit();
        }
        else if (InGameMgr.s_gameState == GameState.ReStart)    //게임 재시작 상태
        {
            SceneManager.LoadScene("InGameScene");
        }
        else if (InGameMgr.s_gameState == GameState.GoTitle)    //로비로 가기 상태
        {
            DataReset();                                        //글로벌 변수 초기화
            SceneManager.LoadScene("TitleScene");
        }
    }

    IEnumerator UpdateItemCo()
    {
        if (string.IsNullOrEmpty(GlobalValue.g_Unique_ID) == true)
            yield break;

        isNetworkLock = true;

        JSONArray[] a_jsArr = { new JSONArray(), new JSONArray(), new JSONArray() };

        for (int i = 0; i < GlobalValue.g_equippedItem.Count; i++)
        {
            a_jsArr[0].Add(GlobalValue.g_equippedItem[i].m_itName.ToString());
            a_jsArr[1].Add(GlobalValue.g_equippedItem[i].m_curMagazine);
            a_jsArr[2].Add(GlobalValue.g_equippedItem[i].m_maxMagazine);
        }

        for (int i = 0; i < GlobalValue.g_userItem.Count; i++)
        {
            a_jsArr[0].Add(GlobalValue.g_userItem[i].m_itName.ToString());
            a_jsArr[1].Add(GlobalValue.g_userItem[i].m_curMagazine);
            a_jsArr[2].Add(GlobalValue.g_userItem[i].m_maxMagazine);
        }

        WWWForm a_form = new WWWForm();
        a_form.AddField("Input_user", GlobalValue.g_Unique_ID, System.Text.Encoding.UTF8);
        a_form.AddField("Item_name", a_jsArr[0].ToString(), System.Text.Encoding.UTF8);
        a_form.AddField("Item_curmag", a_jsArr[1].ToString(), System.Text.Encoding.UTF8);
        a_form.AddField("Item_maxmag", a_jsArr[2].ToString(), System.Text.Encoding.UTF8);


        UnityWebRequest a_www = UnityWebRequest.Post(m_saveItemUrl, a_form);
        yield return a_www.SendWebRequest();

        if (a_www.error != null)     //에러가 있을 경우
        {
            Debug.Log(a_www.error);
        }

        isNetworkLock = false;
    }

    IEnumerator UpdateConfigCo()
    {
        if (string.IsNullOrEmpty(GlobalValue.g_Unique_ID) == true)
            yield break;

        isNetworkLock = true;

        JSONArray a_jsArr = new JSONArray();
        a_jsArr.Add(GlobalValue.g_cfBGImg.name.ToString());
        a_jsArr.Add(string.Format("{0:N2}", GlobalValue.g_cfBGValue));
        a_jsArr.Add(GlobalValue.g_cfEffImg.name.ToString());
        a_jsArr.Add(string.Format("{0:N2}", GlobalValue.g_cfEffValue));

        WWWForm a_form = new WWWForm();
        a_form.AddField("Input_user", GlobalValue.g_Unique_ID, System.Text.Encoding.UTF8);
        a_form.AddField("Sound_option", a_jsArr.ToString(), System.Text.Encoding.UTF8);
                
        UnityWebRequest a_www = UnityWebRequest.Post(m_saveConfigUrl, a_form);
        yield return a_www.SendWebRequest();

        if (a_www.error != null)     //에러가 있을 경우
        {
            Debug.Log(a_www.error);
        }

        isNetworkLock = false;
    }

    IEnumerator UpdateCharPosCo()
    {
        if (string.IsNullOrEmpty(GlobalValue.g_Unique_ID) == true)
            yield break;

        isNetworkLock = true;

        GlobalValue.g_CharPos = PlayerCtrl.inst.transform.position;

        JSONArray a_jsArr = new JSONArray();
        a_jsArr.Add(GlobalValue.g_CharPos.x.ToString());
        a_jsArr.Add(GlobalValue.g_CharPos.z.ToString());

        WWWForm a_form = new WWWForm();
        a_form.AddField("Input_user", GlobalValue.g_Unique_ID, System.Text.Encoding.UTF8);
        a_form.AddField("Char_pos", a_jsArr.ToString(), System.Text.Encoding.UTF8);

        UnityWebRequest a_www = UnityWebRequest.Post(m_saveCharPosUrl, a_form);
        yield return a_www.SendWebRequest();

        if (a_www.error != null)     //에러가 있을 경우
        {
            Debug.Log(a_www.error);
        }

        isNetworkLock = false;

    }

    public void PushPacket(PacketType a_packType)           //패킷 추가 요청 함수
    {
        for (int i = 0; i < m_packetBuff.Count; i++)
            if (m_packetBuff[i] == a_packType)              //이미 패킷이 처리중이면 또 추가하지 않기
                return;

        m_packetBuff.Add(a_packType);                       //아닐경우 추가
    }

    public void DataReset()
    {
        GlobalValue.g_Unique_ID = "";  
        GlobalValue.g_NickName = "";  
        GlobalValue.g_CharPos = Vector3.zero;
        GlobalValue.g_StandUpAnim = false;
        GlobalValue.g_equippedItem.Clear();
        GlobalValue.g_userItem.Clear();
        GlobalValue.g_cfBGImg = null;
        GlobalValue.g_cfBGValue = 1.0f;
        GlobalValue.g_cfEffImg = null;
        GlobalValue.g_cfEffValue = 1.0f;
    }
}
