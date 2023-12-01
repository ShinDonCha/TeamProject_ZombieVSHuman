using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

//좀비의 이동, 애니메이션 등 좀비 동작과 관련된 것들을 담당하는 스크립트
//좀비 프리팹에 붙여서 사용

public enum ZombieState         //좀비 상태들
{
    Idle,
    Damaged,
    Trace,        
    Attack,
    Die,
}

public class ZombieCtrl : MonoBehaviour
{
    Animator m_zombieAni = null;                                           //좀비의 애니메이터 담을 변수
    [HideInInspector] public GameObject m_aggroTarget = null;              //좀비의 타겟(플레이어)
    [HideInInspector] public ZombieState m_zombiestate = ZombieState.Idle; //현재 좀비의 상태
    Rigidbody m_zombieRigid = null;             //좀비의 리지드바디
    ZombieSensing m_zSense = null;              //좀비의 타겟 감지 스크립트

    //---- 좀비 스텟
    [HideInInspector] public float m_attackDist = 1.0f;     //좀비 공격거리
    float m_curHp = 0.0f;                                   //좀비의 현재 체력
    //---- 좀비 스텟

    Vector3 m_calcVec = Vector3.zero;           //타겟과 좀비사이의 벡터 담는 변수
    Vector3 m_calcNor = Vector3.zero;           //타겟과 좀비사이의 방향벡터
    float m_calcMag = 0.0f;                     //타겟과 좀비사이의 거리

    public Image m_hpBarImg = null;             //HP바 이미지

    public ZCommonSet m_zCommon = null;         //ZCommonSet ScriptableObject(좀비의 공통 변수를 담은 오브젝트) 가져올 변수
    int m_aniNum = 0;                           //애니메이션 넘버
    float m_moveSpeed = 3.0f;                   //좀비의 이동 속도

    //------ 숨쉬기 상태에서 피격 시 동작하는 부분
    float m_autoMTime = 5.0f;                   //5초동안 이동
    float m_mTimer = 0.0f;                      //계산용 변수
    //------ 숨쉬기 상태에서 피격 시 동작하는 부분

    [HideInInspector] public bool m_collCheck = false;          //좀비가 지형과 충돌했는지 체크

    //----- 네비게이션 관련 변수
    private NavMeshAgent m_navAgent = null;                     //좀비의 네비매쉬에이전트
    private NavMeshPath m_navPath = null;                       //목적지 까지의 경로
    private int m_curPathIndex = 1;                             //이번에 목표로 할 경로 번호
    private float m_pathLength = 0.0f;                          //전체 경로 거리
    private float m_pathCalcTime = 0.2f;                        //경로 계산을 몇 초마다 할지?
    private float m_pathTimer = 0.0f;                           //계산용 변수
    private float m_moveDurTime = 0.0f;                         //최종 목적지까지 도착하는데 걸리는 시간
    private float m_moveLimTimer = 0.0f;                        //총 경로 이동 제한 시간
    //----- 네비게이션 관련 변수

    // Start is called before the first frame update
    void Start()
    {
        m_zombieRigid = GetComponent<Rigidbody>();

        m_zombieAni = GetComponent<Animator>();        

        m_zSense = GetComponentInChildren<ZombieSensing>();

        m_navAgent = GetComponent<NavMeshAgent>();

        m_navPath = new NavMeshPath();

        SetAni();                                 //좀비들에게 랜덤 애니메이션 지정해주기

        m_curHp = m_zCommon.m_maxHp;
    }

