using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//������ ��������� ���ϰ�, ���¸� �ٲ��ֱ� ���� ��ũ��Ʈ
//���� �������� "AttackRange" ���ӿ�����Ʈ�� �ٿ��� ���

public class ZombieSensing : MonoBehaviour
{
    ZombieCtrl m_zombieCtrl = null;                                     //�θ��� ZombieCtrl ��ũ��Ʈ�� ������ ����
    [HideInInspector] public SphereCollider m_traceCollider = null;     //"AttackRange"�� �ִ� �ݶ��̴�(���� ���� ������)

    // Start is called before the first frame update
    void Start()
    {
        m_zombieCtrl = transform.parent.GetComponent<ZombieCtrl>();

        m_traceCollider = GetComponent<SphereCollider>();

        m_traceCollider.radius = m_zombieCtrl.m_zCommon.m_norTraceDist;     //������ ���� �����Ÿ� ��ŭ �ݶ��̴� ũ�� ����
    }

    // Update is called once per frame
    //void Update()
    //{

    //}

    private void OnTriggerEnter(Collider other)
    {
        if (m_zombieCtrl.m_zombiestate == ZombieState.Die)
            return;

        if (other.CompareTag("Player"))              //�����Ÿ��� �ȿ� ���� ����� �÷��̾���
        {            
            m_zombieCtrl.m_zombiestate = ZombieState.Trace;      //���� �������·� ����
            m_zombieCtrl.m_aggroTarget = other.gameObject;       //�ش��÷��̾ ����������� ����
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (m_zombieCtrl.m_zombiestate == ZombieState.Die)
            return;

        if (other.CompareTag("Player"))              //�����Ÿ� ������ ����� ������
        {
            m_zombieCtrl.m_zombiestate = ZombieState.Idle;       //���� ��������·� ����
            m_zombieCtrl.m_aggroTarget = null;
        }
    }
}
