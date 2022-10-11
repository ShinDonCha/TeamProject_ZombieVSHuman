using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//메인카메라(플레이어 후방) 동작 담당 스크립트
//"Main Camera"에 붙여서 사용

public class CameraCtrl : MonoBehaviour
{
    //---- 회전값 계산
    Vector3 m_basicPos = Vector3.zero;      //플레이어에게 적용하기 전에 줌 값만 적용한 카메라 위치
    Vector3 m_changedRot = Vector3.zero;    //변경 된 회전값
    Vector3 m_targetPos = Vector3.zero;     //카메라가 바라볼 위치
    Quaternion m_calcRot;                   //변경 된 회전값(vector3)을 Quaternion으로 변경해서 담는 변수
    float m_rotSpeed = 50.0f;               //초당 회전 속도   
    //---- 회전값 계산
    
    float m_zoomDistance = 0.0f;                             //플레이어와 카메라 사이의 거리       
    [HideInInspector] public float m_maxX = 60.0f;           //카메라 위아래 최대 rotation값
    float m_minX = -60.0f;                                   //카메라 위아래 최소 rotation값


    // Start is called before the first frame update
    void Start()
    {      
        m_zoomDistance = 2.0f;                                  //플레이어와 카메라 사이의 거리 2.0f
        transform.rotation = Quaternion.Euler(Vector3.zero);    //초기 rotation (0.0.0)
    }

    // Update is called once per frame
    void Update()
    {
        if (Cursor.lockState == CursorLockMode.None)        //마우스 잠금이 풀렸을 때는 카메라 회전 없음
            return;

        //------- 카메라 회전
        m_targetPos = PlayerCtrl.inst.transform.position;   
        m_targetPos.y += 2.8f;                              //카메라가 바라볼 곳은 플레이어 위치에서 위로 2.8f만큼 올라간 곳
        
        m_changedRot.y += Input.GetAxis("Mouse X") * m_rotSpeed * Time.deltaTime;   //카메라의 rotation X
        m_changedRot.x -= Input.GetAxis("Mouse Y") * m_rotSpeed * Time.deltaTime;   //카메라의 rotation Y

         //----카메라 위아래 각도 제한
        if (m_maxX < m_changedRot.x)    //카메라의 rotation Y가 최대치를 넘을 수 없음
            m_changedRot.x = m_maxX;        
        if (m_changedRot.x < m_minX)    //카메라의 rotation Y가 최소치 보다 작을 수 없음
            m_changedRot.x = m_minX;
         //----카메라 위아래 각도 제한

        m_zoomDistance = 2 + Mathf.Sin((m_changedRot.x / (m_maxX * 2)) * Mathf.PI);     //카메라와 타겟의 거리 1 ~ 3 (위를 바라볼 때 1, 아래를 볼때 3)   

        m_basicPos.x = 0.0f;
        m_basicPos.y = 0.0f;
        m_basicPos.z = -m_zoomDistance;                                     //줌거리만큼 떨어져서 플레이어를 보기 위해 -사용
        m_calcRot = Quaternion.Euler(m_changedRot.x, m_changedRot.y, 0);    //계산된 회전값 저장
        transform.position = m_calcRot * m_basicPos + m_targetPos;          //타겟에서 2만큼 떨어진 거리를 유지 
        transform.LookAt(m_targetPos);                                      //타겟을 바라보도록 설정
        //------- 카메라 회전

        //------- 플레이어 회전        
        Vector3 a_CamForward = transform.forward;
        a_CamForward.y = 0.0f;
        PlayerCtrl.inst.transform.forward = a_CamForward;                   //카메라가 보는 방향이 플레이어의 정면이 되도록 함
        //------- 플레이어 회전
    }
}
