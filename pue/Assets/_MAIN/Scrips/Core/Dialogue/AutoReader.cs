using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class AutoReader : MonoBehaviour
{
    private int DEFAUTL_CHARACTER = 18;
    private float READER_TIME_PADING = 0.5f;
    private float MAX_READ_TIME = 99f;
    private float MIN_READ_TIME = 1f;
    private string STATUS_TEXT_AUTO = "AUTO";
    private string STATUS_TEXT_SKIP = "Skiping";


    private ConversationManager conversationManager;
    private TextArchitect architect;
    private bool skip=false;
    private float speed = 1f;
    public bool GetSkip() { return skip; }
    public float GetSpeed() { return speed; }
    public bool IsOn() { return co_runing != null; }
    private Coroutine co_runing = null;
    [SerializeField]private TextMeshProUGUI statusText;
    public void Initialize(ConversationManager conversation)
    {
        this.conversationManager = conversation;
        architect = conversationManager.GetTextArchitect();
        statusText.text = string.Empty;
    }
    public void Enable()
    {
        if (IsOn()) { return; }
        co_runing = StartCoroutine(AutoRead());
    }
    public void Disable()
    {
        if (!IsOn()) { return; }
        StopCoroutine(co_runing);
        skip = false;
        co_runing = null;
        statusText.text = string.Empty;
    }
    private IEnumerator AutoRead()
    {
        if(!conversationManager.IsRunning())
        {
            Disable();
            yield break;
        }
        if(!architect.IsBuilding() && architect.CurrentText() != string.Empty)
        {
            DialogueSystem.Instance().OnAutoPressed();
        }
        while(conversationManager.IsRunning())
        {
            if(!skip)
            {
                while(!architect.IsBuilding() && !conversationManager.GetWatingAuto())
                { yield return null; }

                float timeStarted=Time.time;

                while (!architect.IsBuilding() || conversationManager.GetWatingAuto())
                { yield return null; }

                float timeRead = Mathf.Clamp(((float) architect.Tmpro().textInfo.characterCount/DEFAUTL_CHARACTER), MIN_READ_TIME, MAX_READ_TIME);
                timeRead = Mathf.Clamp((timeRead-(Time.time-timeStarted)),MIN_READ_TIME,MAX_READ_TIME);
                timeRead = (timeRead / speed) + READER_TIME_PADING;
                yield return new WaitForSeconds(timeRead);
            }
            else
            {
                architect.Forcecompleted();
                yield return new WaitForSeconds(0.05f);
            }
            DialogueSystem.Instance().OnAutoPressed();
        }
    }
    public void Toggle_Auto()
    {
        if(skip)
        { Enable(); }
        else
        {
            if(!IsOn())
            { Enable(); }
            else
                Disable();
        }
        statusText.text = STATUS_TEXT_AUTO;
        skip = false;
    }
    public void Toggle_Skip()
    {
        if (!skip)
        { Enable(); }
        else
        {
            if (!IsOn())
            { Enable(); }
            else
                Disable();
        }
        statusText.text=STATUS_TEXT_SKIP;
        skip = true;
    }
}
