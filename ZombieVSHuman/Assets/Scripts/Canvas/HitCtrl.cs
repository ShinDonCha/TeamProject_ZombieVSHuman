using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���� Ÿ������ �� ���ڼ��� Ÿ��ǥ�� �ִϸ��̼��� ����ϱ� ���� ��ũ��Ʈ
//"Crosshair"�� "HitImg"�� �ٿ��� ���

public class HitCtrl : MonoBehaviour
{
    public static HitCtrl inst = null;
    [HideInInspector] public Animator HitAnim = null;

    // Start is called before the first frame update
    void Start()
    {
        inst = this;
        HitAnim = GetComponent<Animator>();
    }

    // Update is called once per frame

}