    // Update is called once per frame
    void Update()
    {
        if (InGameMgr.s_gameState == GameState.GamePaused)
            return;

        ZombieStateUp();                                //좀비의 현재 상태에 따른 행동 조절 함수

        //--------- 숨쉬기 상태에서 피격 시
        if (0.0f < m_mTimer)                            //좀비 강제 추적상태 타이머
        {
            m_mTimer -= Time.deltaTime;

            if (m_mTimer <= 0.0f)                      //일정시간 후에
            {
                if (m_zombiestate == ZombieState.Damaged)       //좀비 상태가 바뀌지 않았다면
                    m_zombiestate = ZombieState.Idle;           //숨쉬기 상태로 변경

                m_zSense.m_traceCollider.radius = m_zCommon.m_norTraceDist;    //추적거리 원래대로 25 -> 10
            }
        }
        //--------- 숨쉬기 상태에서 피격시

        if (0.0f < m_pathTimer)                        //좀비 경로탐색 타이머
            m_pathTimer -= Time.deltaTime;
    }

    void ZombieStateUp()
    {
        if (m_aggroTarget != null)         //타겟이 있고(좀비의 Sphere Collider 안에 들어온 플레이어가 타겟) 
        {
            Vector3 a_aggroPos = m_aggroTarget.transform.position;      //타겟의 위치정보 가져오기
            a_aggroPos.y = transform.position.y;                        //좀비의 높이와 같은높이로 보정
            m_calcVec = a_aggroPos - transform.position;                //좀비와 플레이어 사이의 벡터
            m_calcMag = m_calcVec.magnitude;                            //좀비와 플레이어 사이의 거리
            m_calcNor = m_calcVec.normalized;                           //좀비가 플레이어를 보는 방향

            if(m_zombieAni.GetCurrentAnimatorStateInfo(0).IsName("attackBlend") == false)   //공격중이 아니라면 보는방향 바꿔주기
                transform.forward = m_calcNor;             //좀비가 타겟을 보도록 함
        }

        if (m_zombiestate == ZombieState.Trace)           //좀비가 추적상태라면
        {
            if(m_collCheck == true)
                m_zombieRigid.constraints = (RigidbodyConstraints)84;      //좀비의 Constraints 변경

            if (m_calcMag <= m_attackDist)               //좀비와 플레이어 사이의 거리가 공격가능거리보다 안쪽에 있다면            
            {
                m_zombiestate = ZombieState.Attack;      //좀비를 공격 상태로 변경
                ZAnimSet("Attack");
                return;
            }

            if (m_pathTimer <= 0.0f)                //경로 탐색 시작
            {
                m_pathLength = 0.0f;                //계산 했던 경로 길이 초기화
                if(NavPathCalc(transform.position, m_aggroTarget.transform.position) == true)     //경로 계산에 성공했다면
                {
                    m_moveDurTime = m_pathLength / m_moveSpeed;             //최종 목적지까지 도달하는데 걸리는 시간
                    m_moveLimTimer = 0.0f;                                  //제한시간 계산 초기화
                }

                m_pathTimer = m_pathCalcTime;                               //경로 계산 주기 적용
            }

            MoveToPath();                           //경로 이동 시작
            ZAnimSet("Trace");                      //추적 애니메이션 재생
        }

        else if (m_zombiestate == ZombieState.Damaged)                  //좀비가 숨쉬기(Idle)상태에서 공격 받은 상태
        {
            if (m_collCheck == true)
                m_zombieRigid.constraints = (RigidbodyConstraints)84;   //좀비의 Constraints 변경

            ZAnimSet("Trace");                                          //추적 애니메이션 재생

            m_navAgent.velocity = m_moveSpeed * transform.forward;      //공격 받은 방향으로 직선이동
        }

        else if (m_zombiestate == ZombieState.Attack)                   //좀비가 공격상태라면
        {
            if (m_collCheck == true)
                m_zombieRigid.constraints = RigidbodyConstraints.FreezeAll;    //좀비 안밀리게 고정

            if (m_zombieAni.GetCurrentAnimatorStateInfo(0).IsName("attackBlend") == false)  //공격 애니메이션이 다 끝나면 추적상태로 변경
                m_zombiestate = ZombieState.Trace;

        }

        else if (m_zombiestate == ZombieState.Idle)     //좀비가 숨쉬기 상태라면
        {
            if (m_collCheck == true)
                m_zombieRigid.constraints = (RigidbodyConstraints)116;    //좀비의 Constraints 변경

            ZAnimSet("Idle");                           //숨쉬기 애니메이션 재생
        }

        else if (m_zombiestate == ZombieState.Die)      //좀비가 죽은 상태라면
        {
            m_zombieRigid.constraints = RigidbodyConstraints.FreezeAll;    //좀비 안밀리게 고정
            m_aggroTarget = null;                       //타겟 삭제
            ZAnimSet("Die");                            //사망 애니메이션 재생
        }
    }
    

