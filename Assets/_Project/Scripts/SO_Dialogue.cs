using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue",menuName ="SO/New Dialogue")]
public class SO_Dialogue : ScriptableObject
{
    public string playerAName;
    public string playerBName;
    public DialogueContainer[] dialogues;
}

[System.Serializable]
public class DialogueContainer
{
    public PlayerID playerID;
    [TextArea(3,3)]
    public string dialogue;
}
