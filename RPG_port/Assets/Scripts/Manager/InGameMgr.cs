using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//처음 게임시작 시 인게임 내부의 설정 담당 스크립트
//"InGameMgr"에 붙여서 사용

public enum GameState   //게임의 현재 상태
{
    GameIng,
    GamePaused,
    GameEnd,
    GoTitle,
    ReStart,
}

public class InGameMgr : MonoBehaviour
{
    public static GameState s_gameState = GameState.GameIng;

    public GameObject m_dragDropPanel = null;   //[Drag&DropPanel] 프리팹
    public ConfigBoxCtrl m_cbCtrl = null;       //[ConfigBox] 프리팹의 ConfigBoxCtrl 스크립트 가져오기 위한 변수

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        Time.timeScale = 1.0f;                          //일시정지 해제
        s_gameState = GameState.GameIng;                //게임 진행 중 상태로 변경
        Cursor.lockState = CursorLockMode.Locked;       //마우스 커서를 윈도우 중앙에 고정시킨 후 보이지 않게 하기

        //------ 서버에서 받아온 유저가 장착하고있던 아이템 정보, 외형 적용
        EquipmentCtrl[] a_equipCtrls = m_dragDropPanel.GetComponentsInChildren<EquipmentCtrl>(true);
        
        for (int i = 0; i < a_equipCtrls.Length; i++)
        {
            a_equipCtrls[i].m_slotCtrl.m_itemInfo = GlobalValue.g_equippedItem[i];
            a_equipCtrls[i].ItemOnOff();
        }
        //------ 서버에서 받아온 유저가 장착하고있던 아이템 정보, 외형 적용

        //------ 서버에서 받아온 환경설정 정보 적용
        m_cbCtrl.OnEnable();
        m_cbCtrl.VolumeChange(GlobalValue.g_cfBGImg);
        m_cbCtrl.VolumeChange(GlobalValue.g_cfEffImg);
        //------ 서버에서 받아온 환경설정 정보 적용
    }

    // Update is called once per frame
    //void Update()
    //{

    //}
}
