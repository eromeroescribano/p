using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILogicalLine
{
    string Keyword();
    bool Maches(DIALOGUE_LINE line);
    IEnumerator Execute(DIALOGUE_LINE data);
}
