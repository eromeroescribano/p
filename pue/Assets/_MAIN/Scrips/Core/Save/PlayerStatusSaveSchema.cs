using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

[Serializable]
[CreateAssetMenu(fileName = "PlayerStatus", menuName = "SaveComponents", order = 1)]
public class PlayerStatusSaveSchema : ScriptableObject
{
    [SerializeField] int money;
    [SerializeField] int cassAffection;

    //get
    public PlayerStatusSaveSchema GetPlayerStatusSaveSchema() { return this; }
    public int GetMoney() { return money; }
    public int GetCassAffection() { return cassAffection; }

    //constructor
    public PlayerStatusSaveSchema(int moneyValue, int cassAffectionValue)
    {
        this.money = moneyValue;
        this.cassAffection = cassAffectionValue;
    }
    // IF "money = 30"
    //     {
    //    cassAffection >=30 
    //   { goto script CassIntroduction}
    //}else if{ dfg}
    //  goto Introduction
    //  if { } else else if



    //dictionary
    static Dictionary<string, int> gameVariables;

    public static Dictionary<string, int> GetVariablesDictionary() { return gameVariables; }

    public void BuildGameVariablesDictionary()
    {

        gameVariables = new Dictionary<string, int>();
        gameVariables.Add("money", this.money);
        gameVariables.Add("cassAffection", this.cassAffection);

    }




    //save values of dictionary to schema Variables
    public void SaveDictionaryValues()
    {

        this.money = gameVariables["money"];
        this.cassAffection = gameVariables["cassAffection"];

    }

    //rewrite current scriptableObject values with those of a previous save
    public PlayerStatusSaveSchema(PlayerSave save)
    {

        this.money = save.getPlayerSaveMoney();
        this.cassAffection = save.getPlayerSaveCassAffection();

    } //might be useless

    public void loadSave(PlayerSave save)
    {

        this.money = save.getPlayerSaveMoney();
        this.cassAffection = save.getPlayerSaveCassAffection();

        BuildGameVariablesDictionary();

    }
    public void loadIntoSchema(PlayerStatusSaveSchema newStatus)
    {
        //this.gameVariables = newStatus.gameVariables;
        SaveDictionaryValues();
    }

    /// <summary>
    /// ////////
    /// </summary>
    /// <returns></returns>



    //set
    public void SetMoney(int newMoney) { this.money = newMoney; }
    public void SetCassAffection(int newAffection) { this.cassAffection = newAffection; }

    //add
    public void AddMoney(int moneyToAdd) { this.money += moneyToAdd; }

    //Add - Cass Affection
    public void AddCassAffection() { ++cassAffection; }
    public void AddCassAffection(int affectionValue) { cassAffection += affectionValue; }


    //test
    public void AddTest()
    {
        this.AddMoney(20);
        this.AddCassAffection();

    }

}
