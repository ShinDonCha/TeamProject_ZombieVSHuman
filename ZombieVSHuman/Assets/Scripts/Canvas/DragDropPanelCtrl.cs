using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//������ ���� �巡�� �� ��� �κ��� ���������� ����ϴ� ��ũ��Ʈ
//[Drag&DropPanel] �����տ��� ���, "Drag&DropPanel"�� �ٿ��� ���

public class DragDropPanelCtrl : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerEnterHandler
{
    public GameObject m_dragSlot = null;            //Ȱ��ȭ�� "DragSlot" ���ӿ�����Ʈ�� ���� ����    
    public GameObject m_equipmentPanel = null;      //"EquipmentPanel" ���� ������Ʈ
    public GameObject m_invenPanel = null;          //"InventoryPanel" ���� ������Ʈ
    public GameObject m_lootPanel = null;           //"LootPanel" ���� ������Ʈ    

    SlotCtrl m_dragSlotCtrl = null;         //"DragSlot"�� SlotCtrl ��ũ��Ʈ�� ���� ����
    SlotCtrl m_slotCtrl = null;             //OnBeginDrag() �Ǵ� ����� SlotCtrl ��ũ��Ʈ�� ���� ����
    SlotCtrl m_targetSlotCtrl = null;       //OnDrop() �Ǵ� ����� SlotCtrl ��ũ��Ʈ�� ���� ����

    //------ �������� â
    [Header("------ Information ------")]
    public GameObject m_information = null; //"Information" ���� ������Ʈ
    public Text m_nameText = null;          //��� �̸�
    public Text m_statText = null;          //��� ����
    public Text m_explainText = null;       //��� ����
    [HideInInspector] public Vector3[] m_rectCorner = new Vector3[4];   //����� ������ ������ �� �ǳ��� �Ѿ�� �ʵ��� �ϱ����� ��ġ ������ ����
    //------ �������� â

    [Header("----- Prefab -----")]
    public GameObject m_slotObj = null;     //[Slot] ������
    public GameObject m_worldItem = null;   //�ΰ��� �� ���� ������ ������ ������

    void Start()
    {
        GetComponent<RectTransform>().GetWorldCorners(m_rectCorner);    //"Drag&DropPanel"�� �� ������ ��ġ ����
        m_dragSlotCtrl = m_dragSlot.GetComponent<SlotCtrl>();           //Ȱ��ȭ �� "DragSlot"�� SlotCtrl ��ũ��Ʈ ��������
        ListSort();        //�÷��̾� ������ �����۸���Ʈ ���� �Լ�
        SetSlot();         //�κ��丮�� �ٴڿ� �ִ� ������ �������� �������� �Լ�
    }

    //�巡�װ� ���۵��� ��
    public void OnBeginDrag(PointerEventData eventData)     
    {
        if (eventData.pointerCurrentRaycast.gameObject.tag.Contains("Slot"))       //����� �±װ� Slot�� ��츸 (������ �ƴ� �κ� Ŭ�������� �������� �ʵ���)
        {
            m_slotCtrl = eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotCtrl>();       //����� SlotCtrl ��ũ��Ʈ ��������            

            if (m_slotCtrl.m_itemInfo.m_itType != ItemType.Null)            //����� ������ ���ϰ� �ִٸ�
            {
                m_dragSlot.SetActive(true);                                 //"DragSlot" ������Ʈ�� �ѱ�
                m_dragSlotCtrl.m_itemInfo = m_slotCtrl.m_itemInfo;          //"DragSlot"�� SlotCtrl�� ��������� ���                
                m_dragSlotCtrl.ChangeImg();                                 //"DragSlot"�� �̹��� ���� �Լ�
            }
        }           
    }

    //���콺 �巡�� ���� �� ��� �߻��ϴ� �̺�Ʈ
    public void OnDrag(PointerEventData eventData)
    {
        if (m_dragSlot.activeSelf == false)        //ó�� �巡�� ����� �Ϲݽ����� �ƴϿ��ٸ� ����
            return;

        m_dragSlot.transform.position = eventData.position;   //"DragSlot"�� ��ġ�� ���콺�� ��ġ�� ����
    }

