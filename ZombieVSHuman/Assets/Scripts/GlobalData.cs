using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//아이템의 종류와 설정값을 정하고, 글로벌 변수들을 모아놓은 스크립트

public enum ItemType
{
    HeadGear,
    Armour,
    Weapon,
    Count,
    Null
}

public enum ItemName
{
    Kick,
    Bat,
    M16,
    K2,
    AK47,
    SniperRifle,
    Armour,
    Helmet,
    ItemCount,
}

public class ItemInfo //각 Item 정보
{
    public ItemName m_itName = ItemName.Kick;   //아이템 이름
    public int m_curMagazine = 0;               //현재 장전되어 있는 총알개수
    public int m_maxMagazine = 0;               //최대 총알개수

    public bool m_isDropped = false;

    public string m_name = "";                  //아이템 이름
    public int m_damage = 10;                   //아이템 공격력   
    
    public int m_reMagazine = 0;                //재장전 총알 개수
    
    public float m_expandScale = 0.0f;          //발당 늘어나는 십자선 크기
    public float m_maxScale = 0.0f;             //십자선 최대 크기
    public float m_shrinkSpeed = 0.0f;          //십자선 줄어드는 속도
    public float m_attackDelay = 1.8f;          //공격 딜레이
   
    public ItemType m_itType = ItemType.Null;   //아이템 타입
    public Sprite m_iconImg = null;             //아이템에 사용될 이미지
    public Vector2 m_iconSize = Vector2.one;    //아이템 이미지의 가로 사이즈, 세로 사이즈
    public string m_itemEx = "";                //아이템 설명
    
    public void SetType(ItemName itemName)
    {
        m_itName = itemName;
        if (itemName == ItemName.Bat)
        {
            m_name = "Baseball Bat";
            m_itType = ItemType.Weapon;
            m_iconSize.x = 1.0f;   
            m_iconSize.y = 1.0f;   
            m_curMagazine = 100;
            m_reMagazine = 0;
            m_maxMagazine = 100;
            m_damage = 20;            
            m_attackDelay = 2.0f;          
            m_isDropped = false;
            m_itemEx = "쓰임에 따라 스포츠가 될지 , 느와르물이 될지 정해진다. 지금은 후자일지도..";

            m_iconImg = Resources.Load("Weapons/Bat", typeof(Sprite)) as Sprite;
        }
        else if (itemName == ItemName.M16)
        {
            m_name = "M16";
            m_itType = ItemType.Weapon;
            m_iconSize.x = 0.95f;  
            m_iconSize.y = 0.7f;  
            m_damage = 30;
            m_curMagazine = 20;
            m_reMagazine = 20;
            m_maxMagazine = 200;
            m_expandScale = 0.12f;         
            m_maxScale = 2.0f;            
            m_shrinkSpeed = 0.18f;        
            m_attackDelay = 0.1f;          
            m_isDropped = false;
            m_itemEx = "예비군 아저씨들의 상징과도 같은 무기, 명중률.. 생각보다 뛰어나다!";

            m_iconImg = Resources.Load("Weapons/M16", typeof(Sprite)) as Sprite;
        }
        else if (itemName == ItemName.K2)
        {
            m_name = "K2";
            m_itType = ItemType.Weapon;
            m_iconSize.x = 1.0f;  
            m_iconSize.y = 0.7f;  
            m_damage = 30;
            m_curMagazine = 30;
            m_reMagazine = 30;
            m_maxMagazine = 200;
            m_expandScale = 0.15f;         
            m_maxScale = 2.0f;            
            m_shrinkSpeed = 0.15f;         
            m_attackDelay = 0.1f;          
            m_isDropped = false;
            m_itemEx = "현역들의 상징이자 못다루는 한국남자가 없을 정도, 근데 목숨처럼 소중한 이 무기가 왜 떨어져있지?";

            m_iconImg = Resources.Load("Weapons/K2", typeof(Sprite)) as Sprite;            
        }
        else if (itemName == ItemName.AK47)
        {
            m_name = "AK47";
            m_itType = ItemType.Weapon;
            m_iconSize.x = 1.0f; 
            m_iconSize.y = 0.8f; 
            m_damage = 40;
            m_curMagazine = 30;
            m_reMagazine = 30;
            m_maxMagazine = 200;
            m_expandScale = 0.28f;         
            m_maxScale = 3.0f;            
            m_shrinkSpeed = 0.25f;         
            m_attackDelay = 0.15f;          
            m_isDropped = false;
            m_itemEx = "이제는 총기소유를 할 수 있는 나라에서는 아무나 볼 수 있는 무기, 근데 여긴 한국인데?";

            m_iconImg = Resources.Load("Weapons/AK47", typeof(Sprite)) as Sprite;
        }
        else if (itemName == ItemName.SniperRifle)
        {
            m_name = "Sniper Rifle";
            m_itType = ItemType.Weapon;
            m_iconSize.x = 1.0f;  
            m_iconSize.y = 0.4f;  
            m_damage = 80;
            m_curMagazine = 1;
            m_reMagazine = 1;
            m_maxMagazine = 15;
            m_expandScale = 0.28f;         
            m_maxScale = 3.0f;           
            m_shrinkSpeed = 0.25f;        
            m_attackDelay = 3.0f;         
            m_isDropped = false;
            m_itemEx = "원샷 원킬? 생각보다 어려울껄";

            m_iconImg = Resources.Load("Weapons/Sniper_rifle", typeof(Sprite)) as Sprite;
        }
        else if (itemName == ItemName.Kick)
        {
            m_name = "";
            m_itType = ItemType.Null;
            m_iconSize.x = 1.0f;
            m_iconSize.y = 1.0f;
            m_damage = 10;
            m_curMagazine = 0;
            m_reMagazine = 0;
            m_maxMagazine = 0;
            m_expandScale = 0.0f;         
            m_maxScale = 0.0f;           
            m_shrinkSpeed = 0.0f;        
            m_attackDelay = 1.8f;       
            m_isDropped = false;
            m_itemEx = "";

            //m_audioClip = null;
            m_iconImg = null;
        }
        else if (itemName == ItemName.Armour)
        {
            m_name = "Armour";
            m_itType = ItemType.Armour;
            m_iconSize.x = 1.0f;     
            m_iconSize.y = 1.0f;     
            m_curMagazine = 30;
            m_maxMagazine = 30;
            m_isDropped = false;

            m_itemEx = "군인들이 사용하던 방탄복. 입으면 좀비들에게 살살 맞을 지도?";
            m_iconImg = Resources.Load("Armours/Armour", typeof(Sprite)) as Sprite;
        }
        else if (itemName == ItemName.Helmet)
        {
            m_name = "Helmet";
            m_itType = ItemType.HeadGear;
            m_iconSize.x = 0.9f;    
            m_iconSize.y = 0.9f;     
            m_curMagazine = 15;
            m_maxMagazine = 15;
            m_isDropped = false;

            m_itemEx = "헬멧안에서 땀냄새가 좀 나지만, 안쓰는거 보단 나을 껄";
            m_iconImg = Resources.Load("Helmets/Helmet", typeof(Sprite)) as Sprite;
        }
    }
}

