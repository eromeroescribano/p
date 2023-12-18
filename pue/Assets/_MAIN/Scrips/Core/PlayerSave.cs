using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerSave
{
    int money;
    int cassAffection;

    /*
    public PlayerSave() { 
        this.money = 0;
        this.cassAffection = 0;
        Save = new PlayerStatusSaveSchema();
    }
    */
    //private void Awake()
    //{
    //    money = Save.GetMoney();
    //    cassAffection = Save.GetCassAffection();
    //}

    //public void doAdd() {
    //    Save.AddTest();
    //}

    public PlayerSave(int money, int cassAffection)
    {
        this.money = money;
        this.cassAffection = cassAffection;
    }
    public PlayerSave(PlayerSave newPlayerSave)
    {
        this.money = newPlayerSave.money;
        this.cassAffection = newPlayerSave.cassAffection;
    }

    public PlayerSave(PlayerStatusSaveSchema player)
    {
        this.money = player.GetMoney();
        this.cassAffection = player.GetCassAffection();
    }

    public int getPlayerSaveMoney() { return money; }
    public int getPlayerSaveCassAffection() { return cassAffection; }

}
