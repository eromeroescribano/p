using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static LogicalLineUtils.Encapsulation;
using static LogicalLineUtils.Conditions;
public class LL_Condition : ILogicalLine
{
    
    public string Keyword() { return "if";}
    private string ELSE = "else";
    private string[] CONTAINERS = new string[] { "(", ")" };
    public IEnumerator Execute(DIALOGUE_LINE line)
    {
        string RaeCondition = ExtractCondition(line.GetRawData().Trim());
        bool conditionResult = EvaluateCondition(RaeCondition);
        Conversation currentConversation = DialogueSystem.Instance().GetConversationManager().GetConvesation();
        int currentProgress = DialogueSystem.Instance().GetConversationManager().GetProgress();
        EncapsulatedData ifData = RipEncapsulatedData(currentConversation, currentProgress, false);
        EncapsulatedData elseData = new EncapsulatedData();
        if (ifData.GetEndingIndex()+1 < currentConversation.Count())
        {
            string nextLine = currentConversation.GetLines()[ifData.GetEndingIndex()+1].Trim();
            if (nextLine == ELSE)
            {
                elseData = RipEncapsulatedData(currentConversation, ifData.GetEndingIndex() + 1, false);
                ifData.SetEndingIndex(elseData.GetEndingIndex());
            }
        }

        currentConversation.setProgress(ifData.GetEndingIndex());
        
        EncapsulatedData selectedData =conditionResult ? ifData: elseData;
        if(!selectedData.IsNull() && selectedData.GetLines().Count > 0)
        {
            Conversation newConversation = new Conversation(selectedData.GetLines());
            DialogueSystem.Instance().GetConversationManager().GetConvesation().setProgress(selectedData.GetEndingIndex());
            DialogueSystem.Instance().GetConversationManager().EnqueuePriority(newConversation);
        }
        yield return null;
    }

    public bool Maches(DIALOGUE_LINE line)
    {
        return line.GetRawData().Trim().StartsWith(Keyword());
    }
    private string ExtractCondition(string line)
    {
        int startIndex= line.IndexOf(CONTAINERS[0])+1;
        int endIndex= line.IndexOf(CONTAINERS[1]);

        return line.Substring(startIndex, endIndex- startIndex).Trim();
    }
}
