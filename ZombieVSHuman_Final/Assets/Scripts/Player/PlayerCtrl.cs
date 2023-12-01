using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//플레이어 이동, 애니메이션, 공격 등 플레이어의 동작과 관련된 대부분을 담당하는 스크립트
//"Player"에 붙여서 사용

[RequireComponent(typeof(Rigidbody))]   //자동으로 typeof("")  ""안에 있는 컴포넌트를 추가함.
public class PlayerCtrl : MonoBehaviour
{
    public static PlayerCtrl inst = null;

    Rigidbody m_myRigidbody;          //플레이어 리지드바디

    //------- 플레이어 이동
    //x 좌표는 기본 수평 Input 값(h), z 좌표는 기본 수직 Input 값 (v)
    float h = 0.0f;         // 기본 수평 Input 값
    float v = 0.0f;         // 기본 수직 Input 값    
    float m_moveSpeed = 0.0f;         //플레이어의 속도를 담을 변수
    float m_normalSpeed = 4.0f;       //기본 속도
    float m_runSpeed = 10.0f;         //뛸 때 속도
    Vector3 m_moveInputV;     //스크립트로 캐릭터를 움직일때 쓰는 벡터값(앞뒤)                                           
    Vector3 m_moveInputH;     //스크립트로 캐릭터를 움직일때 쓰는 벡터값(좌우) 
    Vector3 m_nextMoveV;      //방향 벡터값에 움직이는 속도를 곱해준 벡터값
    Vector3 m_nextMoveH;      //방향 벡터값에 움직이는 속도를 곱해준 벡터값
    //------- 플레이어 이동

    //------- 플레이어 애니메이션
    float m_aniSpeed = 5.0f;  // 애니메이션 블렌더에 적용할 스프린트 / 달리기 변경에 적용되는 변수
    float m_aniRot = 0.0f;   // 애니메이션 블렌더에 적용할 좌측 / 중간 / 우측 캐릭터 회전에 적용되는 변수        
    [HideInInspector] public Animator m_animController;    //플레이어가 사용하는 모델에 적용된 애니메이션 컨트롤러
    //------- 플레이어 애니메이션        

    //----- 플레이어 상태 관련
    [HideInInspector] public float m_curHp = 0.0f;               //현재 체력
    [HideInInspector] public float m_maxHp = 100.0f;             //최대 체력
    [HideInInspector] public float m_curSt = 0.0f;               //스태미나
    [HideInInspector] public float m_maxSt = 100.0f;             //최대 스태미나
    float m_decSt = 25.0f;                                       //달리기 시 초당 스테미너 감소량
    float m_incSt = 15.0f;                                       //휴식 시 초당 스테미너 증가량
    float m_restTime = 2.0f;                                     //스태미나를 채우기 위해 필요한 시간
    [HideInInspector] public bool m_groggy = false;              //그로기 상태
    bool m_rest = true;                                          //휴식 상태
    //----- 플레이어 상태 관련

    //----- 플레이어 공격
    [HideInInspector] public float m_atkDelayTimer = 0.0f;       //공격딜레이 계산용 변수  
    //----- 플레이어 공격

    //----- 현재 상태 체크용 변수
    [HideInInspector] public bool m_isRun = true;       //움직일 수 있는 상태인지
    [HideInInspector] public bool m_isLoot = false;     //줍기 상태 인지     
    [HideInInspector] public bool m_getGun = false;     //현재 무기가 총기인지
    //----- 현재 상태 체크용 변수

    [HideInInspector] public List<ItemInfo> m_itemList = new List<ItemInfo>();     //현재 플레이어의 충돌반경에 들어온 아이템 리스트    

    [HideInInspector] public WeaponCtrl m_nowWeapon = null;     //현재 플레이어가 소지하고있는 무기

