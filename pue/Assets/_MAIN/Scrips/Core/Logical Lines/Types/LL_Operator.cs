using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;
using System;
using static LogicalLineUtils.Expressions;

public class LL_Operator : ILogicalLine
{
    public string Keyword()
    {
        throw new System.NotImplementedException();
    }
    public IEnumerator Execute(DIALOGUE_LINE line)
    {
        string trimmedLine=line.GetRawData().Trim();
        string[] parts= Regex.Split(trimmedLine, REGEX_ARITMATIC());

        if (parts.Length < 3 ) 
        {
            Debug.LogError($"Invalid command: {trimmedLine}");
            yield break;
        }
        string vari= parts[0].Trim().TrimStart(VariableStore.VARIABLE_ID());
        string op = parts[1].Trim();
        string[] remainingPars=new string[parts.Length-2];
        Array.Copy(parts,2, remainingPars,0,parts.Length-2);
        object value = CalculateValue(remainingPars);
        if (value == null) 
        {
            yield break;
        }
        ProcessOperators(vari, op, value);
    }
    private void ProcessOperators(string variable, string op , object value) 
    {
        if(VariableStore.TryGetValue(variable,out object currentValue))
        {
            ProcessOpOnVariable(variable, op, value, currentValue);
        }
        else if (op =="=")
        {
            VariableStore.CreateVariable(variable, value);
        }
    }
    private void ProcessOpOnVariable(string variable,string op, object value,object currentValue)
    {
        switch (op) 
        {
            case "=":
                VariableStore.TrySetValue(variable, value);
                break;
            case "+=":
                VariableStore.TrySetValue(variable, ConcatenateOfAdd(value, currentValue));
                break;
            case "-=":
                VariableStore.TrySetValue(variable, Convert.ToDouble(currentValue) - Convert.ToDouble(value));
                break;
            case "*=":
                VariableStore.TrySetValue(variable, Convert.ToDouble(currentValue) * Convert.ToDouble(value));
                break;
            case "/=":
                VariableStore.TrySetValue(variable, Convert.ToDouble(currentValue) / Convert.ToDouble(value));
                break;
            default:
                Debug.LogError($"Invalid operator: {op}");
                break;
        }
    }
    private object ConcatenateOfAdd(object value, object currentValue)
    {
        if(value is string)
        {
            return currentValue.ToString()+value;
        }
        return Convert.ToDouble(currentValue) + Convert.ToDouble(value);
    }
    public bool Maches(DIALOGUE_LINE lINE)
    {
        Match match = Regex.Match(lINE.GetRawData().Trim(), REGEX_OPERATOR_LINE());
        return match.Success;
    }
}
