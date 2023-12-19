using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameSaveManager : MonoBehaviour
{
    static GameSaveManager Instance;
    [SerializeField] PlayerStatusSaveSchema player;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }


    }

    private void Start()
    {
        player.BuildGameVariablesDictionary();
    }

    public static GameSaveManager GetInstance() { return Instance; }

    public bool HasSaveDirectory()
    {

        if (Directory.Exists(Application.persistentDataPath + "/Saves")) { return true; }
        else { return false; }

    }

    public void SaveGame()
    {

        //PlayerSave.doAdd();

        if (!HasSaveDirectory()) { Directory.CreateDirectory(Application.persistentDataPath + "/Saves"); }
        //PlayerSave p = new PlayerSave(player.GetMoney(), player.GetCassAffection());
        player.SaveDictionaryValues();          //saves current value of variables ingame
        PlayerSave p = new PlayerSave(player);  //exports such variables to the serializable class PlayerSave


        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream SaveFile = File.Create(Application.persistentDataPath + "/Saves/SaveSlot1");
        binaryFormatter.Serialize(SaveFile, p);
        SaveFile.Close();


    }

    public void LoadGame()
    {

        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream SaveFile = File.Open(Application.persistentDataPath + "/Saves/SaveSlot1", FileMode.Open); // OK
        //PlayerSave = (PlayerSave)binaryFormatter.Deserialize(SaveFile);
        PlayerSave loadGame = new PlayerSave((PlayerSave)binaryFormatter.Deserialize(SaveFile));

        //Debug.Log(loadGame.getPlayerSaveMoney()); // OK

        player.loadSave(loadGame);

        //Debug.Log(player.GetMoney()); OK

        //PlayerStatusSaveSchema load = ScriptableObject.CreateInstance<PlayerStatusSaveSchema>(); // breaks
        //load.loadSave(loadGame);

        //Debug.Log(loadGame.getPlayerSaveMoney()); // KO

        //player = load;
        //player = ScriptableObject.CreateInstance<PlayerStatusSaveSchema>();
        //player = PlayerStatusSaveSchema(loadGame);
        SaveFile.Close();



    }

}
