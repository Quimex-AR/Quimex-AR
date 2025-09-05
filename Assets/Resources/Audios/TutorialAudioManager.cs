using System.Collections.Generic;
using UnityEngine;

public class TutorialAudioManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public List<AudioClip> audioClips;

    private Dictionary<string, AudioClip> audioDictionary;
    private int handSalutationCount = 0;

    void Awake()
    {
        audioDictionary = new Dictionary<string, AudioClip>();
        foreach (var clip in audioClips)
        {
            if (clip != null && !audioDictionary.ContainsKey(clip.name))
                audioDictionary.Add(clip.name, clip);
        }

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public void ResetSession()
    {
        handSalutationCount = 0;
    }

    public void PlayAudioForAnimation(string animationName)
    {
        if (string.IsNullOrEmpty(animationName)) return;

        string clipName = animationName;

        if (animationName == "hand_salutation")
        {
            clipName = handSalutationCount == 0 ? "hand_salutation" : "hand_salutation1";
            handSalutationCount++;
        }

        if (audioDictionary.TryGetValue(clipName, out var clip))
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning($"[TutorialAudioManager] No se encontró un audio con el nombre: {clipName}");
        }
    }
}
