using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource audioSource;
    public AudioClip clickButton;
    public AudioClip clickMascot;
    public AudioClip winnerSound;

    private void Awake()
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

    public void PlayClickButtonSound()
    {
        if (clickButton != null)
        {
            audioSource.PlayOneShot(clickButton);
        }
    }

    public void PlayClickMascotSound()
    {
        if (clickMascot != null)
        {
            audioSource.PlayOneShot(clickMascot);
        }
    }

    public void PlayWinnerSound()
    {
        if (winnerSound != null)
        {
            audioSource.PlayOneShot(winnerSound);
        }
    }
}