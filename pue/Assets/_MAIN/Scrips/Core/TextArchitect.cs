using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading;
using System;

public class TextArchitect
{

    private TextMeshProUGUI tmpro_ui;
    private TextMeshPro tmpro_word;

    public TMP_Text Tmpro()
    {
        if (tmpro_ui != null)
        { return tmpro_ui; }
        else
        { return tmpro_word; }
    }
    public string CurrentText() { return Tmpro().text; }
    private string targetText = "";
    public string GetTargetText() { return targetText; }
    private string preText = "";

    public string GetPreText() { return preText; }

    public string fullTargetText() { return preText + targetText; }
    public enum BuildMethod { instant, typewriter, fade }

    private BuildMethod buildMethod = BuildMethod.typewriter;
    public void SetBuildMethod(BuildMethod buildMethod) { this.buildMethod = buildMethod; }
    public Color textColor { get { return Tmpro().color; } set { Tmpro().color = value; } }
    public float speed { get { return baseSpeed * speedMultiplier; } set { speedMultiplier = value; } }
    private float baseSpeed = 1;
    private float speedMultiplier = 1;

    public int characterPerCycle { get { return speed <= 2f ? characterMultiplier : speed <= 2.5f ? characterMultiplier * 2 : characterMultiplier * 3; } }
    private int characterMultiplier = 1;

    private bool hurryUp = false;
    public void SetHurryUp(bool hurryUp) { this.hurryUp = hurryUp; }
    public bool GetHurryUp() { return hurryUp; }

    public TextArchitect(TextMeshProUGUI tmpro_ui)
    {
        this.tmpro_ui = tmpro_ui;
    }
    public TextArchitect(TextMeshPro tmpro_word)
    {
        this.tmpro_word = tmpro_word;
    }


    public Coroutine Build(string text)
    {
        preText = "";
        targetText = text;
        Stop();
        buildProcess = Tmpro().StartCoroutine(Building());

        return buildProcess;
    }
    public Coroutine Append(string text)
    {
        preText = Tmpro().text;
        targetText = text;
        Stop();
        buildProcess = Tmpro().StartCoroutine(Building());
        return buildProcess;
    }

    private Coroutine buildProcess = null;

    public bool IsBuilding() { return buildProcess != null; }
    public void Stop()
    {
        if (!IsBuilding()) { return; }
        Tmpro().StopCoroutine(buildProcess);
        buildProcess = null;
    }
    IEnumerator Building()
    {
        Prepare();
        switch (buildMethod)
        {
            case BuildMethod.typewriter:
                yield return Buid_Typewriter();
                break;
            case BuildMethod.fade:
                yield return Buid_Fade();
                break;
        }
        OnConplete();
    }
    private void OnConplete()
    {
        buildProcess = null;
        hurryUp = false;

    }
    public void Forcecompleted()
    {
        switch (buildMethod)
        {
            case BuildMethod.typewriter:
                Tmpro().maxVisibleCharacters = Tmpro().textInfo.characterCount;
                break;
            case BuildMethod.fade:
                break;

        }
        Stop();
        OnConplete();
    }

    private void Prepare()
    {
        switch (buildMethod)
        {
            case BuildMethod.instant:
                Prepare_Instant();
                break;
            case BuildMethod.typewriter:
                Prepare_Typewriter();
                break;
            case BuildMethod.fade:
                Prepare_Fade();
                break;
        }

    }
    private void Prepare_Instant()
    {
        Tmpro().color = Tmpro().color;
        Tmpro().text = fullTargetText();
        Tmpro().ForceMeshUpdate();
        Tmpro().maxVisibleCharacters = Tmpro().textInfo.characterCount;
    }
    private void Prepare_Typewriter()
    {
        Tmpro().color = Tmpro().color;
        Tmpro().maxVisibleCharacters = 0;
        Tmpro().text = preText;
        if (preText != "")
        {
            Tmpro().ForceMeshUpdate();
            Tmpro().maxVisibleCharacters = Tmpro().textInfo.characterCount;
        }
        Tmpro().text += targetText;
        Tmpro().ForceMeshUpdate();


    }
    private void Prepare_Fade()
    {

    }
    private IEnumerator Buid_Typewriter()
    {
        while (Tmpro().maxVisibleCharacters < Tmpro().textInfo.characterCount)
        {
            Tmpro().maxVisibleCharacters += hurryUp ? characterPerCycle * 5 : characterPerCycle;
            yield return new WaitForSeconds(0.015f / speed);
        }
    }
    private IEnumerator Buid_Fade()
    {
        yield return null;
    }
}