public class GlobalValue
{
    public static string g_Unique_ID = "";              //유저의 고유아이디
    public static string g_NickName = "";               //유저의 별명
    public static bool g_StandUpAnim = false;           //일어나는 애니메이션을 재생해야하는지(신규 유저만 재생)
    public static Vector3 g_CharPos = Vector3.zero;     //저장된 플레이어의 위치를 받아올 변수

    public static List<ItemInfo> g_equippedItem = new List<ItemInfo>();     //플레이어가 장착하고 있는 아이템 목록
    public static List<ItemInfo> g_userItem = new List<ItemInfo>();         //플레이어가 소유하고 있는 아이템 목록
    public static int g_invenFullSlotCount = 12;          //InvenPanel에 넣어줄 슬롯 개수        
    public static int g_equipFullSlotCount = 3;           //equipPanel에 넣어줄 슬롯 개수

    //----- 환경설정 정보저장
    public static Sprite g_cfBGImg = null;                //배경음 이미지
    public static float g_cfBGValue = 1.0f;               //배경음 볼륨 값
    public static Sprite g_cfEffImg = null;               //효과음 이미지
    public static float g_cfEffValue = 1.0f;              //효과음 볼륨 값
    //----- 환경설정 정보저장

    public static void InitData()
    {
        for (int invenadd = 0; invenadd < g_invenFullSlotCount; invenadd++)         //인벤토리 슬롯의 개수만큼 리스트 생성
        {
            ItemInfo a_ItemList = new ItemInfo();
            g_userItem.Add(a_ItemList);
        }             
        
        for (int equipadd = 0; equipadd < 3; equipadd++)    //장비개수(헬멧, 아머, 웨폰)의 개수만큼 리스트 생성
        {
            ItemInfo a_ItemList = new ItemInfo();            
            g_equippedItem.Add(a_ItemList);
        }
    }//public static void InitData()

}