    //����� ���۵��� ��
    public void OnDrop(PointerEventData eventData)
    {
        if (m_dragSlot.activeSelf == false)       //ó�� �巡�� ����� �Ϲݽ����� �ƴϿ��ٸ� ����
            return;       

        if (eventData.pointerCurrentRaycast.gameObject.tag.Contains("Slot"))       //���Կ� ���������
        {
            m_targetSlotCtrl = eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotCtrl>();     //��� ������ SlotCtrl ��ũ��Ʈ ��������            

            //��� ��� ������ "EquipmentPanel"���� �����ε� ������ �����۰� ���� Ÿ���� �ƴ϶�� ����
            if (m_targetSlotCtrl.gameObject.tag.Contains("Equip") &&                                 
                m_targetSlotCtrl.gameObject.name != m_dragSlotCtrl.m_itemInfo.m_itType.ToString())
                return;
           
            m_slotCtrl.m_itemInfo = m_targetSlotCtrl.m_itemInfo;                        //�����Ϸ��� �����۰� ���� ���â �������� ���� ��ȯ
            m_targetSlotCtrl.m_itemInfo = m_dragSlotCtrl.m_itemInfo;                    //�����Ϸ��� �����۰� ���� ���â �������� ���� ��ȯ

            m_slotCtrl.ChangeImg();                                                     //�ٲ� ������� �̹��� ��ü
            m_targetSlotCtrl.ChangeImg();                                               //�ٲ� ������� �̹��� ��ü
            m_slotCtrl.SaveList(m_slotCtrl.transform.parent.gameObject);                //�ǳ� �������� ������ ����Ʈ�� ���� ����
            m_targetSlotCtrl.SaveList(m_targetSlotCtrl.transform.parent.gameObject);    //�ǳ� �������� ������ ����Ʈ�� ���� ����
        }

        ItemSetting();      //���� �����۵��� ����ִ� �ǳڿ� �°� ������ ���� ���� �Լ�
    }

    //���콺 �巡�װ� ������ �� �߻��ϴ� �̺�Ʈ
    public void OnEndDrag(PointerEventData eventData)
    {
        if (m_dragSlot.activeSelf == false)    //ó�� �巡�� ����� �Ϲݽ����� �ƴϿ��ٸ� ����
            return;

        m_dragSlotCtrl.m_itemInfo = null;     //"DragSlot"�� ������ ���� �ʱ�ȭ
        m_targetSlotCtrl = null;              //��Ҵ� OnDrop ����� ���� �ʱ�ȭ
        m_slotCtrl = null;                    //��Ҵ� OnBeginDrag ����� ���� �ʱ�ȭ
        m_dragSlot.SetActive(false);          //"DragSlot" ������Ʈ ����        
    }       

    //�����۵��� ��ġ�� ���� �ǳ��� ���� ����
    public void SetSlot()
    {
        Cursor.lockState = CursorLockMode.None;                             //�ݴµ��� ���콺 Ŀ�� ��Ÿ���� �ϱ�

        SlotCtrl[] a_rSlotC = m_lootPanel.GetComponentsInChildren<SlotCtrl>();             //"LootPanel"�� �ڽ� ������ SlotCtrl ��ũ��Ʈ ��������
        for (int Lootadd = 0; Lootadd < a_rSlotC.Length; Lootadd++)                        //"LootPanel"�� ���� ������ŭ ����
            a_rSlotC[Lootadd].m_itemInfo = PlayerCtrl.inst.m_itemList[Lootadd];

        SlotCtrl[] a_iSlotC = m_invenPanel.GetComponentsInChildren<SlotCtrl>();             //"InventoryPanel"�� �ڽ� ������ SlotCtrl ��ũ��Ʈ ��������
        for (int invenadd = 0; invenadd < a_iSlotC.Length; invenadd++)                      //"InventoryPanel"�� ���� ������ŭ ����
            a_iSlotC[invenadd].m_itemInfo = GlobalValue.g_userItem[invenadd];

        SlotCtrl[] a_eSlotC = m_equipmentPanel.GetComponentsInChildren<SlotCtrl>();         //"LootPanel"�� �ڽ� ������ SlotCtrl ��ũ��Ʈ ��������
        for (int equippedadd = 0; equippedadd < a_eSlotC.Length; equippedadd++)             //"LootPanel"�� ���� ������ŭ ����
            a_eSlotC[equippedadd].m_itemInfo = GlobalValue.g_equippedItem[equippedadd];
    }


