using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource ambientSource;
    
    [Header("Sound Effects")]
    public AudioClip gravityFlipSound;
    public AudioClip leverScream;
    public AudioClip timeDelayEcho;
    public AudioClip doorOpen;
    public AudioClip wrongDoorTrap;
    public AudioClip memoryWhisper1;
    public AudioClip memoryWhisper2;
    public AudioClip memoryWhisper3;
    public AudioClip jumpscare;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip, volume);
    }
    
    public void PlayWhisper(int fragmentNumber)
    {
        switch(fragmentNumber)
        {
            case 1:
                PlaySFX(memoryWhisper1, 0.7f);
                break;
            case 2:
                PlaySFX(memoryWhisper2, 0.7f);
                break;
            case 3:
                PlaySFX(memoryWhisper3, 0.7f);
                break;
        }
    }
    
    public void SetAmbientVolume(float volume)
    {
        ambientSource.volume = volume;
    }
}