    public GameObject m_bloodEff = null;                //플레이어의 피 튀기는 이펙트 ([CBloodEff] 프리팹)
    public GameObject m_bloodDecal = null;              //혈흔 이미지 ([CBloodDecal] 프리팹)
    public Transform m_upperTr = null;                  //피 이펙트 표시할 위치
    public Transform m_bottomTr = null;                 //혈흔 이미지 출력할 위치

    //플레이어가 장착할 수 있는 아이템 목록
    [Header("------ EquipEnableItem ------")]
    public GameObject[] m_equipEnableWeapons = null;
    public GameObject[] m_equipEnableArmours = null;
    public GameObject[] m_equipEnableHelmets = null;    
    //플레이어가 장착할 수 있는 아이템 목록

    // Start is called before the first frame update
    void Awake()
    {
        inst = this;

        m_myRigidbody = GetComponent<Rigidbody>();

        m_animController = GetComponentInChildren<Animator>();

        m_moveSpeed = m_normalSpeed;

        m_curHp = m_maxHp;
        m_curSt = m_maxSt;

        for(int i = 0; i < 28; i++)
            m_itemList.Add(new ItemInfo());             //리스트 28개(루트판넬의 슬롯개수 만큼) 만들어놓기

        transform.position = GlobalValue.g_CharPos;     //데이터베이스에서 불러온 플레이어의 위치 적용

        if (GlobalValue.g_StandUpAnim == true)          //신규 유저일 경우 일어서는 애니메이션 재생
            m_animController.SetTrigger("StandUp");
    }

    // Update is called once per frame
    void Update()
    {
        if (InGameMgr.s_gameState == GameState.GamePaused)
            return;

        if (m_animController.GetCurrentAnimatorStateInfo(0).IsName("stand up") == true)
            return;

        Move();             //플레이어 움직임 관련 함수
        Animation();        //플레이어 애니메이션 관련 함수
        Attack();           //플레이어 공격 관련 함수
        RestCheck();        //플레이어 스테미나 관련 함수
    }

