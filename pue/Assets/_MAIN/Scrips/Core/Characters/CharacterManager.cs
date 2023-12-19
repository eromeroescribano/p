using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    static CharacterManager instance;
    public static CharacterManager Instance() { return instance; }
    private Dictionary<string, Character> characters =new Dictionary<string, Character>();
    private void Awake()
    {
        instance = this;
    }
}
