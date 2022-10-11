using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//필드에 생성된 아이템을 ItemName에 맞게 기본값을 부여해주고, 그에 맞는 외형 생성하는 스크립트
//[Item]프리팹에 붙여서 사용

public class ItemCtrl : MonoBehaviour
{    
    [HideInInspector] public ItemInfo m_itemInfo = new ItemInfo();      //아이템 정보를 저장할 변수
    public GameObject[] m_itemInven = null;                             //아이템의 외형 목록

    // Start is called before the first frame update
    void Start()
    {
        ModelSet();
    }

    public void ModelSet()
    {
        int a_Num = 0;

        if (m_itemInfo.m_itType == ItemType.Null)           //좀비에게서 드랍된 아이템일 경우 랜덤으로 설정
        {
            a_Num = Random.Range(1, m_itemInven.Length);    //기본무기 제외
            m_itemInfo.SetType((ItemName)a_Num);            //랜덤으로 정해진 아이템에 맞게 기본 설정값 적용
        }
        else
            a_Num = (int)m_itemInfo.m_itName;               //좀비로부터 드랍된게 아니면 기존에 있던 정보대로 따름

        m_itemInfo.m_isDropped = true;                      //현재 드랍된 상태
        GameObject a_go = Instantiate(m_itemInven[a_Num]);  //아이템 정보에따라 외형 생성
        a_go.transform.SetParent(transform, false);         //[Item] 프리팹의 차일드로 모델 넣기        
    }
}
