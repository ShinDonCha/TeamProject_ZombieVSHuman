using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//좀비의 HP바가 스크린의 정면을 항상 보도록 하기위한 스크립트

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
