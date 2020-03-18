using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class WalletManager
{
    private static int monCoins;
    private static int hoots;
    private static int souls;

    static WalletManager()
    {
        MonCoins = Hoots = Souls = 0;
    }

    public static int Hoots
    {
        get { return hoots; }
        private set
        {
            hoots = value;
            GuiManager.Ref?.UpdateHootCount(hoots);
        }
    }
    public static int Souls
    {
        get { return souls; }
        private set
        {
            souls = value;
            GuiManager.Ref?.UpdateSoulCount(souls);
        }
    }
    public static int MonCoins
    {
        get { return monCoins; }
        private set
        {
           monCoins = value;
           GuiManager.Ref?.UpdateMonCoinCount(monCoins);
        }
    }


    //[HideInInspector]
    //public static WalletManager Ref { get; private set; } // For external access of script

    public static bool SubtractHoots(int amount)
    {
        if (amount > Hoots)
            return false;
        else
        {
            Hoots -= amount;
            return true;
        }
    }

    public static void AddHoots(int amount)
    {
        Hoots += amount;
    }

    public static void SetHoots(int amount)
    {
        Hoots = amount;
    }

    public static bool SubtractMonCoins(int amount)
    {
        if (amount > MonCoins)
            return false;
        else
        {
            MonCoins -= amount;
            return true;
        }
    }

    public static void AddMonCoins(int amount)
    {
        MonCoins += amount;
    }

    public static void SetMonCoins(int amount)
    {
        MonCoins = amount;
    }

    public static bool SubtractSouls(int amount)
    {
        if (amount > Souls)
            return false;
        else
        {
            Souls -= amount;
            return true;
        }
    }

    public static void AddSouls(int amount)
    {
        Souls += amount;
    }

    public static void SetSouls(int amount)
    {
        Souls = amount;
    }
}
