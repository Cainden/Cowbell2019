using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySpace;
using MySpace.Stats;

//A Manager of game-loop changing functionality between the tycoon-style daytime gameplay and the nighttime gameplay.

public class GameManager : MonoBehaviour
{
    #region Serialized Variables
    [SerializeField] DebugToolsScript DebugMenu;

    [SerializeField] Roles roleInfo;
    private static Roles _roleInfo;

    [SerializeField] int startingHoots = 1000;

    [SerializeField] WorkerConstructionData[] workersToMake;
    #endregion

    //Might not want to do this for the gamemanager?
    #region Singleton Management

    public static GameManager Ref { get; set; }

    private void Awake()
    {
        if (!Ref)
        {
            Ref = this;

            //We don't want this for the gamemanager
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (Ref != this)
            {
                Destroy(gameObject);
            }
        }
    }
    #endregion

    private void Start()
    {
        //TimeManager.AddEventTriggerInSeconds(20, GiveGuest);
        TimeManager.AddEventTriggerToGameTime(9, 0, 0, CreateBasicGuest, true);
        DebugMenu.SetPanelActive(false);
        _roleInfo = roleInfo;

        //Might need to change this if loading a save
        WalletManager.SetHoots(1000);
        foreach (WorkerConstructionData d in workersToMake)
        {
            ClickManager.Ref.AddNewCleaner(d);
        }
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            AppManager.Ref.ChangeApplicationState(Enums.AppState.MainMenu);
        }
        //if (Input.GetKeyDown(KeyCode.BackQuote))
        //{
        //    DebugMenu.SetPanelActive(!DebugMenu.debugElementsUI);
        //}
    }

    private void CreateBasicGuest()
    {
        ClickManager.Ref.Button_Book(CreateDefaultGuest());
        //ClickManager.Ref.AddNewGuest();
    }

    public static GuestConstructionData CreateDefaultGuest()
    {
        return new GuestConstructionData()
        {
            manFirstName = NameFactory.GetNewFirstName(),
            manLastName = NameFactory.GetNewLastName(),
            manId = System.Guid.NewGuid(),
            manType = Enums.ManTypes.Guest,
            generalStats = new GeneralStat[1]
            {
                new GeneralStat()
                {
                    statType = GeneralStat.StatType.Dirtiness,
                    value = 1,
                }
            }
        };
    }

    public static RoleInfo GetRoleInfo(Enums.ManRole role)
    {
        switch (role)
        {
            case MySpace.Enums.ManRole.Cleaner:
                return _roleInfo.Cleaner;
            default:
                return new RoleInfo();
        }
    }

    #region Net Revenue Stuff
    //Can do more stuff with this later to access where the revenue came from or went to using the revenueinfo.

    //Can just use the average profit per day as the return value for any rooms that pay based on guest actions outside of a regular time interval, like casino's or gift shops, etc.

    public delegate void NetRevenueDelegate(List<RevenueInfo> list);
    public static NetRevenueDelegate NetRevenueCalculationEvent;

    public static int CalculateNetRevenue()
    {
        return (int)CalculateHardNetRevenue();
    }

    public static float CalculateHardNetRevenue()
    {
        List<RevenueInfo> list = new List<RevenueInfo>();
        NetRevenueCalculationEvent?.Invoke(list);
        float f = 0;
        foreach (RevenueInfo i in list)
        {
            f += i.effect;
        }
        return f;
    }
    #endregion
}

namespace MySpace
{
    [System.Serializable]
    public struct Roles
    {
        public RoleInfo Cleaner;
    }

    [System.Serializable]
    public struct RoleInfo
    {
        [Tooltip("Paid Daily")]
        public int income;
    }

    public struct RevenueInfo
    {
        public enum RevenueType : byte { Worker, Guest, Room }

        public float effect;
        public RevenueType revenueType;
        public System.Guid objectId;
        public bool estimated;

        public RevenueInfo(float value, RevenueType type, System.Guid id)
        {
            effect = value;
            revenueType = type;
            objectId = id;
            estimated = false;
        }
    }
}

