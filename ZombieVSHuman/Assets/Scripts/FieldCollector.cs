using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//생성된 아이템과 좀비 저장소
//"FieldCollection"에 붙여서 사용

public class FieldCollector : MonoBehaviour
{
    public static FieldCollector inst = null;
    
    public GameObject m_ItemColl = null;
    public GameObject m_ZombieColl = null;

    private void Awake()
    {
        inst = this;
    }
}
