using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//"Drag&DropPanel"속 하위 판넬들(EquipmentPanel, InventoryPanel, LootPanel)의 하위 슬롯들과 DragSlot의 동작을 담당하는 스크립트
//Slot과 DragSlot에 붙여서 사용

public class SlotCtrl : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    public ItemInfo m_itemInfo = null;     //현재 슬롯의 아이템 정보

    float m_timer = 0.0f;                  //계산용 변수
    float m_equipTime = 2.0f;              //아이템 장착시 필요한 시간

    public GameObject m_itemImg = null;    //아이템이미지를 담을 변수
    public GameObject m_timerImg = null;   //타이머이미지를 담을 변수

    void Start()
    {       
        ChangeImg();        //이미지 변경 함수 실행
    }

    void Update()
    {        
        if (InGameMgr.s_gameState == GameState.GamePaused)
            return;

        if (this.gameObject.CompareTag("DragSlot"))             //드래그 슬롯일 경우 중지
            return;
        
        if (m_timerImg.activeSelf == true && m_itemInfo.m_itType != ItemType.Null)    //마우스 우클릭으로 타이머가 켜졌고, 빈 슬롯이 아닐 경우..
        {
            m_timer += Time.deltaTime;
            m_timerImg.GetComponent<Image>().fillAmount = m_timer / m_equipTime;      //장착 시 소요시간 표시
            if(m_equipTime < m_timer)
            {
                m_timerImg.SetActive(false);
                m_timer = 0.0f;

                if(gameObject.name.Contains("Slot"))          //선택된 슬롯이 "InventoryPanel" 또는 "LootPanel"의 슬롯일 경우
                    Equip();
                else                                          //"EquipmentPanel"의 슬롯일 경우
                    Remove();

                NetworkMgr.inst.PushPacket(PacketType.ItemChange);     //아이템 변경 패킷 요청
            }
        }
    }

    public void ChangeImg()     //바뀐 슬롯의 정보에 맞게 아이템 이미지 교체해주는 함수
    {
        m_itemImg.GetComponent<Image>().sprite = m_itemInfo.m_iconImg;
        m_itemImg.transform.localScale = (Vector3)m_itemInfo.m_iconSize;
    }

    void Equip()    //"EquipmentPanel"에 장착
    {
        DragDropPanelCtrl a_DDCtrl = CanvasCtrl.inst.gameObject.GetComponentInChildren<DragDropPanelCtrl>();
        GameObject a_GO = a_DDCtrl.m_equipmentPanel;                        //"EquipmentPanel" 게임오브젝트를 담은 변수 가져오기
        SlotCtrl a_SC = a_GO.transform.GetChild((int)m_itemInfo.m_itType).GetComponent<SlotCtrl>(); //현재 슬롯의 아이템타입과 일치하는 "EquipmentPanel" 슬롯의 SlotCtrl 가져오기
        a_SC.m_itemInfo = m_itemInfo;                                       //"EquipmentPanel" 슬롯의 정보 바꾸기
        a_SC.ChangeImg();                                                   //"EquipmentPanel" 슬롯의 이미지 바꾸기
        m_itemInfo = GlobalValue.g_equippedItem[(int)m_itemInfo.m_itType];  //현재 선택된 슬롯의 정보 바꾸기
        ChangeImg();                                                        //현재 선택된 슬롯의 이미지 바꾸기
        SaveList(a_GO);                                                     //장착된 장비의 정보 저장
        SaveList(transform.parent.gameObject);                              //장착해제된 장비의 정보 저장
        a_DDCtrl.ItemSetting();                                             //현재 아이템들이 들어있는 판넬에 맞게 아이템 설정 변경
    }

    void Remove()   //"EquipmentPanel"에서 제거
    {
        DragDropPanelCtrl a_DDCtrl = CanvasCtrl.inst.gameObject.GetComponentInChildren<DragDropPanelCtrl>();
        GameObject a_GO = a_DDCtrl.m_invenPanel;                            //"InventoryPanel" 게임오브젝트를 담은 변수 가져오기

        for (int i = 0; i < GlobalValue.g_invenFullSlotCount; i++)          //"InventoryPanel" 슬롯의 최대개수 만큼 실행
        {
            if (GlobalValue.g_userItem[i].m_itType == ItemType.Null)        //"InventoryPanel" 슬롯중에 빈 슬롯에만 현재 장착아이템 저장하기
            {
                SlotCtrl a_SC = a_GO.transform.GetChild(i).GetComponent<SlotCtrl>();
                a_SC.m_itemInfo = m_itemInfo;                               //비어있는 "InventoryPanel" 슬롯에 현재 슬롯 정보 저장
                a_SC.ChangeImg();                                           //이미지 바꾸기
                m_itemInfo = GlobalValue.g_userItem[i];                     //현재 선택된 슬롯의 정보 바꾸기
                ChangeImg();                                                //현재 선택된 슬롯의 이미지 바꾸기
                SaveList(a_GO);                                             //장착해제된 장비의 정보 저장
                SaveList(transform.parent.gameObject);                      //장착된 장비의 정보 저장                
                break;                                                      //빈 슬롯을 찾았을 경우 한번만 실행
            }
            else
                continue;                                                   //빈 슬롯이 아닐 경우 넘어가기   
        }

        SoundMgr.inst.SoundPlay(SoundList.Change);                          //장비 변경 사운드 재생
    }

    //------ 판넬별 정보 저장할 리스트 찾기
    List<ItemInfo> FindList(GameObject a_Panel)                             
    {
        List<ItemInfo> a_List = new List<ItemInfo>();

        switch (a_Panel.name)
        {
            case "EquipmentPanel":
                a_List = GlobalValue.g_equippedItem;
                break;
            case "InventoryPanel":
                a_List = GlobalValue.g_userItem;
                break;
            case "LootPanel":
                a_List = PlayerCtrl.inst.m_itemList;
                break;
        }
        return a_List;
    }
    //------ 판넬별 정보 저장할 리스트 찾기

    //----- 글로벌 리스트 변수의 정보 변경
    public void SaveList(GameObject a_Panel)
    {        
        for (int a_num = 0; a_num < FindList(a_Panel).Count; a_num++)
        {            
            SlotCtrl a_slotCtrl = a_Panel.transform.GetChild(a_num).GetComponent<SlotCtrl>();

            if (a_Panel.name != "LootPanel")            //인벤토리 판넬과, 장비판넬에 있는 아이템일 경우만..            
                a_slotCtrl.m_itemInfo.m_isDropped = false;           

            FindList(a_Panel)[a_num] = a_slotCtrl.m_itemInfo;
        }

        if (a_Panel.name.Contains("Equip"))         //장비 판넬일 경우
        {
            EquipmentCtrl[] a_equipCtrls = a_Panel.GetComponentsInChildren<EquipmentCtrl>();     //장비 판넬 하위에서 해당 스크립트 모두 찾아온 뒤
            for (int i = 0; i < a_equipCtrls.Length; i++)
            {
                a_equipCtrls[i].ItemOnOff();            //아이템 외형 생성&제거
            }
        }
    }
    //----- 글로벌 리스트 변수의 정보 변경

    //---- 슬롯에서 마우스 오른쪽 클릭으로 장착 이벤트 활성화시켰을 때
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)         //현재 슬롯에 마우스 오른쪽 클릭시..
        {
            m_timerImg.SetActive(true);                                     //타이머 이미지 활성화
        }
    }
    //---- 슬롯에서 마우스 오른쪽 클릭으로 장착 이벤트 활성화시켰을 때

    //---- 마우스를 슬롯에 올려놨을 때 정보표시용
    public void OnPointerEnter(PointerEventData eventData)
    {        
        if (eventData.pointerCurrentRaycast.gameObject.tag.Contains("DragSlot"))        //드래그 슬롯일 경우 취소
            return;

        DragDropPanelCtrl a_DDCtrl = CanvasCtrl.inst.gameObject.GetComponentInChildren<DragDropPanelCtrl>();

        if (m_itemInfo.m_itType == ItemType.Null)               //비어있는 슬롯인 경우 취소
        {
            a_DDCtrl.m_information.SetActive(false);
            return;
        }
        
        a_DDCtrl.m_information.SetActive(true);

        //----- 정보창 위치설정
        Vector3 a_Pos = transform.position;                 //현재 슬롯의 위치정보
        a_Pos.x += 150.0f;                                  //슬롯으로부터 x축으로 150.0f만큼 위치로 정보창의 위치 설정
        a_DDCtrl.m_information.transform.position = a_Pos;
        
        Vector3[] a_informRect = new Vector3[4];                        
        a_DDCtrl.m_information.GetComponent<RectTransform>().GetWorldCorners(a_informRect);     //정보창의 네 귀퉁이 위치를 가져옴

        if (a_informRect[0].y < a_DDCtrl.m_rectCorner[0].y)                      //정보창이 "Drag&DropPanel"보다 아래에 있을 때
            a_Pos.y += (a_DDCtrl.m_rectCorner[0].y - a_informRect[0].y);         //차이만큼 정보창을 위로 올려줌

        else if (a_DDCtrl.m_rectCorner[1].y < a_informRect[1].y)                 //정보창이 "Drag&DropPanel"보다 위에 있을 때
            a_Pos.y -= (a_informRect[1].y- a_DDCtrl.m_rectCorner[1].y);          //차이만큼 정보창을 아래로 내려줌

        if (a_DDCtrl.m_rectCorner[2].x < a_informRect[2].x)                      //정보창이 "Drag&DropPanel"보다 오른쪽에 있을 때
            a_Pos.x -= (a_informRect[2].x - a_DDCtrl.m_rectCorner[2].x);         //차이만큼 정보창을 왼쪽으로 옮김

        a_DDCtrl.m_information.transform.position = a_Pos;
        //----- 정보창 위치설정

        //----- 정보창 내용설정
        a_DDCtrl.m_nameText.text = m_itemInfo.m_name;

        if(m_itemInfo.m_itType == ItemType.Weapon)                              //무기일 경우
            if (m_itemInfo.m_itName == ItemName.Bat)
                a_DDCtrl.m_statText.text = string.Format("<color=#FF0000>대미지 : </color> {0}\n<color=#FF0000>내구도 : </color> 00\n" +
                                                        "<color=#FF0000>공격 딜레이 : </color> {1}초",
                                                          m_itemInfo.m_damage, m_itemInfo.m_attackDelay);
            else
                a_DDCtrl.m_statText.text = string.Format("<color=#FF0000>대미지 : </color> {0}\n<color=#FF0000>장전된 총알 : </color> {1}\n" +
                                                        "<color=#FF0000>남은 총알 : </color> {2}\n<color=#FF0000>공격 딜레이 : </color> {3}초",
                                                          m_itemInfo.m_damage, m_itemInfo.m_curMagazine, m_itemInfo.m_maxMagazine, m_itemInfo.m_attackDelay);
        else                                                                    //방어구일 경우
            a_DDCtrl.m_statText.text = string.Format("<color=#FF0000>남은 내구도 : </color> {0}",
                                                          m_itemInfo.m_curMagazine);

        a_DDCtrl.m_explainText.text = m_itemInfo.m_itemEx;          //아이템 설명 적용
        //----- 정보창 내용설정
    }
    //---- 마우스를 슬롯에 올려놨을 때 정보표시용
}