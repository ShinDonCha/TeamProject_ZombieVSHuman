using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//������ �����۰� ���� �����
//"FieldCollection"�� �ٿ��� ���

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
