using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

//���� ���� ��� ��ũ��Ʈ
//"SpawnTrigger" ������Ʈ�� �ٿ��� ���

public class ZombieSpawn : MonoBehaviour
{
    [Header("----- Zombie Spawn -----")]
    int m_spawnCount = 10;              //������ ���� ����
    float m_rangeX = 0;                //���� ������ X����
    float m_rangeZ = 0;                //���� ������ Z����

    List<BoxCollider> m_spawnPoints = new List<BoxCollider>();   //������ġ ���� ����Ʈ
    public GameObject[] m_zombiePrefab = null;    //���� ������

    [Header("------ Cinemachine ------")]
    public CinemachineVirtualCamera m_eventCamera = null;   //���� �̺�Ʈ �߻� �� ������ ������ ���� ī�޶��.   
    public CinemachineVirtualCamera m_playerCamera = null;  //���� �̺�Ʈ �߻� �� ������ �÷��̾� ī�޶�

    public GameObject m_playerHand = null;                  //���� �̺�Ʈ �߻� �� ���⸦ �������ֱ� ���� ����

    bool m_isPlayerin = false;                      //Ʈ���� �߻� Bool ����
    float m_eventTime = 0;                          //�̺�Ʈ ī�޶� �۵��ð��� ���� ����
    float m_charEventTime = 4.0f;                   //�̺�Ʈ ĳ���� ī�޶� �۵��ð��� ���� ����


    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)      //���� ������Ʈ�� ���� BoxCollider�� ���� ����Ʈ�� ���
            m_spawnPoints.Add(transform.GetChild(i).GetComponent<BoxCollider>());
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isPlayerin == true)       //������ ���۵� ����
        {
            m_eventTime += Time.deltaTime;
            m_eventCamera.transform.Rotate(Vector3.up * Time.deltaTime);    //�ش� �̺�Ʈ ī�޶� �¿��� ��� ���ݾ� �̵���Ű�鼭 ȭ�� �����ֱ�
            
            if (m_charEventTime > 0)                //4�� �ڿ� ����
            {
                m_charEventTime -= Time.deltaTime;

                if (m_charEventTime < 0)            //�̶��� �÷��̾� �յ��� ������ �� (�÷��̾� ���� ī�޶�� ȭ�� �ٲ�)
                {
                    m_eventCamera.m_Priority = 1;                               //�̺�Ʈ ī�޶��� �켱���� ���� (4 -> 1)
                    m_playerHand.SetActive(false);                              //�÷��̾ �������� ���� ��� ����
                    CanvasCtrl.inst.m_chrText.SetActive(true);                  //�÷��̾� �ؽ�Ʈ(�ڸ�) ���
                    PlayerCtrl.inst.m_animController.SetTrigger("LookAround");  //�ֺ��� ���Ǵ� �ִϸ��̼� ����
                }
            }            

            if (m_eventTime > 8.0f)     //8�� �Ŀ� �ٽ� ���� ����ī�޶�(�÷��̾��� �Ĺ濡�� �����ִ�)�� ����
            {
                Cursor.lockState = CursorLockMode.Locked;           //���콺 Ŀ�� ��ױ�
                m_playerHand.SetActive(true);                       //���� ���� ���� ��Ÿ���� �ϱ�
                CanvasCtrl.inst.m_crossHair.SetActive(true);        //���ڼ� ��Ÿ���� �ϱ�
                CanvasCtrl.inst.m_playerState.SetActive(true);      //�÷��̾� ����â ��Ÿ���� �ϱ�
                CanvasCtrl.inst.m_chrText.SetActive(false);         //�÷��̾� �ؽ�Ʈ(�ڸ�) �����
                InGameMgr.s_gameState = GameState.GameIng;          //���� �� ���·� ����
                m_charEventTime = 4f;                               //�̺�Ʈ Ÿ�̸� �ʱ�ȭ
                m_eventTime = 0;                                    //�̺�Ʈ Ÿ�̸� �ʱ�ȭ
                m_isPlayerin = false;                               //������ ������
                m_playerCamera.m_Priority = 1;                      //�÷��̾� ���� ī�޶� �켱���� ���� (3 -> 1)
                Destroy(gameObject);                                //�� ������Ʈ �ı�(������ ����)
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))                   //�÷��̾ ��������Ʈ�� �浹���� ��
        {
            for (int i = 0; i < m_spawnPoints.Count; i++)       
                StartCoroutine(SpawnZombie(m_spawnPoints[i]));      //���� ���� ����

            if (m_eventCamera == null)                              //�̺�Ʈ ī�޶� ���� ���������̶��
            {
                m_spawnPoints.Clear();                              //����Ʈ �ʱ�ȭ
                Destroy(gameObject);                                //�� ������Ʈ �ı�(������ ����)
            }
            else                                                    //�̺�Ʈ ī�޶� �ִ� ���������̶��
            {
                CanvasCtrl.inst.m_crossHair.SetActive(false);       //���ڼ� �����
                CanvasCtrl.inst.m_playerState.SetActive(false);     //�÷��̾� ����â �����
                m_isPlayerin = true;                                //���� ���� ����
                m_eventCamera.m_Priority = 4;                       //����ī�޶��� ��ġ 2���� ���� ��ġ�� �켱���� ���� (1 -> 4)
                m_playerCamera.m_Priority = 3;                      //����ī�޶��� ��ġ 2���� ���� ��ġ�� �켱���� ���� (1 -> 3)
                InGameMgr.s_gameState = GameState.GamePaused;       //���� �Ͻ����� ���·� ����
                Cursor.lockState = CursorLockMode.None;             //���콺 ��� ����
                CanvasCtrl.inst.m_chrText.GetComponentInChildren<Text>().text = TextChange(m_eventCamera);  //���� ������ �´� �ؽ�Ʈ�� ����
            }
        }
    }

    IEnumerator SpawnZombie(BoxCollider a_boxCol)               //���� ���� �Լ�
    {
        m_rangeX = a_boxCol.size.x / 2.0f;                      //����Ʈ�� Boxcollider ũ�⿡ ����� ���� ����
        m_rangeZ = a_boxCol.size.z / 2.0f;                      //����Ʈ�� Boxcollider ũ�⿡ ����� ���� ����
        
        for (int i = 0; i < m_spawnCount; i++)                  //���� ����
        {
            int a_xRange = Random.Range(-(int)m_rangeX, (int)m_rangeX);            
            int a_zRange = Random.Range(-(int)m_rangeZ, (int)m_rangeZ);

            Vector3 a_point = new Vector3(a_xRange, 0.0f, a_zRange);     //������ ��ġ�� BoxCollider ������ ����

            int a_rnd = Random.Range(0, m_zombiePrefab.Length);         //������ ���� ������ ����� ����
            Instantiate(m_zombiePrefab[a_rnd], a_boxCol.gameObject.transform.position + a_point, Quaternion.identity,
                        FieldCollector.inst.m_ZombieColl.transform);    //ù��° ����Ʈ���� ���ʴ�� ����, ��������� ���ϵ�� �ֱ�
        }

        yield return new WaitForEndOfFrame();
    }

    string TextChange(CinemachineVirtualCamera cineCamera)            //�ó׸ӽ� �̺�Ʈ ī�޶󸶴� �ؽ�Ʈ ����
    {
        string a_str = "";

        switch (cineCamera.name)
        {
            case "CM vcam1":
                a_str = "���� ? ����� ����� ������..? �ϴ� �ֺ��� �������..! ";
                break;
            case "CM vcam2":
                a_str = "�ٷ� ���� �������� ���ݾ�..!\n �� �����ϰ����� �׷��� ����� ���ؾ��ϴ� �����߰ڴ°� ";
                break;
            case "CM vcam3":
                a_str = "�� ���δ� ���������� ���������� ���ư��߰ڴ� ��..? ";
                break;
            case "CM vcam4":
                a_str = "������ ������ ���� �ٱ��ʿ� �ٸ��� ���� ���ΰ� ���� ���� ������..\n��������� �������� ";
                break;
            case "CM vcam5":
                a_str = "�� �ٸ��� �ǳʰ��� �������� ������?";
                break;
        }

        return a_str;        
    }
}
