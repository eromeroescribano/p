using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableStoreTesting : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        VariableStore.CreateDatabase("DB_Numbers");
        VariableStore.CreateDatabase("DB_Booleans");

        VariableStore.CreateVariable("DB_Numbers.num1", 1);
        VariableStore.CreateVariable("DB_Numbers.num5", 5);
        VariableStore.CreateVariable("DB_Booleans.lightIsOn", true);
        VariableStore.CreateVariable("DB_Numbers.float1", 7.5f);
        VariableStore.CreateVariable("str1", "Hello");
        VariableStore.CreateVariable("str2", "World");
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            VariableStore.PrintAllVariables();
        }
        if(Input.GetKeyUp(KeyCode.A))
        {
            VariableStore.TryGetValue("DB_Numbers.num1", out object num1);
            VariableStore.TryGetValue("DB_Numbers.num5", out object num5);

            Debug.Log($"num1 + num2 = {(int)num1 + (int)num5}");
        }
        if(Input.GetKeyDown(KeyCode.D))
        {
            if(VariableStore.TryGetValue("DB_Booleans.lightIsOn",out object lightIsOn) && lightIsOn is bool)
            {
                VariableStore.TrySetValue("DB_Booleans.lightIsOn", !(bool)lightIsOn);
            }
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            VariableStore.TryGetValue("str1", out object str_hello);
            VariableStore.TryGetValue("str2", out object str_World);
            
            VariableStore.TrySetValue("str1", (string) str_hello+str_World);
        }
    }
}
