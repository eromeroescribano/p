using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class NameContainer 
{
    [SerializeField] private GameObject root;
    [SerializeField] private TextMeshProUGUI nameText;
    CanvasGroup can;
    public void Show(string maneToShow =" ")
    {
        can = root.GetComponent<CanvasGroup>();
        can.alpha=1;
        if (maneToShow != string.Empty ) 
        {
            nameText.text = maneToShow;
        }
    }
    public void Hide() 
    {
        can = root.GetComponent<CanvasGroup>();
        can.alpha = 0;
        //root.SetActive(false);
    }
}