    void Move()
    {
        v = Input.GetAxis("Vertical");
        h = Input.GetAxisRaw("Horizontal");

        if (m_isRun == false)       //줍기 동작 중 이동 불가
            return;

        if (m_animController.GetBool("Kick") == true)       //발차기 모션 중 이동 불가
            return;
        
        if (h == 0 && v == 0 && m_animController.GetBool("Roll") == false)  //입력 없을 시 아래 코드 동작 안함
            return;

        m_moveInputV = transform.forward * v;                   //회전상태에서의 정면값과 v을 곱해서 현재 바라보고있는 방향을 기준으로 움직이도록 함
        m_moveInputH = transform.right * h;                     //회전상태에서의 우측값과 h를 곱해서 현재 바라보고있는 방향을 기준으로 움직이도록 함

        m_nextMoveV = m_moveInputV.normalized * m_moveSpeed;       //방향 벡터값에 움직이는 속도를 곱해서 앞뒤로 이동할 거리를 구함
        m_nextMoveH = m_moveInputH.normalized * m_moveSpeed;       //방향 벡터값에 움직이는 속도를 곱해서 좌우로 이동할 거리를 구함

        //---------- 달리기
        if (Input.GetKey(KeyCode.LeftShift) && v > 0
            && m_nowWeapon.m_crossCtrl.m_zoomInOut == false && m_groggy == false)   //줌인 상태가 아니거나 스테미나가 있을 경우만 가능    
        {     
            m_rest = false;                         //스태미나 소모하는 중
            m_curSt -= m_decSt * Time.deltaTime;    //스테미나 감소
            m_moveSpeed = m_runSpeed;               //이동속도 변경

            if (m_aniSpeed < m_runSpeed)                    //블렌더에 적용스킬 스피드 값 (이 값에 따라 걷기 -> 뛰기로 서서히 변함)
                m_aniSpeed += Time.deltaTime * 7;
            else if (m_aniSpeed > m_runSpeed)               //최대 속도
                m_aniSpeed = m_runSpeed;
        }
        //---------- 달리기

        //------ 걷기
        else                                        //기본 걷는 상태일때
        {
            m_rest = true;                          //스테미나 회복하는 중
            m_moveSpeed = m_normalSpeed;            //이동속도 변경

            //----- 줌인아웃
            if (m_nowWeapon.m_crossCtrl.m_zoomInOut == true)    //줌인 상태
            {
                m_animController.SetBool("ZoomIn", true);   //애니메이터에 정보 전달
                m_moveSpeed /= 2;                           //줌인 상태라면 이동속도가 절반
                m_animController.speed = 0.5f;              //애니메이션 재생속도 절반
            }
            else //줌아웃 상태
            {
                m_animController.SetBool("ZoomIn", false);  //애니메이터에 정보 전달
                m_animController.speed = 1.0f;              //애니메이션 재생속도 원래대로
            }
            //----- 줌인아웃

            if (m_aniSpeed > m_normalSpeed)          //뛰고있는 상태인 블렌더에 줄어드는 속도를 보내 애니메이션 변경시키기
            {
                m_aniSpeed -= Time.deltaTime * 7;
                if (m_aniSpeed < m_normalSpeed)     //달리기 -> 걷기로 애니메이션 서서히 변경
                    m_aniSpeed = m_normalSpeed;
            }
        }
        //------ 걷기

        //---- 플레이어 이동
        if (m_animController.GetBool("Roll") == true)          //구르기 중 일 때 플레이어 움직임
            m_myRigidbody.MovePosition(m_myRigidbody.position + transform.forward * m_moveSpeed * Time.deltaTime);
        else                                                    //기본 움직일 때 플레이어 움직임
        {
            m_myRigidbody.MovePosition(m_myRigidbody.position + m_nextMoveV * Time.deltaTime);         //플레이어 이동
            m_myRigidbody.MovePosition(m_myRigidbody.position + m_nextMoveH * Time.deltaTime);         //플레이어 이동
        }
        //---- 플레이어 이동
    }

    void Animation()
    {
        m_animController.SetBool("Get Gun", m_getGun);          //총 들었을 때 애니메이션 변경

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (m_nowWeapon.m_crossCtrl.m_isReloading == true)          //재장전 중 아이템줍기 불가
                return;

            if (m_animController.GetCurrentAnimatorStateInfo(0).IsTag("Loot") &&           
                m_animController.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.99)   //줍는 애니메이션이나 일어서는 애니메이션이 재생 중에 다시 동작 불가
                return;

            if( m_animController.IsInTransition(0) == true)      //애니메이션 변환 중일 때도 재동작 불가
                return;

            m_isLoot = !m_isLoot;      //줍기 상태인지 아닌지

            m_animController.SetBool("isLoot", m_isLoot);      //줍기 관련 애니메이션 재생 (줍기, 일어나기)

            CanvasCtrl.inst.DDPanelSet(m_isLoot);              //슬롯 세팅 (현재 상태에 따라 "Drag&DropPanel"을 생성하거나 삭제시킴)
        }

        if (Input.GetKeyDown(KeyCode.Space))                //구르기 애니메이션 출력
        {
            if (m_curSt < 20.0f  || m_groggy == true
                || m_animController.GetBool("Roll") == true 
                || m_animController.GetBool("Kick") == true
                || m_nowWeapon.m_crossCtrl.m_zoomInOut == true
                || m_nowWeapon.m_crossCtrl.m_isReloading == true)    //스테미너가 없거나 다른동작 중일 때 불가
                return;

            m_animController.SetBool("Roll", true);         //애니메이션 재생
            m_nowWeapon.m_crossCtrl.RollCrossCtrl();        //구르기 시 십자선 변경
            m_curSt -= 20.0f;                               //스테미나 감소
            m_restTime = 2.0f;                              //휴식하기 위한 시간 2초로 늘어남
        }

