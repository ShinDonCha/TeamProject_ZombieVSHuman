using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//좀비를 타격했을 때 십자선에 타격표시 애니메이션을 재생하기 위한 스크립트
//"Crosshair"의 "HitImg"에 붙여서 사용

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
