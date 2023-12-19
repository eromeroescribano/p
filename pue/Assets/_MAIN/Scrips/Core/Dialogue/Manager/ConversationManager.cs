using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DL_DIALOGUE_DATA;

public class ConversationManager
{
    private Coroutine process = null;
    public bool IsRunning() { return process != null; }

    private TextArchitect archit = null;
    private bool userPrompt = false;
    private LogiclLineManager logiclLineManager;
    public TextArchitect GetTextArchitect() { return archit; }

    private bool watingAuto = false;
    public bool GetWatingAuto() {  return watingAuto; }
    private ConversationQueue conversatioQueue;
    //private Conversation convesation=null;
    public Conversation GetConvesation()
    {
        if (conversatioQueue.IsEmpty())
        {  return null; }
        else
        { return conversatioQueue.top(); }
    }
    public int GetProgress() { return (conversatioQueue.IsEmpty() ? -1 : conversatioQueue.top().getProgress()); }
    public ConversationManager(TextArchitect textArchitect)
    {
        this.archit = textArchitect;
        DialogueSystem.Instance().AddPrompt_Next(OnUserPrompt_Next);
        logiclLineManager = new LogiclLineManager();
        conversatioQueue = new ConversationQueue();
    }
    public void Enqueue(Conversation convesation ){ conversatioQueue.Enqueue( convesation ); }
    public void EnqueuePriority(Conversation convesation ){ conversatioQueue.EnqueuePriority( convesation ); }
    private void OnUserPrompt_Next()
    { 
        userPrompt = true; 
    }

    public Coroutine StartConversation(Conversation conversation)
    {

        StopConversation();
        conversatioQueue.Clear();

        Enqueue( conversation );
        process = DialogueSystem.Instance().StartCoroutine(RunningConversation());
        return process;
    }
    public void StopConversation()
    {
        if (!IsRunning())
        { return; }

        else { DialogueSystem.Instance().StopCoroutine(process); process = null; }
    }

    IEnumerator RunningConversation()
    {

        while(!conversatioQueue.IsEmpty())
        {
            Conversation currentConversation = GetConvesation();
            
            if(currentConversation.HasReacheEnd())
            {
                conversatioQueue.Dequeue();
                continue;
            }

            string rawLine = currentConversation.CurrentLine();
            if (string.IsNullOrWhiteSpace(rawLine))
            {
                TryAdvanceConversation(currentConversation);
                continue; 
            }

            DIALOGUE_LINE line = DialogueParser.Parse(rawLine);
            if (logiclLineManager.TryGetLogic(line, out Coroutine logic))
            {
                yield return logic;
            }
            else 
            {
                if (line.HasDialogue())
                {
                    yield return Line_RunDialogue(line);
                }

                if (line.HasCommands())
                {
                    yield return Line_RunCommands(line);
                }
                if (line.HasDialogue())
                {
                    yield return WaitForUserInput();
                }
            }
            TryAdvanceConversation(currentConversation);
        }
        process = null;

    }
    private void TryAdvanceConversation(Conversation convesation)
    {
        convesation.IncremaentProgres();
        if(convesation != conversatioQueue.top())
        {
            return;
        }
        if(convesation.HasReacheEnd())
        {
            conversatioQueue.Dequeue();
        }

    }

    IEnumerator Line_RunDialogue(DIALOGUE_LINE line)
    {

        if (line.HasSpeaker()) 
        { 
            DialogueSystem.Instance().ShowName(TagManager.Inject(line.GetSpeaker().Displayname(),true, true));
                
            BacklogPanel.Instance().PutInTest(@$"{line.GetSpeaker().Displayname()} '{line.GetDialogue().GetRawData()}'");

        }
        else
        {
            BacklogPanel.Instance().PutInTest(@$"'{line.GetDialogue().GetRawData()}'");
        }
        yield return BuildLineSegments(line.GetDialogue());
    }
    IEnumerator BuildLineSegments(DL_DIALOGUE_DATA line)
    {
        for (int i = 0; i < line.GetsegmeDialogue().Count; ++i)
        {
            DIALOGUE_SEGMENT sement = line.GetsegmeDialogue()[i];
            yield return WaitForDialogueSegmentMarkToBeTriggered(sement);
            yield return BuildDialogue(sement.GetDialogue(), sement.AppendText());

            if (line.GetMAxim())
            {
                yield return WaitForUserInput();
                if (i == line.GetsegmeDialogue().Count - 2)
                {
                    line.SetMAxim(false);
                }
            }

        }

    }
    IEnumerator WaitForDialogueSegmentMarkToBeTriggered(DIALOGUE_SEGMENT sement)
    {

        switch (sement.GetDialogueMark())
        {
            case DIALOGUE_SEGMENT.DialogueSignals.C:
            case DIALOGUE_SEGMENT.DialogueSignals.A:
                yield return WaitForUserInput(); 
                break; 
            case DIALOGUE_SEGMENT.DialogueSignals.WC:
            case DIALOGUE_SEGMENT.DialogueSignals.WA:
                watingAuto = true;
                yield return new WaitForSeconds(sement.GetSignalWaitDelay());
                watingAuto=false;
                break;
            default: 
                break;
        }

    }

    
    IEnumerator Line_RunCommands(DIALOGUE_LINE line)
    {
        List<DL_COMMAND_DATA.Command> commands = line.GetCommands().GetCommands();
        foreach (DL_COMMAND_DATA.Command command in commands)
        {
            if (command.GetWait())
            { 
                yield return CommandManager.Instance().Execute(command.GetName(), command.GetArguments());
            }
            else
            {
                CommandManager.Instance().Execute(command.GetName(), command.GetArguments());
            }
        }
        yield return null;
    }

    IEnumerator BuildDialogue(string dialogue, bool append = false)
    {
        dialogue= TagManager.Inject(dialogue);
        if (!append)
        { archit.Build(dialogue); }
        else
        { archit.Append(dialogue); }
        while (archit.IsBuilding())
        {
            if (userPrompt)
            {
                if (!archit.GetHurryUp())
                {
                    archit.SetHurryUp(true);
                }
                else
                {
                    archit.Forcecompleted();
                }
                userPrompt = false;
            }
            yield return null;
        }
    }

    
    IEnumerator WaitForUserInput()
    {
        DialogueSystem.Instance().GetPront().Show();
        while (!userPrompt)
        { 
            yield return null; 
        }
        DialogueSystem.Instance().GetPront().Hide();
        userPrompt = false;
    }

}