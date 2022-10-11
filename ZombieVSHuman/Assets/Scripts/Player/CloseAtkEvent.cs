using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//근접공격 (주먹, 야구방망이) 시 공격이 들어갔는지 판정하기 위한 스크립트
//배트 휘두르는 애니메이션과 발차기 애니메이션에 이벤트로 추가되어있음

public class CloseAtkEvent : MonoBehaviour
{
    void CloseAttack()
    {
        PlayerCtrl.inst.m_nowWeapon.m_closeAtk = !PlayerCtrl.inst.m_nowWeapon.m_closeAtk;
    }
}
