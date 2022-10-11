using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//[ConfigBox]�� ���� ��� ��ũ��Ʈ
//"ConfigBox"�� �ٿ��� ���

public class ConfigBoxCtrl : MonoBehaviour
{
    //----- ���
    [Header("----- Cancel -----")]    
    public Button m_cancelBtn = null;           //"Cancel"
    //----- ���

    //------ �α׾ƿ� & ��������
    [Header("----- Logout & GameEnd -----")]
    public Button m_logoutBtn = null;           //"GoTitle"
    public Button m_exitGameBtn = null;         //"ExitGame"
    public GameObject m_confirmBox = null;      //�������� Ȯ�� �ǳ�(���� ��� X)
    //------ �α׾ƿ� & ��������

    //---- ��ư �̹���
    [Header("----- Btn Image -----")]
    public Sprite[] m_buttonSlideImg = null;
    public Sprite m_buttonOffImg = null;
    //---- ��ư �̹���

    [Header("----- Sound Controller-----")]
    public Button m_bgBtn = null;                    //����� ��ư
    public Slider m_bgSlider = null;                 //����� �����̴�

    public Button m_effBtn = null;                   //ȿ���� ��ư
    public Slider m_effSlider = null;                //ȿ���� �����̴�

    // Start is called before the first frame update
    void Start()
    {    
        if (m_cancelBtn != null)                     //��� ��ư Ŭ�� ��
            m_cancelBtn.onClick.AddListener(() =>
            {
                Destroy(gameObject);                 //������Ʈ ����
                CanvasCtrl.inst.m_cfbOnOff = false;  //ȯ�漳�� �ڽ� ������ ���·� ����
            });

        if (m_logoutBtn != null)                     //�α׾ƿ� ��ư Ŭ�� ��
            m_logoutBtn.onClick.AddListener(() =>
            {
                NetworkMgr.inst.PushPacket(PacketType.ItemChange);      //�α׾ƿ� ���� ������ ���� ������ ����
                NetworkMgr.inst.PushPacket(PacketType.ConfigSet);       //�α׾ƿ� ���� ȯ�漳�� ���� ������ ����
                NetworkMgr.inst.PushPacket(PacketType.CharPosSet);      //�α׾ƿ� ���� ĳ���� ��ġ ������ ����
                InGameMgr.s_gameState = GameState.GoTitle;              //�α׾ƿ� ���·� ����
            });

        if (m_exitGameBtn != null)
            m_exitGameBtn.onClick.AddListener(() =>
            {
                NetworkMgr.inst.PushPacket(PacketType.ItemChange);      //�������� ���� ������ ���� ������ ����
                NetworkMgr.inst.PushPacket(PacketType.ConfigSet);       //�������� ���� ȯ�漳�� ���� ������ ����
                NetworkMgr.inst.PushPacket(PacketType.CharPosSet);      //�������� ���� ĳ���� ��ġ ������ ����
                InGameMgr.s_gameState = GameState.GameEnd;              //�������� ���·� ����
            });

        if (m_bgBtn != null)                //����� ��ư Ŭ�� ��
            m_bgBtn.onClick.AddListener(() =>
            {
                
            });

        if (m_effBtn != null)               //ȿ���� ��ư Ŭ�� ��
            m_effBtn.onClick.AddListener(() =>
            {
                Image a_btnImg = m_effBtn.GetComponent<Image>();
                if (a_btnImg.sprite == m_buttonOffImg)              //���Ұ� ���� -> ���Ұ� ����
                    ButtonImgChange(a_btnImg, m_effSlider);
                else                                                //���Ұ� X -> ���Ұ� ����
                    a_btnImg.sprite = m_buttonOffImg;

                VolumeChange(a_btnImg.sprite);                      //���� ���� �Լ�
            });        
    }

    // Update is called once per frame
    //void Update()
    //{
        
    //}    

    public void SlideValueChange(Image a_btnImg)       //����� �Ǵ� ȿ���� �����̴��� ����� �� ����
    {
        if (EventSystem.current.currentSelectedGameObject == null)     //�����̴��� ���� �����Ѱ� �ƴ϶�� ����
            return;

        Slider a_Slider = EventSystem.current.currentSelectedGameObject.GetComponent<Slider>();     //���� Ŭ�� �� �����̴� ���� ��������
        ButtonImgChange(a_btnImg, a_Slider);           //���� �̹��� ���� �Լ�
        VolumeChange(a_btnImg.sprite);                 //���� ���� �Լ�
    }

    void ButtonImgChange(Image a_btnImg, Slider a_slider)
    {
        //���� �����̴��� value���� ���� �̹��� ����
        if (a_slider.value <= 0.0f)
            a_btnImg.sprite = m_buttonSlideImg[0];
        else if (0.0f < a_slider.value && a_slider.value <= 0.33f)
            a_btnImg.sprite = m_buttonSlideImg[1];
        else if (0.33f < a_slider.value && a_slider.value <= 0.66f)
            a_btnImg.sprite = m_buttonSlideImg[2];
        else if (0.66f < a_slider.value)
            a_btnImg.sprite = m_buttonSlideImg[3];
    }

    public void VolumeChange(Sprite a_btnSprite)
    {
        //----------- ȿ���� ����
        if (a_btnSprite != m_buttonOffImg)                          //���Ұ� �̹����� �ƴ϶��
            SoundMgr.inst.m_audioSource.volume = 1.0f;              //������� �⺻ ���� �ִ�
        else
            SoundMgr.inst.m_audioSource.volume = 0.0f;              //������� �⺻ ���� 0
        //----------- ȿ���� ����
    }

    public void OnEnable()
    {
        if (GlobalValue.g_cfBGImg != null && GlobalValue.g_cfEffImg != null)    //���� ������ �ҷ��� �� ������ ���� �ʾҴٸ�
        {
            m_bgBtn.GetComponent<Image>().sprite = GlobalValue.g_cfBGImg;       //����� �̹��� ����
            m_effBtn.GetComponent<Image>().sprite = GlobalValue.g_cfEffImg;     //ȿ���� �̹��� ����
            m_bgSlider.value = GlobalValue.g_cfBGValue;                         //����� �� ����
            m_effSlider.value = GlobalValue.g_cfEffValue;                       //����� �� ����
        }
    }

    private void OnDestroy()
    {
        //---�۷ι� ������ ���� ����
        GlobalValue.g_cfBGImg = m_bgBtn.GetComponent<Image>().sprite;
        GlobalValue.g_cfBGValue = m_bgSlider.value;
        GlobalValue.g_cfEffImg = m_effBtn.GetComponent<Image>().sprite;
        GlobalValue.g_cfEffValue = m_effSlider.value;
        //---�۷ι� ������ ���� ����

        NetworkMgr.inst.PushPacket(PacketType.ConfigSet);          //������ ȯ�漳�� ���� ����

        DragDropPanelCtrl a_ddPanel = CanvasCtrl.inst.GetComponentInChildren<DragDropPanelCtrl>();  //"UICanvas"�� "Drag&DropPanel"���� ��������

        if (a_ddPanel == null)                              //�������� ���� ���¿��ٸ�
            Cursor.lockState = CursorLockMode.Locked;       //���콺 Ŀ�� ��ױ�

        Time.timeScale = 1.0f;                              //�Ͻ����� ����
        InGameMgr.s_gameState = GameState.GameIng;          //���� ���� �� ���·� ����
    }

    
}