    //���� �����۵��� ����ִ� �ǳڿ� �°� ������ ���� ����
    public void ItemSetting()
    {
        //LootPanel���� Inventory Panel�� ������ �Ű��� �������� ����� �� ���� ����
        ItemCtrl[] a_items = FieldCollector.inst.m_ItemColl.GetComponentsInChildren<ItemCtrl>();
        for (int i = 0; i < a_items.Length; i++)
        {
            if (a_items[i].m_itemInfo.m_isDropped == false)
                Destroy(a_items[i].gameObject);
        }

        //Inventory Panel���� LootPanel�� ������ �Ű��� �������� ���� ����
        for (int WorlditCount = 0; WorlditCount < PlayerCtrl.inst.m_itemList.Count; WorlditCount++)
        {
            if (PlayerCtrl.inst.m_itemList[WorlditCount].m_itType == ItemType.Null)         //�� �����̸� �������� ����
                continue;

            if (PlayerCtrl.inst.m_itemList[WorlditCount].m_isDropped == false)      //������ �Ű��� �������� ���� ����
            {
                PlayerCtrl.inst.m_itemList[WorlditCount].m_isDropped = true;        //�������� �� �� �������ִ� ���·� ����

                int a_rndX = Random.Range(-1, 2);
                int a_rndZ = Random.Range(-1, 2);

                if (a_rndX == 0 && a_rndZ == 0)      //�÷��̾�� ��ġ�� ���� ����
                    a_rndX = 1;

                GameObject a_Item = Instantiate(m_worldItem);                       //������ ������ �ʿ� ����
                a_Item.transform.position = PlayerCtrl.inst.transform.position + new Vector3(a_rndX, 1, a_rndZ);  //������ġ�� �÷��̾� ������ ������ ��ġ�� ��
                a_Item.transform.SetParent(FieldCollector.inst.m_ItemColl.transform, false);                      //�ʵ� ������ ������ ���ϵ�� �ֱ�
                a_Item.GetComponent<ItemCtrl>().m_itemInfo = PlayerCtrl.inst.m_itemList[WorlditCount];            //������ �������� ������ ����Ʈ�� ��ġ�ϰ� ����                              
            }
        }

        SoundMgr.inst.SoundPlay(SoundList.Change);         //��� ���� ���� ���
    }


    //�÷��̾� ���� ������(�ʿ� ����� ������)����Ʈ ����. (�߰��� ����ִ� ������ ������ ������ ����ִ� �뵵)
    void ListSort()                             
    {
        bool a_listEnd = false;     //���̻� ã�� �ʾƵ� �� 

        for (int i = 0; i < PlayerCtrl.inst.m_itemList.Count; i++)      //��ó �ٴڿ� ����� ������ ������ŭ ����
        {
            if (a_listEnd == true)
                break;

            if (PlayerCtrl.inst.m_itemList[i].m_itType == ItemType.Null)                    //������ ������ ���� ����(����ִ� ����)�� ���
                for (int j = i + 1; j < PlayerCtrl.inst.m_itemList.Count; j++)              //�ϳ� �ڿ������� ������ ����Ʈ��� ����
                    if (PlayerCtrl.inst.m_itemList[j].m_itType != ItemType.Null)            //������� ���� ������ ã���� ���� ��ü
                    {
                        a_listEnd = false;                                                  //������ ������ �ʾ���
                        ItemInfo a_itemInfo = PlayerCtrl.inst.m_itemList[j];                //�ٲ� ����� ���� �ӽ� ����
                        PlayerCtrl.inst.m_itemList[j] = PlayerCtrl.inst.m_itemList[i];      //���ʿ��� ã�� ������ ����ִ� ���·� ���� ����
                        PlayerCtrl.inst.m_itemList[i] = a_itemInfo;                         //����ִ� ���Կ� ���� �־��ֱ�
                        break;                                                              //�ѹ� ���࿡ �ϳ����� �ٲٱ�
                    }
                    else
                        a_listEnd = true;                                                   //����Ʈ���� �ٲ��� ���� ã�����ϸ� ���� ����

        }
    }


    private void OnDestroy()     //"Drag&DropPanel" ���� �� ����
    {
        ListSort();         //�÷��̾� ���� ������ ����Ʈ ����

        if (Cursor.lockState == CursorLockMode.None)
            Cursor.lockState = CursorLockMode.Locked;    //���콺Ŀ�� �ٽ� ��ױ�
    }

    //������ ���� ǥ�ÿ� �̺�Ʈ �Լ�
    public void OnPointerEnter(PointerEventData eventData)  //���콺�� ������ ���� �ö��ִٸ�
    {
        if (m_information.activeSelf == true)
            m_information.SetActive(false);
    }
}