    void ZAnimSet(string newAnim)           //애니메이터의 State 바꿔주는 함수
    {
        if(newAnim == "Trace")              //추적 애니메이션
        {
            if(m_zombieAni.GetBool("IsRun") == false)
                m_zombieAni.SetBool("IsRun", true);
        }
        else if (newAnim == "Attack")       //공격 애니메이션
        {
            m_zombieAni.SetTrigger("IsAttack");
        }
        else if (newAnim == "Idle")         //기본 애니메이션
        {
            if (m_zombieAni.GetBool("IsRun") == true)
                m_zombieAni.SetBool("IsRun", false);
        }
        else if (newAnim == "Die")
        {
            if (m_zombieAni.GetCurrentAnimatorStateInfo(0).IsName("dieBlend"))  //이미 사망 애니메이션 재생 중이라면 반복하지 않음
                return;

            m_zombieAni.SetTrigger("IsDie");
        }
    }

    public void SetAni()           //좀비마다 랜덤 애니메이션 부여해주는 함수
    {
        m_aniNum = Random.Range(0, m_zCommon.m_aniClip.m_idleAni.Length);       //[ZCommon]의 숨쉬기 애니메이션의 개수만큼 랜덤
        m_zombieAni.SetFloat("idleBlend", (float)m_aniNum);

        m_aniNum = Random.Range(0, m_zCommon.m_aniClip.m_runAni.Length);        //[ZCommon]의 무브 애니메이션의 개수만큼 랜덤
        m_zombieAni.SetFloat("runBlend", (float)m_aniNum);

        m_aniNum = Random.Range(0, m_zCommon.m_aniClip.m_attackAni.Length);     //[ZCommon]의 공격 애니메이션의 개수만큼 랜덤
        m_zombieAni.SetFloat("attackBlend", (float)m_aniNum);

        m_aniNum = Random.Range(0, m_zCommon.m_aniClip.m_dieAni.Length);        //[ZCommon]의 죽는 애니메이션의 개수만큼 랜덤
        m_zombieAni.SetFloat("dieBlend", (float)m_aniNum);
    }

    public void TakeDamage(Vector3 hitPoint, float damage, float bwRange)
    {
        if (m_curHp <= 0.0f)
            return;

        HitCtrl.inst.HitAnim.SetTrigger("Hit");         //피격 애니메이션 재생
        
        if (0.0f < m_curHp)
        {            
            m_curHp -= damage;            

            //------- 좀비가 밀려날 방향 계산
            Vector3 a_corVec = PlayerCtrl.inst.transform.position;
            a_corVec.y = transform.position.y;
            Vector3 a_dirVec = (a_corVec - transform.position).normalized;
            //------- 좀비가 밀려날 방향 계산

            if (m_zombiestate == ZombieState.Idle)              //숨쉬기 상태에서 맞은 경우
            {
                m_zombiestate = ZombieState.Damaged;            //직선 추적 상태로 변경
                m_mTimer = m_autoMTime;                         //계산용 타이머 3초로 지정
                gameObject.transform.forward = a_dirVec;        //맞은 방향을 보도록 변경
                m_zSense.m_traceCollider.radius = m_zCommon.m_hitTraceDist;     //좀비의 추적거리를 변경 (10 -> 25)
            }

            transform.position = transform.position - a_dirVec * bwRange;       //일정거리만큼 뒤로 밀림
            CreateBloodEff(hitPoint);                                           //피 이펙트, 혈흔 이미지 생성

            if (m_curHp <= 0.0f)  //이번 공격으로 죽었다면
            {
                m_curHp = 0.0f;
                m_zombiestate = ZombieState.Die;                                       //죽음 상태로 변경
                m_zSense.m_traceCollider.radius = m_zCommon.m_norTraceDist;            //좀비의 추적거리 초기화
                Destroy(gameObject, m_zCommon.m_aniClip.m_dieAni[m_aniNum].length);   //죽는 애니메이션 재생시간 후에 삭제
            }

            m_hpBarImg.fillAmount = m_curHp / m_zCommon.m_maxHp;        //좀비의 HP바 표시
        }
    }

