using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class TagManager
{
    private static Dictionary<string, Func<string>> tags = new Dictionary<string, Func<string>>()
    {
        {"<mainchar>", () => "Luis"},
        {"<time>", () => DateTime.Now.ToString("hh:mm:tt")},
        {"<playerlevel>", () => "15"},
        {"<input>",() => InputPanel.Instance().getLastInput()},
        {"<player>",() => Inject("<input>",false,true)}
    };
    private static Regex tagRegex = new Regex("<\\w+>");
    /*
    public TagManager() 
    {
        InitiateTag();
    }
    private static void InitiateTag()
    {
        tags["<mainchar>"] = () => "Luis";
        tags["<time>"] = () => DateTime.Now.ToString("hh:mm:tt");
        tags["<playerlevel>"] =() => "15";
        tags["<temvall>"] =() => "42";
        tags["<input>"] =() => InputPanel.Instance().getLastInput();
    }*/
    public static string Inject(string text, bool injeTag = true, bool injectVar = true)
    {
        if (injeTag) 
        {
            text = InjectTag(text); 
        }
        if (injectVar) 
        {
            text = InjectVariables(text); 
        }
        return text;
    }
    public static string InjectTag(string text)
    {
        if (tagRegex.IsMatch(text))
        {
            foreach (Match match in tagRegex.Matches(text))
            {
                if (tags.TryGetValue(match.Value, out var tagValueRequest))
                {
                    text = text.Replace(match.Value, tagValueRequest());
                }
            }
        }
        return text;
    }

    public static string InjectVariables(string value)
    {
        var matches = Regex.Matches(value, VariableStore.REGEX_Variable_IDS());
        var matchesList=matches.Cast<Match>().ToList();
        for(int i = matchesList.Count-1; i >=0 ; --i)
        { 
            var match = matchesList[i];
            string variableName= match.Value.TrimStart(VariableStore.VARIABLE_ID(),'!');
            bool negate = match.Value.StartsWith('!');
            bool endsInlegalChar = variableName.EndsWith(VariableStore.DATAB_VARIABLE_ID());
            if(endsInlegalChar)
            {
                variableName = variableName.Substring(0, variableName.Length - 1);
            }
            
            if(!VariableStore.TryGetValue(variableName,out object variableValue))
            {
                UnityEngine.Debug.LogError($"Variable{variableName} not found");
                continue;
            }
            if(negate && variableValue is bool)
            {
                variableValue = !(bool)variableValue;
            }

            int lenghtRemoved = match.Index+match.Length> value.Length ? value.Length+match.Index : match.Length;
            if(endsInlegalChar)
            {
                lenghtRemoved -= 1;
            }
            value =value.Remove(match.Index, lenghtRemoved);
            value = value.Insert(match.Index, variableValue.ToString());
        }
        return value;
    }
}