        if (m_animController.GetCurrentAnimatorStateInfo(0).IsName("Kick"))
        {            
            if(0.7f < (m_animController.GetCurrentAnimatorStateInfo(0).normalizedTime / m_animController.GetCurrentAnimatorStateInfo(0).length))          //발차기 애니메이션 끝날 때 파라미터 변경
            m_animController.SetBool("Kick", false);
        }
            

        if (m_animController.GetCurrentAnimatorStateInfo(0).IsName("Roll") &&
            0.97f < m_animController.GetCurrentAnimatorStateInfo(0).normalizedTime)          //구르기 애니메이션 끝날 때 파라미터 변경
            m_animController.SetBool("Roll", false);

        if (m_animController.GetCurrentAnimatorStateInfo(0).IsName("Loot_off") &&
            0.97f < m_animController.GetCurrentAnimatorStateInfo(0).normalizedTime)     //줍기 애니메이션 끝날 때 이동 가능하게 변경
            m_isRun = true;
                

        //----- 회전 애니메이션(현재는 회전 애니메이션을 제거한 상태)
        if (h > 0)
        {
            m_aniRot += Time.deltaTime * 30;
            if (m_aniRot > 10)
                m_aniRot = 10;
        }
        else if (h < 0)
        {
            m_aniRot -= Time.deltaTime * 30;
            if (m_aniRot < -10)
                m_aniRot = -10;
        }
        else    //좌우측으로의 이동이 없을 때
        {
            if (m_aniRot > 0.1)
                m_aniRot -= Time.deltaTime * 30;
            else if (m_aniRot < -0.1)
                m_aniRot += Time.deltaTime * 30;
            else
                m_aniRot = 0;
        }
        //----- 회전 애니메이션(현재는 회전 애니메이션을 제거한 상태)

