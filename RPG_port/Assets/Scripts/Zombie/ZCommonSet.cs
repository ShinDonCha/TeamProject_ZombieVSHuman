using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//����鿡�� ���������� ���Ǵ� ������ ���� ��ũ��Ʈ
//ScriptableObject ���� ("ZCommon")

[System.Serializable]
public class ZombieAniClip
{
    public AnimationClip[] m_idleAni = null;            //���񿡰� �ο��� ������ �ִϸ��̼� ���
    public AnimationClip[] m_attackAni = null;          //���񿡰� �ο��� ���� �ִϸ��̼� ���
    public AnimationClip[] m_runAni = null;             //���񿡰� �ο��� �޸��� �ִϸ��̼� ���
    public AnimationClip[] m_dieAni = null;             //���񿡰� �ο��� ��� �ִϸ��̼� ���
}

[CreateAssetMenu(fileName = "ZCommon", menuName = "ScriptableObject/ZCommonSet", order = int.MaxValue)]
public class ZCommonSet : ScriptableObject
{
    public ZombieAniClip m_aniClip;             //ZombieAniClip Ŭ���� ��ü ����
    public GameObject m_bloodEff = null;        //���� �ǰ� �� �̹���
    public GameObject m_bloodDec = null;        //���� �ǰ� �� �ٴڿ� �������� ���� �̹���
    [HideInInspector] public float[] m_decPos = { -0.5f, 0.0f, 0.5f };
    public Texture[] m_decTex = null;           //���� �̹��� ���

    public GameObject m_dropItem = null;        //���� ��� �� ����Ǵ� ������

    [HideInInspector] public float m_maxHp = 100.0f;                     //������ �ִ� ü��
    [HideInInspector] public float m_norTraceDist = 10.0f;               //�⺻ ���� �����Ÿ�
    [HideInInspector] public float m_hitTraceDist = 25.0f;               //������ ���¿��� �ǰ� �� �����Ÿ�

}
