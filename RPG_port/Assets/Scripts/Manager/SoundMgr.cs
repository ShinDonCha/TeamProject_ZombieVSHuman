using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//인게임 내의 사운드 조작 담당 스크립트
//"SoundMgr"에 붙여서 사용

public enum SoundList
{
    Weapon,
    Change,
    Reload
}

public class SoundMgr : MonoBehaviour
{
    public static SoundMgr inst;
    [HideInInspector] public AudioSource m_audioSource = null;   //Audio Source 컴포넌트 가져올 변수

    [Header("----- AudioClip -----")]
    //------ 재생 사운드 클립
    public AudioClip[] m_weaponSound = null;
    public AudioClip m_changeSound = null;
    public AudioClip m_reloadSound = null;
    //------ 재생 사운드 클립

    [Header("----- DefaultVolume -----")]
    //------ 볼륨 기본값 지정 변수
    public float[] m_weaponDefault;
    public float m_changeDefault;
    public float m_reloadDefault;
    //------ 볼륨 기본값 지정 변수

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

    public void SoundPlay(SoundList a_soundList, ItemInfo a_itemInfo = null)           //사운드 재생 함수
    {
        AudioClip a_audioClip = null;                       //오디오 클립 지역변수
        float a_defVolume = 0.0f;                           //볼륨 조절 지역변수

        switch (a_soundList)
        {
            case SoundList.Weapon:
                a_audioClip = m_weaponSound[(int)a_itemInfo.m_itName];          //해당 무기에 맞는 오디오 클립 받아오기
                a_defVolume = m_weaponDefault[(int)a_itemInfo.m_itName];        //해당 무기에 맞는 기본 볼륨값 받아오기
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

        m_audioSource.PlayOneShot(a_audioClip, a_defVolume * GlobalValue.g_cfEffValue);     //오디오 클립과 볼륨에 맞게 사운드 재생
    }

}
