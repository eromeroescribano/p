using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class DL_DIALOGUE_DATA
{
    private string rawData = string.Empty;
    public string GetRawData() { return rawData; }
    private const string segmentPattern = @"\{[ca]\}|\{w[ca]\s\d*\.?\d*\}";

    List<DIALOGUE_SEGMENT> dialogueSegments;
    public List<DIALOGUE_SEGMENT> GetsegmeDialogue() { return dialogueSegments; }
    int maxChar = 500;
    bool MAxim=false;
    public bool GetMAxim() { return MAxim; }
    public void SetMAxim(bool MAxim) { this.MAxim= MAxim; }
    public bool HasDialogue() { return dialogueSegments.Count > 0; }
    public DL_DIALOGUE_DATA(string rawDialogue)
    {
        rawData= rawDialogue;
        dialogueSegments = RipSegments(rawDialogue);

    }


    public List<DIALOGUE_SEGMENT> RipSegments(string rawDialogue)
    {

        List<DIALOGUE_SEGMENT> sumOfAllSegments = new List<DIALOGUE_SEGMENT>();

        MatchCollection matches = Regex.Matches(rawDialogue, segmentPattern);

        int lastIndex = 0;

        DIALOGUE_SEGMENT segment = new DIALOGUE_SEGMENT();

        segment.SetDialogue(matches.Count == 0 ? rawDialogue : rawDialogue.Substring(0, matches[0].Index));

        segment.SetDialogueMarks(DIALOGUE_SEGMENT.DialogueSignals.NONE);
        segment.SetSignalDelay(0);
        sumOfAllSegments = Multy(segment, sumOfAllSegments);

        if (matches.Count == 0) { return sumOfAllSegments; }
        else { lastIndex = matches[0].Index; }

        for (int i = 0; i < matches.Count; ++i)
        {

            Match match = matches[i];
            segment = new DIALOGUE_SEGMENT();

            string matchMark = match.Value;
            matchMark = matchMark.Substring(1, match.Length - 2);
            string[] splitMark = matchMark.Split(' ');
            segment.SetDialogueMarks((DIALOGUE_SEGMENT.DialogueSignals)Enum.Parse(typeof(DIALOGUE_SEGMENT.DialogueSignals), splitMark[0].ToUpper()));
            if (splitMark.Length > 1)
            {
                float.TryParse(splitMark[1], System.Globalization.NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture.NumberFormat, out float signalWaitDelayDecimals);
                segment.SetSignalDelay(signalWaitDelayDecimals);

            }
            int nextIndex = i + 1 < matches.Count ? matches[i + 1].Index : rawDialogue.Length;
            segment.SetDialogue(rawDialogue.Substring(lastIndex + match.Length, nextIndex - (lastIndex + match.Length)));

            lastIndex = nextIndex;

            sumOfAllSegments= Multy(segment, sumOfAllSegments);
        }
        return sumOfAllSegments;
    }
    private List<DIALOGUE_SEGMENT> Multy(DIALOGUE_SEGMENT segmento, List<DIALOGUE_SEGMENT> segmentos)
    {
        if (segmento.GetDialogue().Length < maxChar)
        {
            segmentos.Add(segmento);
            MAxim = false;
        }
        else
        {
            MAxim = true;
            int len = segmento.GetDialogue().Length;
            DIALOGUE_SEGMENT semento1 = new DIALOGUE_SEGMENT();
            DIALOGUE_SEGMENT semento2 = new DIALOGUE_SEGMENT();
            if (segmento.GetDialogue()[maxChar] ==' ')
            {
                semento1.SetDialogue(segmento.GetDialogue().Substring(0, maxChar));
                semento2.SetDialogue(segmento.GetDialogue().Substring( maxChar));
            }
            else
            {
                int corte = maxChar;
                while (segmento.GetDialogue()[corte] != ' ')
                {
                    --corte;
                }
                semento1.SetDialogue(segmento.GetDialogue().Substring(0, corte));
                semento2.SetDialogue(segmento.GetDialogue().Substring(corte));
            }
            segmentos.Add(semento1);
            segmentos.Add(semento2);
        }
        return segmentos;
    }

    public class DIALOGUE_SEGMENT
    {

        string dialogue;
        DialogueSignals DialogueMarks;
        float signalDelay;
        public enum DialogueSignals { NONE, C, A, WA, WC }
        public bool AppendText() { return (DialogueMarks == DialogueSignals.A || DialogueMarks == DialogueSignals.WA); }

        public void SetDialogue(string dialogue) { this.dialogue = dialogue; }
        public void SetDialogueMarks(DialogueSignals val)
        {
            DialogueMarks = val;
        }
        public DialogueSignals GetDialogueMark()
        {
            return DialogueMarks;
        }

        public void SetSignalDelay(float signalDelay) { this.signalDelay = signalDelay; }
        public float GetSignalWaitDelay() { return signalDelay; }
        public string GetDialogue() { return dialogue; }

    }
}
