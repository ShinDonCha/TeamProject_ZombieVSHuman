using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//������ HP�ٰ� ��ũ���� ������ �׻� ������ �ϱ����� ��ũ��Ʈ

public class ZHPBarCtrl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.forward = Camera.main.transform.forward;
    }
}
