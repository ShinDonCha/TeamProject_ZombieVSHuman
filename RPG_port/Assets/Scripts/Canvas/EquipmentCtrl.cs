using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//���� �������� ���, �Ƹ�, ������ ���� ����ǥ�� �� UI����� �÷��̾� ��������, ���� ���۰� ���õ� WeaponCtrl ��ũ��Ʈ ���� ���� ���
//[Drag&DropPanel] ������ ���� "EquipmentPanel"�� ���� ������Ʈ�� (HeadGear, Armour, Weapon)�� �ٿ��� ����

public class EquipmentCtrl : MonoBehaviour
{
    public SlotCtrl m_slotCtrl = null;   //���� ������Ʈ(HeadGear, Armour, Weapon)�� ����ִ� ����� ������ �������� ���� ����

    // Start is called before the first frame update
    //void Start()
    //{
    //}

    // Update is called once per frame
    //void Update()
    //{
    //}

    //������ ���� �¿��� �Լ�
    public void ItemOnOff()
    {
        switch(System.Enum.Parse(typeof(ItemType), gameObject.name))    //ItemType�� ���� ���ӿ�����Ʈ�� �̸��� ��
        {            
            case ItemType.HeadGear:
                for (int i = 0; i < PlayerCtrl.inst.m_equipEnableHelmets.Length; i++)   //�÷��̾��� ���������� ��� ��Ͽ��� ��
                {
                    if (PlayerCtrl.inst.m_equipEnableHelmets[i].name.Contains(m_slotCtrl.m_itemInfo.m_itName.ToString()))  //�̸� ��ġ�ϸ� ��
                        PlayerCtrl.inst.m_equipEnableHelmets[i].SetActive(true);
                    else   //�ƴ� �͵��� ����
                        PlayerCtrl.inst.m_equipEnableHelmets[i].SetActive(false);                    
                }

                //------ �÷��̾� ����ǥ�ÿ� UI ����κ�
                if (m_slotCtrl.m_itemInfo.m_itType == ItemType.HeadGear)                 //����� �������� ��
                {
                    CanvasCtrl.inst.m_helmetImage.GetComponent<Image>().sprite = m_slotCtrl.m_itemInfo.m_iconImg;
                    CanvasCtrl.inst.m_helmetImage.GetComponent<Image>().color = Color.white;
                }
                else                                                                    //����� �������� �ʾ��� ��
                {
                    CanvasCtrl.inst.m_helmetImage.GetComponent<Image>().sprite = Resources.Load("cross", typeof(Sprite)) as Sprite;
                    CanvasCtrl.inst.m_helmetImage.GetComponent<Image>().color = Color.black;
                }

                CanvasCtrl.inst.m_helmetImage.transform.localScale = m_slotCtrl.m_itemInfo.m_iconSize;  //���س��� �̹��� ������� ����
                break;
                //------ �÷��̾� ����ǥ�ÿ� UI ����κ�

            case ItemType.Armour:
                for (int i = 0; i < PlayerCtrl.inst.m_equipEnableArmours.Length; i++)   //�÷��̾��� ���������� �Ƹ� ��Ͽ��� ��
                {
                    if (PlayerCtrl.inst.m_equipEnableArmours[i].name.Contains(m_slotCtrl.m_itemInfo.m_itName.ToString()))   //�̸� ��ġ�ϸ� ��
                        PlayerCtrl.inst.m_equipEnableArmours[i].SetActive(true);
                    else   //�ƴ� �͵��� ����
                        PlayerCtrl.inst.m_equipEnableArmours[i].SetActive(false);                   
                }

                //------ �÷��̾� ����ǥ�ÿ� UI ����κ�
                if (m_slotCtrl.m_itemInfo.m_itType == ItemType.Armour)                 //�ƸӸ� �������� ��
                {
                    CanvasCtrl.inst.m_armourImage.GetComponent<Image>().sprite = m_slotCtrl.m_itemInfo.m_iconImg;
                    CanvasCtrl.inst.m_armourImage.GetComponent<Image>().color = Color.white;
                }
                else                                                                    //�ƸӸ� �������� �ʾ��� ��
                {
                    CanvasCtrl.inst.m_armourImage.GetComponent<Image>().sprite = Resources.Load("cross", typeof(Sprite)) as Sprite;
                    CanvasCtrl.inst.m_armourImage.GetComponent<Image>().color = Color.black;
                }

                CanvasCtrl.inst.m_armourImage.transform.localScale = m_slotCtrl.m_itemInfo.m_iconSize;
                break;
                //------ �÷��̾� ����ǥ�ÿ� UI ����κ�

            case ItemType.Weapon:
                for (int i = 0; i < PlayerCtrl.inst.m_equipEnableWeapons.Length; i++)   //�÷��̾��� ���������� ���� ��Ͽ��� ��
                {                    
                    if (PlayerCtrl.inst.m_equipEnableWeapons[i].name.Contains(m_slotCtrl.m_itemInfo.m_itName.ToString()))    //�̸� ��ġ�ϸ� ����
                    {
                        PlayerCtrl.inst.m_equipEnableWeapons[i].SetActive(true);     //�ش��ϴ� �÷��̾� ���� ������ ��
                        WeaponCtrl a_playerWeapon = PlayerCtrl.inst.m_equipEnableWeapons[i].GetComponent<WeaponCtrl>();  //�ش��ϴ� ������ WeaponCtrl ����
                        a_playerWeapon.m_itemInfo = m_slotCtrl.m_itemInfo;           //���� ���� ����� ������ ����
                        a_playerWeapon.Init();      //�ʱ⼳�� ����
                    }
                    else
                        PlayerCtrl.inst.m_equipEnableWeapons[i].SetActive(false);   //�̸��� ��ġ���� �ʴ� �͵��� ����
                }

                //------ �÷��̾� ����ǥ�ÿ� UI ����κ�
                if (m_slotCtrl.m_itemInfo.m_itType == ItemType.Weapon)              //������ �������� ��
                {
                    CanvasCtrl.inst.m_weaponImage.GetComponent<Image>().sprite = m_slotCtrl.m_itemInfo.m_iconImg;
                    CanvasCtrl.inst.m_weaponImage.transform.localScale = m_slotCtrl.m_itemInfo.m_iconSize;
                }
                else                                                                //������ �������� �ʾ��� ��
                {
                    CanvasCtrl.inst.m_weaponImage.GetComponent<Image>().sprite = Resources.Load("Weapons/Fist", typeof(Sprite)) as Sprite;
                    CanvasCtrl.inst.m_weaponImage.transform.localScale = new Vector3(0.9f, 0.7f, 1.0f);
                }
                
                break;
                //------ �÷��̾� ����ǥ�ÿ� UI ����κ�
        }
    }
}
