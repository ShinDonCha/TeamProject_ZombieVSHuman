using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//아이템 관련 드래그 앤 드랍 부분을 전반적으로 담당하는 스크립트
//[Drag&DropPanel] 프리팹에서 사용, "Drag&DropPanel"에 붙여서 사용

public class DragDropPanelCtrl : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerEnterHandler
{
    public GameObject m_dragSlot = null;            //활성화된 "DragSlot" 게임오브젝트를 담을 변수    
    public GameObject m_equipmentPanel = null;      //"EquipmentPanel" 게임 오브젝트
    public GameObject m_invenPanel = null;          //"InventoryPanel" 게임 오브젝트
    public GameObject m_lootPanel = null;           //"LootPanel" 게임 오브젝트    

    SlotCtrl m_dragSlotCtrl = null;         //"DragSlot"의 SlotCtrl 스크립트를 담을 변수
    SlotCtrl m_slotCtrl = null;             //OnBeginDrag() 되는 대상의 SlotCtrl 스크립트를 담을 변수
    SlotCtrl m_targetSlotCtrl = null;       //OnDrop() 되는 대상의 SlotCtrl 스크립트를 담을 변수

    //------ 정보보기 창
    [Header("------ Information ------")]
    public GameObject m_information = null; //"Information" 게임 오브젝트
    public Text m_nameText = null;          //장비 이름
    public Text m_statText = null;          //장비 정보
    public Text m_explainText = null;       //장비 설명
    [HideInInspector] public Vector3[] m_rectCorner = new Vector3[4];   //장비의 설명을 보여줄 때 판넬을 넘어가지 않도록 하기위한 위치 조정용 변수
    //------ 정보보기 창

    [Header("----- Prefab -----")]
    public GameObject m_slotObj = null;     //[Slot] 프리팹
    public GameObject m_worldItem = null;   //인게임 상에 새로 생성될 아이템 프리팹

    void Start()
    {
        GetComponent<RectTransform>().GetWorldCorners(m_rectCorner);    //"Drag&DropPanel"의 네 귀퉁이 위치 저장
        m_dragSlotCtrl = m_dragSlot.GetComponent<SlotCtrl>();           //활성화 된 "DragSlot"의 SlotCtrl 스크립트 가져오기
        ListSort();        //플레이어 주위의 아이템리스트 정렬 함수
        SetSlot();         //인벤토리와 바닥에 있는 아이템 슬롯으로 가져오는 함수
    }

    //드래그가 시작됐을 때
    public void OnBeginDrag(PointerEventData eventData)     
    {
        if (eventData.pointerCurrentRaycast.gameObject.tag.Contains("Slot"))       //대상의 태그가 Slot일 경우만 (슬롯이 아닌 부분 클릭했을때 동작하지 않도록)
        {
            m_slotCtrl = eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotCtrl>();       //대상의 SlotCtrl 스크립트 가져오기            

            if (m_slotCtrl.m_itemInfo.m_itType != ItemType.Null)            //대상이 정보를 지니고 있다면
            {
                m_dragSlot.SetActive(true);                                 //"DragSlot" 오브젝트를 켜기
                m_dragSlotCtrl.m_itemInfo = m_slotCtrl.m_itemInfo;          //"DragSlot"의 SlotCtrl에 장비정보를 담기                
                m_dragSlotCtrl.ChangeImg();                                 //"DragSlot"의 이미지 변경 함수
            }
        }           
    }

    //마우스 드래그 중일 때 계속 발생하는 이벤트
    public void OnDrag(PointerEventData eventData)
    {
        if (m_dragSlot.activeSelf == false)        //처음 드래그 대상이 일반슬롯이 아니였다면 중지
            return;

        m_dragSlot.transform.position = eventData.position;   //"DragSlot"의 위치를 마우스의 위치로 변경
    }