    public void CreateBloodEff(Vector3 a_Pos)
    {
        if (m_curHp <= 0.0f)
            return;

        GameObject a_bloodEff = Instantiate(m_zCommon.m_bloodEff, a_Pos, Quaternion.identity);        //피 이펙트 생성
        a_bloodEff.GetComponent<ParticleSystem>().Play();                                             //피 이펙트 재생
        Destroy(a_bloodEff, 2.0f);      //2초뒤 이펙트 삭제

        //--------- 바닥에 떨어질 피 위치계산
        Vector3 a_vec = transform.position;
        int a_rnd = Random.Range(0, 3);
        a_vec.x += m_zCommon.m_decPos[a_rnd];
        a_rnd = Random.Range(0, 3);
        a_vec.z += m_zCommon.m_decPos[a_rnd];
        //--------- 바닥에 떨어질 피 위치계산

        Quaternion a_rot = Quaternion.Euler(90, 0, Random.Range(0, 360));               //피 각도 랜덤으로

        a_rnd = Random.Range(0, m_zCommon.m_decTex.Length);                             //피 텍스쳐 여러개중 랜덤

        GameObject a_bloodDec = Instantiate(m_zCommon.m_bloodDec, a_vec, a_rot);
        a_bloodDec.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", m_zCommon.m_decTex[a_rnd]);   //마테리얼 텍스쳐 변경

        float a_scale = Random.Range(0.4f, 0.6f);                                       //스케일 조절
        a_bloodDec.transform.localScale = Vector3.one * a_scale;
        Destroy(a_bloodDec, 3.0f);      //3초뒤 혈흔 삭제
    }

    //---- 좀비가 지형에 처음 충돌했을 때 Position Y를 고정시키기
    private void OnCollisionEnter(Collision collision)
    {
        if (m_collCheck == true)
            return;

        if (collision.collider.CompareTag("Terrain"))                   //처음에 좀비가 지형과 닿았음을 감지
        {
            m_collCheck = true;                                         //충돌 했음
            m_navAgent.enabled = true;                                  //지형에 닿았을 때 좀비의 네비게이션 에이전트 컴포넌트 켜주기
        }        
    }
    //---- 좀비가 지형에 처음 충돌했을 때 Position Y를 고정시키기

    void ZombieAttack()        //공격 애니메이션에 이벤트로 붙어있음
    {
        if(m_calcMag <= m_attackDist)       //좀비와 플레이어의 거리가 좀비의 공격거리보다 작다면...(좀비의 공격거리 안에 있을 경우)
        {
            m_aggroTarget.GetComponent<PlayerCtrl>().TakeDamage(10);        //10의 대미지 입힘
        }
    }

    private void OnDisable()        //좀비가 사라질때 실행 (일정 확률로 아이템 생성)
    {
        if (InGameMgr.s_gameState == GameState.GameEnd)     //게임 종료 시 제외
            return;

        Vector3 a_ZPos = transform.position;
        a_ZPos.y = 101.45f;              //아이템 생성위치를 조금 위로 잡아줌

        int a_num = Random.Range(0, 3);         //0~2
        if (a_num == 0)                          //33%확률로 아이템 드랍
            Instantiate(m_zCommon.m_dropItem, a_ZPos, Quaternion.identity, FieldCollector.inst.m_ItemColl.transform);
    }


