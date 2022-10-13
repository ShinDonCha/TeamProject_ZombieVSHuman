using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//좀비의 추적대상을 정하고, 상태를 바꿔주기 위한 스크립트
//좀비 프리팹의 "AttackRange" 게임오브젝트에 붙여서 사용

public class ZombieSensing : MonoBehaviour
{
    ZombieCtrl m_zombieCtrl = null;                                     //부모의 ZombieCtrl 스크립트를 가져올 변수
    [HideInInspector] public SphereCollider m_traceCollider = null;     //"AttackRange"에 있는 콜라이더(추적 범위 산정용)

    // Start is called before the first frame update
    void Start()
    {
        m_zombieCtrl = transform.parent.GetComponent<ZombieCtrl>();

        m_traceCollider = GetComponent<SphereCollider>();

        m_traceCollider.radius = m_zombieCtrl.m_zCommon.m_norTraceDist;     //좀비의 현재 추적거리 만큼 콜라이더 크기 변경
    }

    // Update is called once per frame
    //void Update()
    //{

    //}

    private void OnTriggerEnter(Collider other)
    {
        if (m_zombieCtrl.m_zombiestate == ZombieState.Die)
            return;

        if (other.CompareTag("Player"))              //추적거리에 안에 들어온 대상이 플레이어라면
        {            
            m_zombieCtrl.m_zombiestate = ZombieState.Trace;      //좀비를 추적상태로 변경
            m_zombieCtrl.m_aggroTarget = other.gameObject;       //해당플레이어를 추적대상으로 잡음
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (m_zombieCtrl.m_zombiestate == ZombieState.Die)
            return;

        if (other.CompareTag("Player"))              //추적거리 밖으로 대상이 나가면
        {
            m_zombieCtrl.m_zombiestate = ZombieState.Idle;       //좀비를 숨쉬기상태로 변경
            m_zombieCtrl.m_aggroTarget = null;
        }
    }
}
