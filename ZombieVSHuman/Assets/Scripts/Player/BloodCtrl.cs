using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//"UICanvas"�� ������ �� �̹��� ���� ��� (�÷��̾� �ǰ� �� �����Ǵ� �� �̹���)
//[CBlood(Canvas)] �����տ� ����

public class BloodCtrl : MonoBehaviour
{
    public Sprite[] m_bloodImg = null;          //�� �̹��� ���
    Image m_Img = null;                         //CBlood(Canvas)�� �̹��� ������Ʈ ���� ����
    int m_alphaCon = 44;                        //1�ʴ� �پ�� ����

    // Start is called before the first frame update
    void Start()
    {
        int a_num = Random.Range(0, m_bloodImg.Length);             //��ϵ� ��ü �� �̹����� ����
        m_Img = GetComponent<Image>();                              //�̹��� ������Ʈ ��������
        m_Img.sprite = m_bloodImg[a_num];                           //���� ���õ� �̹��� ����

        //----- �� �̹��� ������ ��ũ������ ��ǥ, ȸ����
        a_num = Random.Range(-35, 36);
        gameObject.transform.localPosition = new Vector3(a_num * 10, 0, 0);
        a_num = Random.Range(0, 361);
        gameObject.transform.localRotation = Quaternion.Euler(0, 0, a_num);
        //----- �� �̹��� ������ ��ũ������ ��ǥ, ȸ����
    }

    // Update is called once per frame
    void Update()
    {
        //------ �ð��� �带���� �� �̹��� ������� ����
        Color a_color = m_Img.color;
        a_color.a -= (m_alphaCon * Time.deltaTime) / 255;
        m_Img.color = a_color;

        if(m_Img.color.a <= 0)
            Destroy(gameObject);
        //------ �ð��� �带���� �� �̹��� ������� ����
    }
}
