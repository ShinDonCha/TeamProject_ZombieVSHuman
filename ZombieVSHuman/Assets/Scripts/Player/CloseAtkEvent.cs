using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�������� (�ָ�, �߱������) �� ������ ������ �����ϱ� ���� ��ũ��Ʈ
//��Ʈ �ֵθ��� �ִϸ��̼ǰ� ������ �ִϸ��̼ǿ� �̺�Ʈ�� �߰��Ǿ�����

public class CloseAtkEvent : MonoBehaviour
{
    void CloseAttack()
    {
        PlayerCtrl.inst.m_nowWeapon.m_closeAtk = !PlayerCtrl.inst.m_nowWeapon.m_closeAtk;
    }
}
