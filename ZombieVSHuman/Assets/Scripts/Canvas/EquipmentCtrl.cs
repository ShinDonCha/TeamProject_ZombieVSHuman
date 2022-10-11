using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//현재 착용중인 헬멧, 아머, 웨폰에 따라 정보표시 용 UI변경과 플레이어 외형변경, 무기 동작과 관련된 WeaponCtrl 스크립트 변수 변경 담당
//[Drag&DropPanel] 프리팹 내부 "EquipmentPanel"의 하위 오브젝트들 (HeadGear, Armour, Weapon)에 붙여서 동작

public class EquipmentCtrl : MonoBehaviour
{
    public SlotCtrl m_slotCtrl = null;   //현재 오브젝트(HeadGear, Armour, Weapon)에 들어있는 장비의 정보를 가져오기 위한 변수

    // Start is called before the first frame update
    //void Start()
    //{
    //}

    // Update is called once per frame
    //void Update()
    //{
    //}

    //아이템 외형 온오프 함수
    public void ItemOnOff()
    {
        switch(System.Enum.Parse(typeof(ItemType), gameObject.name))    //ItemType과 현재 게임오브젝트의 이름을 비교
        {            
            case ItemType.HeadGear:
                for (int i = 0; i < PlayerCtrl.inst.m_equipEnableHelmets.Length; i++)   //플레이어의 장착가능한 헬멧 목록에서 비교
                {
                    if (PlayerCtrl.inst.m_equipEnableHelmets[i].name.Contains(m_slotCtrl.m_itemInfo.m_itName.ToString()))  //이름 일치하면 온
                        PlayerCtrl.inst.m_equipEnableHelmets[i].SetActive(true);
                    else   //아닌 것들은 오프
                        PlayerCtrl.inst.m_equipEnableHelmets[i].SetActive(false);                    
                }

                //------ 플레이어 정보표시용 UI 변경부분
                if (m_slotCtrl.m_itemInfo.m_itType == ItemType.HeadGear)                 //헬멧을 장착했을 때
                {
                    CanvasCtrl.inst.m_helmetImage.GetComponent<Image>().sprite = m_slotCtrl.m_itemInfo.m_iconImg;
                    CanvasCtrl.inst.m_helmetImage.GetComponent<Image>().color = Color.white;
                }
                else                                                                    //헬멧을 장착하지 않았을 때
                {
                    CanvasCtrl.inst.m_helmetImage.GetComponent<Image>().sprite = Resources.Load("cross", typeof(Sprite)) as Sprite;
                    CanvasCtrl.inst.m_helmetImage.GetComponent<Image>().color = Color.black;
                }

                CanvasCtrl.inst.m_helmetImage.transform.localScale = m_slotCtrl.m_itemInfo.m_iconSize;  //정해놓은 이미지 사이즈로 변경
                break;
                //------ 플레이어 정보표시용 UI 변경부분

            case ItemType.Armour:
                for (int i = 0; i < PlayerCtrl.inst.m_equipEnableArmours.Length; i++)   //플레이어의 장착가능한 아머 목록에서 비교
                {
                    if (PlayerCtrl.inst.m_equipEnableArmours[i].name.Contains(m_slotCtrl.m_itemInfo.m_itName.ToString()))   //이름 일치하면 온
                        PlayerCtrl.inst.m_equipEnableArmours[i].SetActive(true);
                    else   //아닌 것들은 오프
                        PlayerCtrl.inst.m_equipEnableArmours[i].SetActive(false);                   
                }

                //------ 플레이어 정보표시용 UI 변경부분
                if (m_slotCtrl.m_itemInfo.m_itType == ItemType.Armour)                 //아머를 장착했을 때
                {
                    CanvasCtrl.inst.m_armourImage.GetComponent<Image>().sprite = m_slotCtrl.m_itemInfo.m_iconImg;
                    CanvasCtrl.inst.m_armourImage.GetComponent<Image>().color = Color.white;
                }
                else                                                                    //아머를 장착하지 않았을 때
                {
                    CanvasCtrl.inst.m_armourImage.GetComponent<Image>().sprite = Resources.Load("cross", typeof(Sprite)) as Sprite;
                    CanvasCtrl.inst.m_armourImage.GetComponent<Image>().color = Color.black;
                }

                CanvasCtrl.inst.m_armourImage.transform.localScale = m_slotCtrl.m_itemInfo.m_iconSize;
                break;
                //------ 플레이어 정보표시용 UI 변경부분

            case ItemType.Weapon:
                for (int i = 0; i < PlayerCtrl.inst.m_equipEnableWeapons.Length; i++)   //플레이어의 장착가능한 웨폰 목록에서 비교
                {                    
                    if (PlayerCtrl.inst.m_equipEnableWeapons[i].name.Contains(m_slotCtrl.m_itemInfo.m_itName.ToString()))    //이름 일치하면 실행
                    {
                        PlayerCtrl.inst.m_equipEnableWeapons[i].SetActive(true);     //해당하는 플레이어 무기 외형을 온
                        WeaponCtrl a_playerWeapon = PlayerCtrl.inst.m_equipEnableWeapons[i].GetComponent<WeaponCtrl>();  //해당하는 웨폰의 WeaponCtrl 접근
                        a_playerWeapon.m_itemInfo = m_slotCtrl.m_itemInfo;           //현재 착용 장비의 정보로 변경
                        a_playerWeapon.Init();      //초기설정 실행
                    }
                    else
                        PlayerCtrl.inst.m_equipEnableWeapons[i].SetActive(false);   //이름이 일치하지 않는 것들은 오프
                }

                //------ 플레이어 정보표시용 UI 변경부분
                if (m_slotCtrl.m_itemInfo.m_itType == ItemType.Weapon)              //웨폰을 장착했을 때
                {
                    CanvasCtrl.inst.m_weaponImage.GetComponent<Image>().sprite = m_slotCtrl.m_itemInfo.m_iconImg;
                    CanvasCtrl.inst.m_weaponImage.transform.localScale = m_slotCtrl.m_itemInfo.m_iconSize;
                }
                else                                                                //웨폰을 장착하지 않았을 때
                {
                    CanvasCtrl.inst.m_weaponImage.GetComponent<Image>().sprite = Resources.Load("Weapons/Fist", typeof(Sprite)) as Sprite;
                    CanvasCtrl.inst.m_weaponImage.transform.localScale = new Vector3(0.9f, 0.7f, 1.0f);
                }
                
                break;
                //------ 플레이어 정보표시용 UI 변경부분
        }
    }
}
