using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySpace;
using MySpace.Stats;
using Unity.Mathematics;

//A Manager of game-loop changing functionality between the tycoon-style daytime gameplay and the nighttime gameplay.

public class GameManager : MonoBehaviour
{
    #region Serialized Variables
    [Header("Guest Stay Time")]
    public int guestMaxStayTimeDays;
    public int guestMinStayTimeDays;

    [Header("===================================================================================================================================================================================")]
    [SerializeField] DebugToolsScript DebugMenu;

    [SerializeField] RoleInfo[] roles;
    //private static Roles _roleInfo;
    private static Dictionary<Enums.ManRole, RoleInfo> roleDic = new Dictionary<Enums.ManRole, RoleInfo>();

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
        MySpace.Events.EventManager.AddEventTriggerToGameTime(9, 0, 0, CreateBasicGuest, true);
        MySpace.Events.EventManager.AddEventTriggerToGameTime(23, 59, 0, InitiateEndOfDay, true);
        DebugMenu.SetPanelActive(false);

        foreach (RoleInfo r in roles)
        {
            roleDic.Add(r.role, 
                new RoleInfo()
                {
                    incomeMinimum = Mathf.Round(r.incomeMinimum),
                    incomeMaximum = Mathf.Round(r.incomeMaximum),
                    role = r.role
                });
        }

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
    }

    #region
    /// <summary>
    /// Returns a random number calculated with weight based on the standard deviation off of average given.
    /// </summary>
    /// <param name="avg">The average value. If a random number of 0.5 from 0 to 1 is chosen, this function will return the average.</param>
    /// <param name="standDev">The standard deviation of the number distribution</param>
    /// <param name="accuracy">An increased number of accurace increases the accuracy of the iterative process. An accuracy of 2 will loop 100 times, an accuracy of 4 will loop 1000 times. Any accuracy above 15 will treat the accuracy as 15.</param>
    /// <returns></returns>
    public static float GetApproximatedRandomValue(float avg, float standDev, int accuracy = 4)
    {
        //Make sure we don't overload the computer
        if (accuracy > 15)
            accuracy = 15;
        //To make sure that it's accurate at least to some degree
        else if (accuracy < 1)
            accuracy = 1;

        float r = UnityEngine.Random.Range(0f, 1f);
        float total = standDev * 6, increment = total / Mathf.Pow(10, accuracy);
        float result = 0;

        //f1 doesn't use the iteration for each loop so it doesnt need to be calculated more than once.
        float f1 = 1f / (Mathf.Sqrt(2 * Mathf.PI) * Mathf.Sqrt(standDev));

        for (float i = (total * -0.5f) + avg; i < (total * 0.5f) + avg; i += increment)
        {
            float f2 = Mathf.Pow(math.E, -(Mathf.Pow(i - avg, 2) / (2 * standDev)));

            result += f1 * f2 * increment;
            if (result >= r)
            {
                return i;
            }
        }

        //if for some reason it fails, just return the average
        return avg;
    }

    #endregion

    private void CreateBasicGuest()
    {
        ClickManager.Ref.Button_Book(CreateDefaultGuest());
        //ClickManager.Ref.AddNewGuest();
    }

    public void InitiateEndOfDay()
    {
        GuiManager.Ref.DailySummaryPanel.Enable("No Name", 0.95f);
    }

    public static GuestConstructionData CreateDefaultGuest()
    {
        return new GuestConstructionData()
        {
            manFirstName = NameFactory.GetNewFirstName(),
            manLastName = NameFactory.GetNewLastName(),
            manId = System.Guid.NewGuid(),
            manType = Enums.ManTypes.Guest,
            generalStats = new GeneralStat[2]
            {
                new GeneralStat()
                {
                    statType = GeneralStat.StatType.Dirtiness,
                    value = 1
                },
                new GeneralStat()
                {
                    statType = GeneralStat.StatType.Speed,
                    value = 1
                }
            }
        };
    }

    public static RoleInfo GetRoleInfo(Enums.ManRole role)
    {
        return roleDic[role];
    }

    public static int GetRoleSalary(Enums.ManRole role, float loyalty)
    {
        float sMin = roleDic[role].incomeMinimum, sMax = roleDic[role].incomeMaximum;
        if (sMin > sMax)
            Debug.LogError("The role of '" + role + "' has a higher minimum income than maximum!");
        float t = (sMax - sMin) / 9;

        return Mathf.RoundToInt(sMin + (t * (loyalty - 1)));
    }

    public static int GetRandomizedGuestStayTime(/*input a guest stay multiplier of some kind here maybe?*/)
    {
        return Mathf.Clamp((int)GetApproximatedRandomValue(3, 2), Ref.guestMinStayTimeDays, Ref.guestMaxStayTimeDays);
    }

    #region Net Revenue Stuff
    //Can do more stuff with this later to access where the revenue came from or went to using the revenueinfo.

    //Can just use the average profit per day as the return value for any rooms that pay based on guest actions outside of a regular time interval, like casino's or gift shops, etc.

    public delegate void NetRevenueDelegate(List<RevenueInfo> list);
    public static NetRevenueDelegate NetRevenueCalculationEvent;

    public static int CalculateNetRevenue()
    {
        return Mathf.RoundToInt(CalculateHardNetRevenue());
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

    public static List<RevenueInfo> GetNetRevenueInfo()
    {
        List<RevenueInfo> list = new List<RevenueInfo>();
        NetRevenueCalculationEvent?.Invoke(list);
        return list;
    }
    #endregion
}

namespace MySpace
{
    [System.Serializable]
    public struct RoleInfo
    {
        public Enums.ManRole role;

        [Tooltip("Paid Daily")]
        public float incomeMinimum, incomeMaximum;
    }

    public struct RevenueInfo
    {
        public enum RevenueType : byte { Worker, Guest, Room }

        /// <summary>
        /// The increase or decrease in value that this object has on the revnue of each given day.
        /// </summary>
        public float effect;
        public RevenueType revenueType;
        public System.Guid objectId;

        /// <summary>
        /// If the revenue is estimated or simply a hard known value. Estimated values will come from non-daily sources, like a bar or casino that relies on guest usage.
        /// </summary>
        public bool estimated;

        public RevenueInfo(float value, RevenueType type, System.Guid id, bool estimated = false)
        {
            effect = value;
            revenueType = type;
            objectId = id;
            this.estimated = estimated;
        }
    }
}

