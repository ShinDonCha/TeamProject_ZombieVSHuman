using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

//��ǥ������ �÷��̾ ���� ���� �� �߻��ϴ� ���� ��ũ��Ʈ
//"GameEndTrigger"�� �ٿ��� ���

public class GameEndCtrl : MonoBehaviour
{
    public GameObject m_playerHand = null;                  //�÷��̾��� �� ������Ʈ (���� ����� ��)
    public GameObject m_gameEndPanel = null;                //"UICanvas"�� "EndPanel"�� ������ ����

    public CinemachineVirtualCamera m_chrCamera = null;     //�÷��̾� �������� �����ֱ� ���� �̺�Ʈ ī�޶�

    public Button m_reStartBtn = null;                      //����� ��ư
    public Button m_quitBtn = null;                         //���� ���� ��ư

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

    private void OnTriggerEnter(Collider other)     //���������� �浹���� ��
    {
        if (other.gameObject.CompareTag("Player"))  //�浹����� �÷��̾���
        {
            //----UI����� & Ȱ��ȭ
            CanvasCtrl.inst.m_crossHair.SetActive(false);
            CanvasCtrl.inst.m_explainMessage.gameObject.SetActive(false);
            CanvasCtrl.inst.m_playerState.gameObject.SetActive(false);
            CanvasCtrl.inst.m_chrText.gameObject.SetActive(false);
            m_gameEndPanel.SetActive(true);
            //----UI����� & Ȱ��ȭ

            InGameMgr.s_gameState = GameState.GamePaused;               //���� �Ͻ� �������·� ����
            Cursor.lockState = CursorLockMode.None;                     //���콺 ��� ����
            m_playerHand.SetActive(false);                              //�÷��̾� ���� �����
            m_chrCamera.m_Lens.FieldOfView = 68;                        //ī�޶� ���� ����

            Vector3 a_rotate = new Vector3(24.7f, 0, 0);                  
            m_chrCamera.gameObject.transform.Rotate(a_rotate);          //���� �������� x�� +24.7��ŭ ȸ��
            m_chrCamera.m_Priority = 3;                                 //�̺�Ʈ ī�޶��� �켱���� ���� (����ī�޶� 2)
            PlayerCtrl.inst.m_animController.SetTrigger("GameEnd");     //���� �ִϸ��̼� ���
        }
    }
}