        m_animController.SetFloat("Vertical", v * 10);    //애니메이션 컨트롤러에 변수값 전달
        m_animController.SetFloat("Horizontal", m_aniRot); //애니메이션 컨트롤러에 변수값 전달
        m_animController.SetFloat("Speed", m_aniSpeed);    //애니메이션 컨트롤러에 변수값 전달      
    }

    void Attack()
    {
        //---- 공격 가능 상태인지 확인
        if (m_nowWeapon.m_crossCtrl.m_isReloading == true
            || m_animController.GetBool("Roll") == true
            || 5 < m_animController.GetFloat("Speed")
            || m_isRun == false)        //재장전상태나 구르기상태, 뛰기상태, 줍기상태일 경우 공격 불가
            m_nowWeapon.m_misFire = true;
        else
            m_nowWeapon.m_misFire = false;
        //---- 공격 가능 상태인지 확인

        //----- 공격
        if (0.0f < m_atkDelayTimer)             //공격 쿨타임 체크
            m_atkDelayTimer -= Time.deltaTime;
        else if (m_atkDelayTimer <= 0.0f)
        {
            m_atkDelayTimer = 0.0f;
            if (Input.GetMouseButton(0) && m_nowWeapon.m_misFire == false)      //공격가능 상태이고, 공격 실행 시
            {
                if (m_nowWeapon.m_itemInfo.m_itName == ItemName.Kick)           //무기가 맨손일 때
                    m_animController.SetBool("Kick", true);

                else if (m_nowWeapon.m_itemInfo.m_itName == ItemName.Bat)       //무기가 야구방망이일 때
                    m_animController.SetTrigger("Swing");

                else                                                            //무기가 총일 때
                    m_nowWeapon.Fire();

                m_atkDelayTimer = m_nowWeapon.m_itemInfo.m_attackDelay;         //무기별 설정된 공격 딜레이 적용
            }
        }
        //----- 공격        
    }

    public void TakeDamage(float damage)        //공격을 맞았을 때 동작
    {        
        if (m_curHp < 0.0f)
            return;

        damage = DecreaseDamage(ItemType.Armour, damage, 6.0f);   //아머유무에 따른 데미지 감소 함수 실행

        if (0.0f < damage)                                      //남은 대미지가 있으면 실행
            damage = DecreaseDamage(ItemType.HeadGear, damage, 3.0f);  //헬멧유무에 따른 데미지 감소 함수 실행

        CanvasCtrl.inst.SetBlood();                             //"UICanvas"에 피 이미지 출력
        GameObject a_effGO = Instantiate(m_bloodEff, m_upperTr.position, Quaternion.identity);  //플레이어 몸주변에 피 이펙트 생성
        a_effGO.GetComponent<ParticleSystem>().Play();          //파티클 재생
        Destroy(a_effGO, 2.0f);                                 //2초 후 오브젝트 삭제

        GameObject a_decalGO = Instantiate(m_bloodDecal, m_bottomTr.position, Quaternion.Euler(90, 0, Random.Range(0, 360)));    //혈흔의 각도 랜덤으로 해서 생성
        Destroy(a_decalGO, 4.0f);                               //4초 후 오브젝트 삭제

        //------ HP계산
        if (0.0f < m_curHp)         //HP가 남아있으면 감소
        {
            m_curHp -= damage;
            if (m_curHp <= 0.0f)    //HP가 다 달았으면..
            {
                m_curHp = 0.0f;
                m_animController.SetTrigger("Die");             //사망 애니메이션 재생

                switch(m_nowWeapon.m_itemInfo.m_itName)         //현재 들고있는 무기에 따라서 죽는 애니메이션 다르게(블렌드 트리에 전달)
                {
                    case ItemName.Kick:
                        m_animController.SetFloat("DieFloat", 1);
                        break;
                    case ItemName.Bat:
                        m_animController.SetFloat("DieFloat", 2);
                        break;
                    default:
                        m_animController.SetFloat("DieFloat", 3);
                        break;                        
                }

                CanvasCtrl.inst.PlayerDie();    //사망 처리
            }
        }
        //------ HP계산
    }

    //------ 스테미나 회복 관련
    private void RestCheck()
    {
        if (m_curSt <= 0.0f)                            //스테미나를 다 쓴 상태
        {
            m_groggy = true;                            //그로기 상태 온
            m_curSt = m_incSt * Time.deltaTime;         //스테미나 회복
        }
        else if (0.0f < m_curSt && m_curSt < m_maxSt)   //스테미나가 남아있음
        {
            if (m_rest == true)                         //휴식 상태라면
            {
                m_restTime -= Time.deltaTime;
                if (m_restTime <= 0.0f)                 //2초이상 뛰거나 구르지 않았을 경우
                {
                    m_restTime = 0.0f;
                    m_curSt += m_incSt * Time.deltaTime;  //스테미나 회복
                }
            }
            else  //m_rest == false                     //휴식 상태가 아니면
                m_restTime = 2.0f;                      //스테미나 회복에 필요한 시간으로 리셋
        }
        else if (m_maxSt <= m_curSt)                    //최대 충전 시
            m_groggy = false;                           //그로기 상태 해제
    }
    //------ 스테미나 회복 관련

    //---- 플레이어 주변 아이템 감지
    private void OnTriggerEnter(Collider other)         //플레이어가 아이템 주위로 갔을 때
    {
        if (other.CompareTag("Item"))
        {
            for (int i = 0; i < m_itemList.Count; i++)                              //하나의 아이템이 반복적으로 리스트에 들어가는것 방지
            {
                if (m_itemList[i] == other.GetComponent<ItemCtrl>().m_itemInfo)
                    return;
            }

            for (int i = 0; i < m_itemList.Count; i++)
            {
                if (m_itemList[i].m_itType == ItemType.Null)                        //비어있는 리스트 목록을 찾아서
                {
                    m_itemList[i] = other.GetComponent<ItemCtrl>().m_itemInfo;      //정보 넣어주기
                    break;                                                          //반복문 빠져나가기
                }
            }
        }
    }    

    private void OnTriggerExit(Collider other)          //플레이어가 아이템 주위에서 멀어졌을 때
    {
        if (other.CompareTag("Item"))
        {
            for (int i = 0; i < m_itemList.Count; i++)
            {
                if (m_itemList[i] == other.GetComponent<ItemCtrl>().m_itemInfo)     //해당 아이템과 같은 정보인 리스트를 찾아서
                {
                    m_itemList.RemoveAt(i);                                         //리스트 삭제
                    m_itemList.Add(new ItemInfo());                                 //비어있는 리스트 추가
                }
            }
        }
    }
    //---- 플레이어 주변 아이템 감지

    //---- 처음 지형과 충돌했을 때 플레이어의 Constraints 조정(임의로 플레이어가 밀리거나 회전되는 현상 방지)
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Terrain"))
            m_myRigidbody.constraints = (RigidbodyConstraints)116;                  //로테이션 X, Y, Z값 고정, 포지션 Y고정
    }
    //---- 처음 지형과 충돌했을 때 플레이어의 Constraints 조정(임의로 플레이어가 밀리거나 회전되는 현상 방지)

    //--- 데미지 감소 함수
    float DecreaseDamage(ItemType itemType, float takeDamage, float reduceDamage)   //어떤방어구인지, 받는 대미지, 감소시킬 수 있는 대미지
    {
        if (GlobalValue.g_equippedItem[(int)itemType].m_itType == itemType)                             
        {            
            if (reduceDamage <= takeDamage)                                                             //감소시킬 수 있는 대미지가 받은 대미지보다 작거나 같으면
            {
                if ((int)reduceDamage <= GlobalValue.g_equippedItem[(int)itemType].m_curMagazine)       //현재 남아있는 내구도가 감소시킬 수 있는 대미지보다 더 크거나 같으면
                {
                    GlobalValue.g_equippedItem[(int)itemType].m_curMagazine -= (int)reduceDamage;       //내구도 감소
                    takeDamage -= reduceDamage;                                                         //받는 대미지 감소
                }
                else//(GlobalValue.g_equippedItem[(int)itemType].m_curMagazine < (int)reduceDamage)     //남아있는 내구도가 감소시킬 수 있는 대미지보다 작으면
                {
                    takeDamage -= GlobalValue.g_equippedItem[(int)itemType].m_curMagazine;              //남아있는 내구도만큼 받는 대미지 감소
                    GlobalValue.g_equippedItem[(int)itemType].m_curMagazine = (int)0.0f;                //내구도 0
                }
            }
            else//if (takeDamage < reduceDamage)                                                         //감소시킬 수 있는 대미지가 받은 대미지보다 크면
            {
                if ((int)takeDamage <= GlobalValue.g_equippedItem[(int)itemType].m_curMagazine)        //현재 남아있는 내구도가 받는 대미지보다 더 크거나 같으면
                {
                    GlobalValue.g_equippedItem[(int)itemType].m_curMagazine -= (int)takeDamage;        //받는 대미지 만큼 내구도 감소
                    takeDamage = 0.0f;                                                                 //받는 대미지 0
                }
                else//if (GlobalValue.g_equippedItem[(int)ItemType.Armour].m_curMagazine < (int)takeDamage) //남아있는 내구도가 받는 대미지보다 보다 작으면
                {
                    takeDamage -= GlobalValue.g_equippedItem[(int)itemType].m_curMagazine;              //남아있는 내구도만큼 받는 대미지 감소
                    GlobalValue.g_equippedItem[(int)itemType].m_curMagazine = (int)0.0f;                //내구도 0
                }
            }
        }
        return takeDamage;
    }
}   //--- 데미지 감소 함수
