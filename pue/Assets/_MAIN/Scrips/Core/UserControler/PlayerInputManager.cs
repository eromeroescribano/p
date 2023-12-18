using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        { 
            PromptAdvance(); 
        }
        if(Input.mouseScrollDelta.y>0f)
        {
            BacklogPanel.Instance().Show();
        }

    }
    public void PromptAdvance()
    {
        DialogueSystem.Instance().OnPressed();

    }
}
