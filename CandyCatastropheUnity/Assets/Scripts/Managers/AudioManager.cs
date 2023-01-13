using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource MusicSource;
    [SerializeField] AudioSource UISource;
    [SerializeField] AudioSource EffectsSource;
    [SerializeField] AudioSource DialogueSource;

    float MasterVolume = 1.0f;
    float MusicVolume = 1.0f;
    float UIVolume = 1.0f;
    float EffectVolume = 1.0f;
    float DialogueVolume = 1.0f;

    #region AudioManager Singleton
    static private AudioManager am; //refence AudioManager
    static public AudioManager AM { get { return am; } } //public access to read only am 

    //Check to make sure only one am of the AudioManager is in the scene
    void CheckAudioManagerIsInScene()
    {

        //Check if instnace is null
        if (am == null)
        {
            am = this; //set am to this am of the game object
            Debug.Log(am + " Loaded");
        }
        else //else if am is not null an Audio Manager must already exsist
        {
            Destroy(this.gameObject); //In this case you need to delete this am
            Debug.Log("Audio Manager exists. Deleting...");
        }
    }
    #endregion

    private void Awake()
    {
        CheckAudioManagerIsInScene();
    }

    void UpdateAudioSourceVolumes()
    {
        MusicSource.volume = MasterVolume * MusicVolume;
        UISource.volume = MasterVolume * UIVolume;
        EffectsSource.volume = MasterVolume * EffectVolume;
        DialogueSource.volume = MasterVolume * DialogueVolume;
    }

    #region Play/Stop Audio Public Methods
    // I would like to update this to accomodate for multiple effects overlapping. This could potentially be accomplished by multiple audio sources on each
    // with some variant of a queue to determine which audio has a priority to stop if all playing. Doesn't seem too hard, just needs trial and error.

    public void PlayMusic(AudioClip clip, bool fadeOut = true, bool fadeIn = false)
    {

        //The fade in logic doesn't work due to being executed during one frame (Time.deltaTime doesn't work). Will need to use coroutines.
        if (MusicSource.isPlaying && fadeIn)
        {

        }
        MusicSource.Stop();
        MusicSource.clip = clip;
        MusicSource.Play();
        if (fadeOut)
        {

        }
        MusicSource.volume = MasterVolume * MusicVolume;
    }

    public void StopMusic(bool fadeOut = true)
    {
        MusicSource.Stop();
    }

    public void PlayUI(AudioClip clip)
    {
        UISource.Stop();
        UISource.clip = clip;
        UISource.Play();
    }

    public void StopUI()
    {
        UISource.Stop();
    }

    public void PlayEffect(AudioClip clip)
    {
        EffectsSource.Stop();
        EffectsSource.clip = clip;
        EffectsSource.Play();
    }

    public void StopEffect()
    {
        EffectsSource.Stop();
    }

    public void PlayDialogue(AudioClip clip)
    {
        DialogueSource.Stop();
        DialogueSource.clip = clip;
        DialogueSource.Play();
    }

    public void StopDialogue()
    {
        DialogueSource.Stop();
    }
    #endregion

    #region Update Volume Public Methods
    public void SetMasterVolume(float volume)
    {
        MasterVolume = volume;
        UpdateAudioSourceVolumes();
    }

    public void SetMusicVolume(float volume)
    {
        MusicVolume = volume;
        UpdateAudioSourceVolumes();
    }

    public void SetUIVolume(float volume)
    {
        UIVolume = volume;
        UpdateAudioSourceVolumes();
    }

    public void SetEffctsVolume(float volume)
    {
        EffectVolume = volume;
        UpdateAudioSourceVolumes();
    }
    #endregion

    #region Getters for Volume Control

    public float GetGlobalEffectsVolume()
    {
        return MasterVolume * EffectVolume;
    }

    public float GetGlobalMusicVolume()
    {
        return MasterVolume * MusicVolume;
    }

    public float GetGlobalDialogueVolume()
    {
        return MasterVolume * DialogueVolume;
    }

    #endregion

    public IEnumerator FadeOut(AudioSource source, float fadeTime, float sourceVolume)
    {
        while (source.volume > 0)
        {
            source.volume -= (sourceVolume * MasterVolume) * Time.deltaTime / fadeTime;
            yield return null;
        }
        source.Stop();
    }

    public IEnumerator FadeIn(AudioSource source, float fadeTime, float sourceVolume)
    {
        while (source.volume < (sourceVolume * MasterVolume))
        {
            source.volume += (sourceVolume * MasterVolume) * Time.deltaTime / fadeTime;
            yield return null;
        }
    }

}