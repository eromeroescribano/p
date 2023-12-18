using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueContinuePront : MonoBehaviour
{
    private RectTransform root;
    [SerializeField] private Animation anim;
    [SerializeField] private TextMeshProUGUI tmpro;
    public bool IsSowing() { return anim.gameObject.activeSelf; }
     void Start()
    {
        root = GetComponent<RectTransform>();
    }
    public void Show()
    {
        if(tmpro.text == string.Empty)
        {

        }
    }
    public void Hide()
    {

    }
}
