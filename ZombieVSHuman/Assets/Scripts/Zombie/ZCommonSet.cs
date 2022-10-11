using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//좀비들에게 공통적으로 사용되는 변수들 모은 스크립트
//ScriptableObject 생성 ("ZCommon")

[System.Serializable]
public class ZombieAniClip
{
    public AnimationClip[] m_idleAni = null;            //좀비에게 부여할 숨쉬기 애니메이션 목록
    public AnimationClip[] m_attackAni = null;          //좀비에게 부여할 공격 애니메이션 목록
    public AnimationClip[] m_runAni = null;             //좀비에게 부여할 달리기 애니메이션 목록
    public AnimationClip[] m_dieAni = null;             //좀비에게 부여할 사망 애니메이션 목록
}

[CreateAssetMenu(fileName = "ZCommon", menuName = "ScriptableObject/ZCommonSet", order = int.MaxValue)]
public class ZCommonSet : ScriptableObject
{
    public ZombieAniClip m_aniClip;             //ZombieAniClip 클래스 객체 생성
    public GameObject m_bloodEff = null;        //좀비 피격 시 이미지
    public GameObject m_bloodDec = null;        //좀비 피격 시 바닥에 떨어지는 혈흔 이미지
    [HideInInspector] public float[] m_decPos = { -0.5f, 0.0f, 0.5f };
    public Texture[] m_decTex = null;           //혈흔 이미지 목록

    public GameObject m_dropItem = null;        //좀비 사망 시 드랍되는 아이템

    [HideInInspector] public float m_maxHp = 100.0f;                     //좀비의 최대 체력
    [HideInInspector] public float m_norTraceDist = 10.0f;               //기본 좀비 추적거리
    [HideInInspector] public float m_hitTraceDist = 25.0f;               //숨쉬기 상태에서 피격 시 추적거리

}
