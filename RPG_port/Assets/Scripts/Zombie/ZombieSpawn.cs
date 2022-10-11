using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

//좀비 스폰 담당 스크립트
//"SpawnTrigger" 오브젝트에 붙여서 사용

public class ZombieSpawn : MonoBehaviour
{
    [Header("----- Zombie Spawn -----")]
    int m_spawnCount = 10;              //생성할 좀비 숫자
    float m_rangeX = 0;                //좀비가 생성될 X범위
    float m_rangeZ = 0;                //좀비가 생성될 Z범위

    List<BoxCollider> m_spawnPoints = new List<BoxCollider>();   //스폰위치 담을 리스트
    public GameObject[] m_zombiePrefab = null;    //좀비 프리팹

    [Header("------ Cinemachine ------")]
    public CinemachineVirtualCamera m_eventCamera = null;   //스폰 이벤트 발생 시 보여줄 장면들을 위한 카메라들.   
    public CinemachineVirtualCamera m_playerCamera = null;  //스폰 이벤트 발생 시 보여줄 플레이어 카메라

    public GameObject m_playerHand = null;                  //스폰 이벤트 발생 시 무기를 제거해주기 위한 변수

    bool m_isPlayerin = false;                      //트리거 발생 Bool 변수
    float m_eventTime = 0;                          //이벤트 카메라 작동시간을 위한 변수
    float m_charEventTime = 4.0f;                   //이벤트 캐릭터 카메라 작동시간을 위한 변수


    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)      //하위 오브젝트에 붙은 BoxCollider의 정보 리스트에 담기
            m_spawnPoints.Add(transform.GetChild(i).GetComponent<BoxCollider>());
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isPlayerin == true)       //스폰이 시작된 상태
        {
            m_eventTime += Time.deltaTime;
            m_eventCamera.transform.Rotate(Vector3.up * Time.deltaTime);    //해당 이벤트 카메라를 좌에서 우로 조금씩 이동시키면서 화면 보여주기
            
            if (m_charEventTime > 0)                //4초 뒤에 동작
            {
                m_charEventTime -= Time.deltaTime;

                if (m_charEventTime < 0)            //이때가 플레이어 손동작 보여줄 때 (플레이어 정면 카메라로 화면 바뀜)
                {
                    m_eventCamera.m_Priority = 1;                               //이벤트 카메라의 우선순위 변경 (4 -> 1)
                    m_playerHand.SetActive(false);                              //플레이어가 착용중인 무기 잠시 끄기
                    CanvasCtrl.inst.m_chrText.SetActive(true);                  //플레이어 텍스트(자막) 출력
                    PlayerCtrl.inst.m_animController.SetTrigger("LookAround");  //주변을 살피는 애니메이션 실행
                }
            }            

            if (m_eventTime > 8.0f)     //8초 후에 다시 원래 메인카메라(플레이어의 후방에서 보여주는)로 복귀
            {
                Cursor.lockState = CursorLockMode.Locked;           //마우스 커서 잠그기
                m_playerHand.SetActive(true);                       //장착 중인 무기 나타나게 하기
                CanvasCtrl.inst.m_crossHair.SetActive(true);        //십자선 나타나게 하기
                CanvasCtrl.inst.m_playerState.SetActive(true);      //플레이어 상태창 나타나게 하기
                CanvasCtrl.inst.m_chrText.SetActive(false);         //플레이어 텍스트(자막) 숨기기
                InGameMgr.s_gameState = GameState.GameIng;          //게임 중 상태로 변경
                m_charEventTime = 4f;                               //이벤트 타이머 초기화
                m_eventTime = 0;                                    //이벤트 타이머 초기화
                m_isPlayerin = false;                               //스폰이 끝났음
                m_playerCamera.m_Priority = 1;                      //플레이어 정면 카메라 우선순위 변경 (3 -> 1)
                Destroy(gameObject);                                //이 오브젝트 파괴(스폰이 끝남)
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))                   //플레이어가 스폰포인트와 충돌했을 때
        {
            for (int i = 0; i < m_spawnPoints.Count; i++)       
                StartCoroutine(SpawnZombie(m_spawnPoints[i]));      //좀비 스폰 시작

            if (m_eventCamera == null)                              //이벤트 카메라가 없는 스폰지역이라면
            {
                m_spawnPoints.Clear();                              //리스트 초기화
                Destroy(gameObject);                                //이 오브젝트 파괴(스폰이 끝남)
            }
            else                                                    //이벤트 카메라가 있는 스폰지역이라면
            {
                CanvasCtrl.inst.m_crossHair.SetActive(false);       //십자선 숨기기
                CanvasCtrl.inst.m_playerState.SetActive(false);     //플레이어 상태창 숨기기
                m_isPlayerin = true;                                //스폰 중인 상태
                m_eventCamera.m_Priority = 4;                       //메인카메라의 수치 2보다 높은 수치로 우선순위 변경 (1 -> 4)
                m_playerCamera.m_Priority = 3;                      //메인카메라의 수치 2보다 높은 수치로 우선순위 변경 (1 -> 3)
                InGameMgr.s_gameState = GameState.GamePaused;       //게임 일시정지 상태로 변경
                Cursor.lockState = CursorLockMode.None;             //마우스 잠금 해제
                CanvasCtrl.inst.m_chrText.GetComponentInChildren<Text>().text = TextChange(m_eventCamera);  //스폰 지역에 맞는 텍스트로 변경
            }
        }
    }

    IEnumerator SpawnZombie(BoxCollider a_boxCol)               //좀비 스폰 함수
    {
        m_rangeX = a_boxCol.size.x / 2.0f;                      //리스트의 Boxcollider 크기에 비례한 범위 설정
        m_rangeZ = a_boxCol.size.z / 2.0f;                      //리스트의 Boxcollider 크기에 비례한 범위 설정
        
        for (int i = 0; i < m_spawnCount; i++)                  //좀비 생성
        {
            int a_xRange = Random.Range(-(int)m_rangeX, (int)m_rangeX);            
            int a_zRange = Random.Range(-(int)m_rangeZ, (int)m_rangeZ);

            Vector3 a_point = new Vector3(a_xRange, 0.0f, a_zRange);     //생성될 위치는 BoxCollider 내에서 랜덤

            int a_rnd = Random.Range(0, m_zombiePrefab.Length);         //생성될 좀비 외형은 목록중 랜덤
            Instantiate(m_zombiePrefab[a_rnd], a_boxCol.gameObject.transform.position + a_point, Quaternion.identity,
                        FieldCollector.inst.m_ZombieColl.transform);    //첫번째 포인트부터 차례대로 생성, 좀비모음의 차일드로 넣기
        }

        yield return new WaitForEndOfFrame();
    }

    string TextChange(CinemachineVirtualCamera cineCamera)            //시네머신 이벤트 카메라마다 텍스트 변경
    {
        string a_str = "";

        switch (cineCamera.name)
        {
            case "CM vcam1":
                a_str = "뭐지 ? 사람이 사람을 물어뜯어..? 일단 주변에 무기부터..! ";
                break;
            case "CM vcam2":
                a_str = "바로 옆에 경찰서가 있잖아..!\n 좀 위험하겠지만 그래도 무기는 구해야하니 가봐야겠는걸 ";
                break;
            case "CM vcam3":
                a_str = "이 도로는 막혀있으니 오른쪽으로 돌아가야겠는 걸..? ";
                break;
            case "CM vcam4":
                a_str = "저번에 왔을때 병원 바깥쪽에 다리로 가는 도로가 있을 던것 같은데..\n힘들겠지만 가봐야지 ";
                break;
            case "CM vcam5":
                a_str = "이 다리를 건너가면 안전하지 않을까?";
                break;
        }

        return a_str;        
    }
}
