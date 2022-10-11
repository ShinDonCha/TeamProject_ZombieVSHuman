using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

//목표지점에 플레이어가 도착 했을 때 발생하는 엔딩 스크립트
//"GameEndTrigger"에 붙여서 사용

public class GameEndCtrl : MonoBehaviour
{
    public GameObject m_playerHand = null;                  //플레이어의 손 오브젝트 (무기 숨기기 용)
    public GameObject m_gameEndPanel = null;                //"UICanvas"의 "EndPanel"을 조작할 변수

    public CinemachineVirtualCamera m_chrCamera = null;     //플레이어 정면모습을 보여주기 위한 이벤트 카메라

    public Button m_reStartBtn = null;                      //재시작 버튼
    public Button m_quitBtn = null;                         //게임 종료 버튼

    // Start is called before the first frame update
    void Start()
    {
        m_reStartBtn.onClick.AddListener(() =>
        {
            InGameMgr.s_gameState = GameState.ReStart;
        });
        m_quitBtn.onClick.AddListener(() =>
        {
            InGameMgr.s_gameState = GameState.GameEnd;
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)     //도착지점에 충돌했을 때
    {
        if (other.gameObject.CompareTag("Player"))  //충돌대상이 플레이어라면
        {
            //----UI숨기기 & 활성화
            CanvasCtrl.inst.m_crossHair.SetActive(false);
            CanvasCtrl.inst.m_explainMessage.gameObject.SetActive(false);
            CanvasCtrl.inst.m_playerState.gameObject.SetActive(false);
            CanvasCtrl.inst.m_chrText.gameObject.SetActive(false);
            m_gameEndPanel.SetActive(true);
            //----UI숨기기 & 활성화

            InGameMgr.s_gameState = GameState.GamePaused;               //게임 일시 정지상태로 변경
            Cursor.lockState = CursorLockMode.None;                     //마우스 잠금 해제
            m_playerHand.SetActive(false);                              //플레이어 무기 숨기기
            m_chrCamera.m_Lens.FieldOfView = 68;                        //카메라 배율 변경

            Vector3 a_rotate = new Vector3(24.7f, 0, 0);                  
            m_chrCamera.gameObject.transform.Rotate(a_rotate);          //현재 각도에서 x로 +24.7만큼 회전
            m_chrCamera.m_Priority = 3;                                 //이벤트 카메라의 우선순위 변경 (메인카메라가 2)
            PlayerCtrl.inst.m_animController.SetTrigger("GameEnd");     //엔딩 애니메이션 재생
        }
    }
}
