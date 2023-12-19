using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CommandParameters
{
    private char PARAMETER_IDENTIFIER = '-';

    private Dictionary<string,string> parameters = new Dictionary<string,string>();

    public CommandParameters(string[] parametersArray) 
    {
        for (int i = 0; i < parametersArray.Length; ++i)
        {
            if (parametersArray[i].StartsWith(PARAMETER_IDENTIFIER))
            {
                string pName = parametersArray[i];
                string pValue = "";
                if(i+1<parametersArray.Length && !parametersArray[i + 1].StartsWith(PARAMETER_IDENTIFIER))
                {
                    pValue = parametersArray[i + 1];
                    ++i;
                }
                parameters.Add(pName, pValue);
            }
        }
    }

    public bool TryGetValue<T>(string parameterName, out T value, T defaultValue = default(T)) { return TryGetValue(new string[] { parameterName }, out value, defaultValue); }
    public bool TryGetValue<T>(string[] parameterNames, out T value, T defaultValue = default(T))
    {
        foreach (string parameterName in parameterNames) 
        {
            if(parameters.TryGetValue(parameterName,out string parameterValue))
            {
                if(TryCastParameter(parameterValue,out value))
                {
                    return true;
                }
            }
        }
        value = defaultValue;
        return false;
    }
    private bool TryCastParameter<T>(string parameterValue,out T value)
    {
        if(typeof(T) == typeof(bool))
        {
            if(bool.TryParse(parameterValue, out bool boolValue))
            {
                value=(T)(object) boolValue;
                return true;
            }
        }
        else if(typeof(T) == typeof(int))
        {
            if(int.TryParse(parameterValue, out int intValue))
            {
                value=(T)(object)intValue;
                return true;
            }
        }
        else if(typeof(T) == typeof(float))
        {
            if(float.TryParse(parameterValue, out float floatValue))
            {
                value=(T)(object)floatValue;
                return true;
            }
        }
        else if(typeof(T) == typeof(string))
        {
            
            value=(T)(object)parameterValue;
            return true;
        }
        value = default(T);
        return false;

    }
}
