using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalletManager : MonoBehaviour
{
    private int monCoins;
    private int hoots;
    private int souls;

    public int Hoots
    {
        get { return hoots; }
        set
        {
            hoots = value;
            GuiManager.Ref.UpdateHootCount(hoots);
        }
    }
    public int Souls
    {
        get { return souls; }
        set
        {
            souls = value;
            GuiManager.Ref.UpdateSoulCount(souls);
        }
    }
    public int MonCoins
    {
        get { return monCoins; }
        set
        {
           monCoins = value;
           GuiManager.Ref.UpdateMonCoinCount(monCoins);
        }
    }

    //private GuiManager GUIRef = GuiManager.Ref;


    [HideInInspector]
    public static WalletManager Ref { get; private set; } // For external access of script


    void Awake()
    {
        if (Ref == null) Ref = GetComponent<WalletManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Hoots = Souls = MonCoins = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
