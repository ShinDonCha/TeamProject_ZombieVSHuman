using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//"UICanvas"에 생성된 피 이미지 조작 담당 (플레이어 피격 시 생성되는 피 이미지)
//[CBlood(Canvas)] 프리팹에 적용

public class BloodCtrl : MonoBehaviour
{
    public Sprite[] m_bloodImg = null;          //피 이미지 목록
    Image m_Img = null;                         //CBlood(Canvas)의 이미지 컴포넌트 담을 변수
    int m_alphaCon = 44;                        //1초당 줄어들 투명도

    // Start is called before the first frame update
    void Start()
    {
        int a_num = Random.Range(0, m_bloodImg.Length);             //등록된 전체 피 이미지중 랜덤
        m_Img = GetComponent<Image>();                              //이미지 컴포넌트 가져오기
        m_Img.sprite = m_bloodImg[a_num];                           //랜덤 선택된 이미지 적용

        //----- 피 이미지 생성될 스크린상의 좌표, 회전값
        a_num = Random.Range(-35, 36);
        gameObject.transform.localPosition = new Vector3(a_num * 10, 0, 0);
        a_num = Random.Range(0, 361);
        gameObject.transform.localRotation = Quaternion.Euler(0, 0, a_num);
        //----- 피 이미지 생성될 스크린상의 좌표, 회전값
    }

    // Update is called once per frame
    void Update()
    {
        //------ 시간이 흐를수록 피 이미지 사라지는 연출
        Color a_color = m_Img.color;
        a_color.a -= (m_alphaCon * Time.deltaTime) / 255;
        m_Img.color = a_color;

        if(m_Img.color.a <= 0)
            Destroy(gameObject);
        //------ 시간이 흐를수록 피 이미지 사라지는 연출
    }
}
