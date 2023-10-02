using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Audio Repo",menuName ="Scriptable Obj/Audio Repo")]
public class SO_AudioRepo : ScriptableObject
{
    [Header("Audio")]
    public SoundAudioClip[] audio_Loop;
    public SoundAudioClip[] audio_OneShot;
}

[System.Serializable]
public class SoundAudioClip
{
    public string soundName;
    public AudioClip audioClip;
    [Range(0f, 1f)]
    public float volume;
}