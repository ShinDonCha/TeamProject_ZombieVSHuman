using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//게임 종료 시 확인버튼 표시 용 (현재 사용하지 않는 스크립트)

public class ConfirmBoxCtrl : MonoBehaviour
{
    public Button m_OKBtn = null;
    public Button m_CancelBtn = null;

    // Start is called before the first frame update
    void Start()
    {
        if (m_OKBtn != null)
            m_OKBtn.onClick.AddListener(() =>
            {
                NetworkMgr.inst.PushPacket(PacketType.ItemChange);
                InGameMgr.s_gameState = GameState.GameEnd;
            });

        if (m_CancelBtn != null)
            m_CancelBtn.onClick.AddListener(() =>
                Destroy(gameObject));
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