    //드롭이 시작됐을 때
    public void OnDrop(PointerEventData eventData)
    {
        if (m_dragSlot.activeSelf == false)       //처음 드래그 대상이 일반슬롯이 아니였다면 중지
            return;       

        if (eventData.pointerCurrentRaycast.gameObject.tag.Contains("Slot"))       //슬롯에 드랍했으면
        {
            m_targetSlotCtrl = eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotCtrl>();     //대상 슬롯의 SlotCtrl 스크립트 가져오기            

            //드롭 대상 슬롯이 "EquipmentPanel"안의 슬롯인데 장착할 아이템과 같은 타입이 아니라면 중지
            if (m_targetSlotCtrl.gameObject.tag.Contains("Equip") &&                                 
                m_targetSlotCtrl.gameObject.name != m_dragSlotCtrl.m_itemInfo.m_itType.ToString())
                return;
           
            m_slotCtrl.m_itemInfo = m_targetSlotCtrl.m_itemInfo;                        //장착하려는 아이템과 기존 장비창 아이템의 정보 교환
            m_targetSlotCtrl.m_itemInfo = m_dragSlotCtrl.m_itemInfo;                    //장착하려는 아이템과 기존 장비창 아이템의 정보 교환

            m_slotCtrl.ChangeImg();                                                     //바뀐 정보대로 이미지 교체
            m_targetSlotCtrl.ChangeImg();                                               //바뀐 정보대로 이미지 교체
            m_slotCtrl.SaveList(m_slotCtrl.transform.parent.gameObject);                //판넬 종류별로 연동된 리스트에 정보 저장
            m_targetSlotCtrl.SaveList(m_targetSlotCtrl.transform.parent.gameObject);    //판넬 종류별로 연동된 리스트에 정보 저장
        }

        ItemSetting();      //현재 아이템들이 들어있는 판넬에 맞게 아이템 설정 변경 함수
    }

    //마우스 드래그가 끝났을 때 발생하는 이벤트
    public void OnEndDrag(PointerEventData eventData)
    {
        if (m_dragSlot.activeSelf == false)    //처음 드래그 대상이 일반슬롯이 아니였다면 중지
            return;

        m_dragSlotCtrl.m_itemInfo = null;     //"DragSlot"의 아이템 정보 초기화
        m_targetSlotCtrl = null;              //담았던 OnDrop 대상의 정보 초기화
        m_slotCtrl = null;                    //담았던 OnBeginDrag 대상의 정보 초기화
        m_dragSlot.SetActive(false);          //"DragSlot" 오브젝트 끄기        
    }       

    //아이템들의 위치에 따라 판넬의 슬롯 세팅
    public void SetSlot()
    {
        Cursor.lockState = CursorLockMode.None;                             //줍는동안 마우스 커서 나타나게 하기

        SlotCtrl[] a_rSlotC = m_lootPanel.GetComponentsInChildren<SlotCtrl>();             //"LootPanel"의 자식 슬롯의 SlotCtrl 스크립트 가져오기
        for (int Lootadd = 0; Lootadd < a_rSlotC.Length; Lootadd++)                        //"LootPanel"의 슬롯 개수만큼 실행
            a_rSlotC[Lootadd].m_itemInfo = PlayerCtrl.inst.m_itemList[Lootadd];

        SlotCtrl[] a_iSlotC = m_invenPanel.GetComponentsInChildren<SlotCtrl>();             //"InventoryPanel"의 자식 슬롯의 SlotCtrl 스크립트 가져오기
        for (int invenadd = 0; invenadd < a_iSlotC.Length; invenadd++)                      //"InventoryPanel"의 슬롯 개수만큼 실행
            a_iSlotC[invenadd].m_itemInfo = GlobalValue.g_userItem[invenadd];

        SlotCtrl[] a_eSlotC = m_equipmentPanel.GetComponentsInChildren<SlotCtrl>();         //"LootPanel"의 자식 슬롯의 SlotCtrl 스크립트 가져오기
        for (int equippedadd = 0; equippedadd < a_eSlotC.Length; equippedadd++)             //"LootPanel"의 슬롯 개수만큼 실행
            a_eSlotC[equippedadd].m_itemInfo = GlobalValue.g_equippedItem[equippedadd];
    }


