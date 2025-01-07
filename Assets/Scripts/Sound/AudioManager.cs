using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource audioSource; // ใช้สำหรับเล่นเสียง
    public AudioClip clickButton; // คลิปเสียงเมื่อกดปุ่ม
    public AudioClip clickMascot; // คลิปเสียงเมื่อคลิกโดน Mascot
    public AudioClip winnerSound; // คลิปเสียงเมื่อเก็บ Mascot ครบแล้ว

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

        // ปิดเสียงตั้งแต่เริ่มเกม (ถ้ามีการเล่นเสียงใน AudioSource)
        //audioSource.Stop();
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

    // ฟังก์ชันสำหรับเล่นเสียงเมื่อเก็บ Mascot ครบแล้ว
    public void PlayWinnerSound()
    {
        if (winnerSound != null)
        {
            audioSource.PlayOneShot(winnerSound);
        }
    }
}