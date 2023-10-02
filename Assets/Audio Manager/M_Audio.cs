using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public static class M_Audio
{
    private static Transform currentSceneAudio;
    private static float sceneAudioTransitionTime = 1;
    private static float globalVolumeOffset = 1;

    public static void PlayLoopAudio(string[] _AudioName)
    {
        if (currentSceneAudio != null)
        {
            AudioSource[] exsitingAudios = currentSceneAudio.GetComponentsInChildren<AudioSource>();
            foreach (AudioSource audio in exsitingAudios)
                DOTween.To(() => audio.volume, x => audio.volume = x, 0, sceneAudioTransitionTime);
            Object.Destroy(currentSceneAudio.gameObject, sceneAudioTransitionTime + 1);
            currentSceneAudio = null;
        }
        Transform sceneAudioParent = new GameObject("Sound Loop Parent").transform;
        foreach (string audio in _AudioName)
        {
            PlayLoopSoundFadeIn(audio);
        }

        void PlayLoopSoundFadeIn(string _AudioName)
        {
            GameObject soundGameObject = new GameObject("Sound Loop " + _AudioName);
            soundGameObject.transform.SetParent(sceneAudioParent);
            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            audioSource.clip = GetAudioClip(true, _AudioName).audioClip;
            audioSource.loop = true;
            audioSource.volume = 0;
            audioSource.Play();
            DOTween.To(() => audioSource.volume, x => audioSource.volume = x, GetAudioClip(true, _AudioName).volume * globalVolumeOffset, sceneAudioTransitionTime);
            currentSceneAudio = sceneAudioParent;
        }
    }

    public static void PlayOneShotAudio(string _AudioName)
    {
        GameObject soundGameObject = new GameObject("Sound " + _AudioName);
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.clip = GetAudioClip(false, _AudioName).audioClip;
        audioSource.volume = GetAudioClip(false,_AudioName).volume * globalVolumeOffset;
        audioSource.Play();
        Object.Destroy(soundGameObject, audioSource.clip.length);
    }

    public static void PlayOneShotAudio_Delay(string _AudioName, float time)
    {
        Sequence s = DOTween.Sequence();
        s.AppendInterval(time);
        s.AppendCallback(() => PlayOneShotAudio(_AudioName));
    }

    public static void PlayOneShotAudio_FixedTime(string _AudioName, float time)
    {
        GameObject soundGameObject = new GameObject("Sound " + _AudioName);
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.clip = GetAudioClip(false,_AudioName).audioClip;
        audioSource.volume = GetAudioClip(false,_AudioName).volume * globalVolumeOffset;
        audioSource.Play();
        Sequence s = DOTween.Sequence();
        s.AppendInterval(time);
        s.AppendCallback(() => audioSource.Stop());
        Object.Destroy(soundGameObject, audioSource.clip.length);
    }

    public static void GlobalVolumeChange(float offest)
    {
        AudioSource[] exsitingAudios = currentSceneAudio.GetComponentsInChildren<AudioSource>();
        foreach (AudioSource audio in exsitingAudios)
            audio.volume = GetAudioVolume(audio.clip) * globalVolumeOffset;
    }

    private static SoundAudioClip GetAudioClip(bool _IsLoopAudio,string _TargetAudioName)
    {
        if (_IsLoopAudio)
        {
            foreach (SoundAudioClip soundAudioClip in M_Global.Instance.repo_Audio.audio_Loop)
                if (soundAudioClip.soundName == _TargetAudioName) return soundAudioClip;
        }
        else
        {
            foreach (SoundAudioClip soundAudioClip in M_Global.Instance.repo_Audio.audio_OneShot)
                if (soundAudioClip.soundName == _TargetAudioName) return soundAudioClip;
        }
        Debug.LogError("Sound " + _TargetAudioName + " not Found");
        return null;
    }

    private static float GetAudioVolume(AudioClip toGetClip)
    {
        foreach (SoundAudioClip soundAudioClip in M_Global.Instance.repo_Audio.audio_OneShot)
            if (soundAudioClip.audioClip == toGetClip) return soundAudioClip.volume;
        foreach (SoundAudioClip soundAudioClip in M_Global.Instance.repo_Audio.audio_Loop)
            if (soundAudioClip.audioClip == toGetClip) return soundAudioClip.volume;
        Debug.LogError("Sound " + toGetClip + " not Found");
        return 1;
    }
}