    //현재 아이템들이 들어있는 판넬에 맞게 아이템 설정 변경
    public void ItemSetting()
    {
        //LootPanel에서 Inventory Panel로 정보가 옮겨진 아이템의 월드맵 상 외형 삭제
        ItemCtrl[] a_items = FieldCollector.inst.m_ItemColl.GetComponentsInChildren<ItemCtrl>();
        for (int i = 0; i < a_items.Length; i++)
        {
            if (a_items[i].m_itemInfo.m_isDropped == false)
                Destroy(a_items[i].gameObject);
        }

        //Inventory Panel에서 LootPanel로 정보가 옮겨진 아이템의 외형 생성
        for (int WorlditCount = 0; WorlditCount < PlayerCtrl.inst.m_itemList.Count; WorlditCount++)
        {
            if (PlayerCtrl.inst.m_itemList[WorlditCount].m_itType == ItemType.Null)         //빈 슬롯이면 외형생성 안함
                continue;

            if (PlayerCtrl.inst.m_itemList[WorlditCount].m_isDropped == false)      //정보가 옮겨진 아이템의 외형 생성
            {
                PlayerCtrl.inst.m_itemList[WorlditCount].m_isDropped = true;        //아이템이 맵 상에 떨어져있는 상태로 변경

                int a_rndX = Random.Range(-1, 2);
                int a_rndZ = Random.Range(-1, 2);

                if (a_rndX == 0 && a_rndZ == 0)      //플레이어와 겹치는 것을 방지
                    a_rndX = 1;

                GameObject a_Item = Instantiate(m_worldItem);                       //아이템 프리팹 맵에 생성
                a_Item.transform.position = PlayerCtrl.inst.transform.position + new Vector3(a_rndX, 1, a_rndZ);  //생성위치를 플레이어 주위의 랜덤값 위치로 함
                a_Item.transform.SetParent(FieldCollector.inst.m_ItemColl.transform, false);                      //필드 아이템 모음의 차일드로 넣기
                a_Item.GetComponent<ItemCtrl>().m_itemInfo = PlayerCtrl.inst.m_itemList[WorlditCount];            //생성된 아이템의 정보를 리스트와 일치하게 변경                              
            }
        }

        SoundMgr.inst.SoundPlay(SoundList.Change);         //장비 변경 사운드 재생
    }


    //플레이어 주위 아이템(맵에 드랍된 아이템)리스트 정렬. (중간에 비어있는 슬롯이 없도록 앞으로 모아주는 용도)
    void ListSort()                             
    {
        bool a_listEnd = false;     //더이상 찾지 않아도 됨 

        for (int i = 0; i < PlayerCtrl.inst.m_itemList.Count; i++)      //근처 바닥에 드랍된 아이템 개수만큼 실행
        {
            if (a_listEnd == true)
                break;

            if (PlayerCtrl.inst.m_itemList[i].m_itType == ItemType.Null)                    //아이템 정보가 없는 슬롯(비어있는 슬롯)일 경우
                for (int j = i + 1; j < PlayerCtrl.inst.m_itemList.Count; j++)              //하나 뒤에서부터 끝까지 리스트목록 조사
                    if (PlayerCtrl.inst.m_itemList[j].m_itType != ItemType.Null)            //비어있지 않은 슬롯을 찾으면 정보 교체
                    {
                        a_listEnd = false;                                                  //정렬이 끝나지 않았음
                        ItemInfo a_itemInfo = PlayerCtrl.inst.m_itemList[j];                //바꿀 대상의 정보 임시 저장
                        PlayerCtrl.inst.m_itemList[j] = PlayerCtrl.inst.m_itemList[i];      //뒤쪽에서 찾은 슬롯을 비어있는 상태로 정보 변경
                        PlayerCtrl.inst.m_itemList[i] = a_itemInfo;                         //비어있던 슬롯에 정보 넣어주기
                        break;                                                              //한번 실행에 하나씩만 바꾸기
                    }
                    else
                        a_listEnd = true;                                                   //리스트에서 바꿔줄 것을 찾지못하면 정렬 종료

        }
    }


    private void OnDestroy()     //"Drag&DropPanel" 제거 시 동작
    {
        ListSort();         //플레이어 주위 아이템 리스트 정렬

        if (Cursor.lockState == CursorLockMode.None)
            Cursor.lockState = CursorLockMode.Locked;    //마우스커서 다시 잠그기
    }

    //아이템 정보 표시용 이벤트 함수
    public void OnPointerEnter(PointerEventData eventData)  //마우스가 아이템 위에 올라가있다면
    {
        if (m_information.activeSelf == true)
            m_information.SetActive(false);
    }
}