    #region 경로 계산 및 이동관련 함수
    //---- MoveToPath 관련 변수들..
    private Vector3 a_CurCPos = Vector3.zero;               //현재 좀비의 위치를 담을 변수
    private Vector3 a_CacDestV = Vector3.zero;              //이번에 도착해야 할 목적지
    private Vector3 a_TargetDir = Vector3.zero;             //좀비의 위치부터 목적지 까지의 방향벡터
    private float a_NowStep = 0.0f;                         //한 프레임에 이동할 거리(목적지 도착 체크용)
    private Vector3 a_Velocity = Vector3.zero;              //목적지까지의 방향벡터 * 좀비의 이동속도
    //---- MoveToPath 관련 변수들..

    bool NavPathCalc(Vector3 a_startPos, Vector3 a_targetPos)         //경로 탐색 함수
    {
        //---- 기존 경로 초기화
        m_navPath.ClearCorners();           //기존에 저장돼있던 경로 삭제
        m_curPathIndex = 1;                 //목표로 할 경로 번호 초기화
        //---- 기존 경로 초기화

        if (m_navAgent == null || m_navAgent.enabled == false)          //네비매쉬에이전트가 없거나 꺼져있을 경우 취소
            return false;

        if (NavMesh.CalculatePath(a_startPos, a_targetPos, -1, m_navPath) == false)         //경로 계산 + 못찾았을 경우 취소
            return false;

        if (m_navPath.corners.Length < 2)           //제대로 된 경로가 아닐경우 취소 (m_navPath.corners는 처음위치와 타겟의위치를 찾으면 기본 2)
            return false;

        for(int i = 1; i < m_navPath.corners.Length; i++)
        {
            Vector3 a_lenthVec = m_navPath.corners[i] - m_navPath.corners[i - 1];           //각 경로 사이의 벡터 계산
            m_pathLength += a_lenthVec.magnitude;                                           //각 경로 사이의 길이만큼 추가
        }

        if (m_pathLength <= 0.0f)                   //전체 경로 길이가 0보다 작다면 취소
            return false;

        return true;                                //제대로 경로 계산이 끝났음
    }

    void MoveToPath()
    {
        if(m_navPath == null)
            m_navPath = new NavMeshPath();

        if (m_curPathIndex < m_navPath.corners.Length)
        {
            a_CurCPos = transform.position;
            a_CacDestV = m_navPath.corners[m_curPathIndex];     //이번 목적지 위치
            a_CurCPos.y = a_CacDestV.y;                         //높이 오차 안나도록 같게 만들기
            a_TargetDir = a_CacDestV - a_CurCPos;               //현재 좀비위치부터 목적지까지의 방향벡터 계산
            a_TargetDir.y = 0.0f;
            a_TargetDir.Normalize();

            a_NowStep = m_moveSpeed * Time.deltaTime;           //한 프레임에 이동할 거리
            a_Velocity = m_moveSpeed * a_TargetDir;             //이동할 방향과 거리
            m_navAgent.velocity = a_Velocity;                   //좀비 이동

            if ((a_CacDestV - a_CurCPos).magnitude <= a_NowStep)    //목적지까지의 거리가 한 프레임 거리보다 작거나 같으면 도착한걸로 판정
            {
                m_navPath.corners[m_curPathIndex] = transform.position;     //이전 목적지를 현재 좀비 위치로 변경
                m_curPathIndex++;                                           //다음 목적지를 찾도록 +1해주기
            }

            m_moveLimTimer += Time.deltaTime;
            if (m_moveDurTime <= m_moveLimTimer)                    //총 거리를 이동하는데 필요한 시간만큼 시간이 지나면 경로이동 종료
            {
                m_curPathIndex = m_navPath.corners.Length;
            }
        }
    }
    #endregion
}
