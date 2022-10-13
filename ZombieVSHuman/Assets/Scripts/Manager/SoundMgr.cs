using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�ΰ��� ���� ���� ���� ��� ��ũ��Ʈ
//"SoundMgr"�� �ٿ��� ���

public enum SoundList
{
    Weapon,
    Change,
    Reload
}

public class SoundMgr : MonoBehaviour
{
    public static SoundMgr inst;
    [HideInInspector] public AudioSource m_audioSource = null;   //Audio Source ������Ʈ ������ ����

    [Header("----- AudioClip -----")]
    //------ ��� ���� Ŭ��
    public AudioClip[] m_weaponSound = null;
    public AudioClip m_changeSound = null;
    public AudioClip m_reloadSound = null;
    //------ ��� ���� Ŭ��

    [Header("----- DefaultVolume -----")]
    //------ ���� �⺻�� ���� ����
    public float[] m_weaponDefault;
    public float m_changeDefault;
    public float m_reloadDefault;
    //------ ���� �⺻�� ���� ����

    private void Awake()
    {
        inst = this;
        m_audioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    //void Start()
    //{
    //}

    //// Update is called once per frame
    //void Update()
    //{
        
    //}

    public void SoundPlay(SoundList a_soundList, ItemInfo a_itemInfo = null)           //���� ��� �Լ�
    {
        AudioClip a_audioClip = null;                       //����� Ŭ�� ��������
        float a_defVolume = 0.0f;                           //���� ���� ��������

        switch (a_soundList)
        {
            case SoundList.Weapon:
                a_audioClip = m_weaponSound[(int)a_itemInfo.m_itName];          //�ش� ���⿡ �´� ����� Ŭ�� �޾ƿ���
                a_defVolume = m_weaponDefault[(int)a_itemInfo.m_itName];        //�ش� ���⿡ �´� �⺻ ������ �޾ƿ���
                break;

            case SoundList.Change:
                a_audioClip = m_changeSound;
                a_defVolume = m_changeDefault;
                break;

            case SoundList.Reload:
                a_audioClip = m_reloadSound;
                a_defVolume = m_reloadDefault;
                break;
        }

        m_audioSource.PlayOneShot(a_audioClip, a_defVolume * GlobalValue.g_cfEffValue);     //����� Ŭ���� ������ �°� ���� ���
    }

}
