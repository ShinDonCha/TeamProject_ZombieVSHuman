using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//게임오버 시 나타나는 버튼 동작 관련 스크립트
//[GameOverPanel] 프리팹에 사용

public class GameOverCtrl : MonoBehaviour
{
    public Button m_restartBtn = null;              //재시작 버튼
    public Button m_exitBtn = null;                 //게임 종료 버튼

    // Start is called before the first frame update
    void Start()
    {
        if (m_restartBtn != null)       //재시작 버튼
            m_restartBtn.onClick.AddListener(() =>
            {
                NetworkMgr.inst.PushPacket(PacketType.ItemChange);      //현재 아이템 정보 저장 패킷 등록
                InGameMgr.s_gameState = GameState.ReStart;
            });

        if (m_exitBtn != null)          //게임종료 버튼
            m_exitBtn.onClick.AddListener(() =>
            {
                NetworkMgr.inst.PushPacket(PacketType.ItemChange);      //현재 아이템 정보 저장 패킷 등록
                InGameMgr.s_gameState = GameState.GameEnd;
            });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
