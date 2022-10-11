using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//총알의 움직임과 대미지 관련 스크립트
//Bullet 프리팹에 붙여서 사용

[RequireComponent(typeof(Rigidbody))]
public class BulletCtrl : MonoBehaviour
{
    [HideInInspector] public float m_bulletSpeed = 2000.0f;          //총알의 속도
    [HideInInspector] public int m_bulletDmg = 0;                    //총알의 대미지(무기마다 다른 수치를 받아옴)

    // Start is called before the first frame update
    void Start()
    {        
        GetComponent<Rigidbody>().AddForce(transform.forward * m_bulletSpeed);  //정면방향으로 직선이동(총알의 방향은 WeaponCtrl에서 공격하는순간 설정됨)

        Destroy(gameObject, 3.0f);          //3초뒤 자동으로 삭제
    }

    // Update is called once per frame
    //void Update()
    //{

    //}

    private void OnTriggerEnter(Collider other)         //어딘가에 충돌했을 경우
    {
        if (other.gameObject.CompareTag("Zombie"))      //대상이 좀비라면
        {
            ZombieCtrl a_ZCtrl = other.GetComponent<ZombieCtrl>();
            a_ZCtrl.TakeDamage(transform.position, m_bulletDmg, a_ZCtrl.m_attackDist / 4.0f);        //좀비 공격거리의 1/4만큼 밀려남
            Destroy(gameObject);
        }
        else if (other.CompareTag("Terrain") || other.CompareTag("Item"))   //지형이나 아이템에 맞을경우 삭제
            Destroy(gameObject);
    }
}
