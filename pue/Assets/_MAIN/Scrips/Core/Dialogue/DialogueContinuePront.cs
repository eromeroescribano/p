using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueContinuePront : MonoBehaviour
{
    private RectTransform root;
    [SerializeField] private Animator anim;
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
            if(IsSowing())
            {
                Hide();
            }
            return;
        }
        tmpro.ForceMeshUpdate();
        anim.gameObject.SetActive(true);
        root.transform.SetParent(tmpro.transform);

        TMP_CharacterInfo finalCharacter= tmpro.textInfo.characterInfo[tmpro.textInfo.characterCount-1];
        Vector3 targetPos = finalCharacter.bottomRight;
        float chracterWidth = finalCharacter.pointSize * 0.5f;
        targetPos = new Vector3(targetPos.x + chracterWidth, targetPos.y, 0);
        root.localPosition = targetPos;
    }
    public void Hide()
    {
        anim.gameObject.SetActive(false);
    }
}
