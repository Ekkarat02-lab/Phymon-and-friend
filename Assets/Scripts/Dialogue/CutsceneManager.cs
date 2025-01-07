using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CutsceneManager : MonoBehaviour
{
    public GameObject dialoguePrefab;
    private GameObject dialogueInstance;
    public AudioClip[] dialogueAudioClips;
    public Sprite[] dialogueImages;
    public RuntimeAnimatorController[] dialogueImageAnimations;
    private int currentLine = 0;
    public int currentStory;
    public string sceneName;

    private bool isPlayingAudio = false;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Animator transitionAnim;
    [SerializeField] private GameObject dialogueImageDisplay;

    public Button skipButton; // ปุ่ม Skip
    public Button returnToMenuButton; // ปุ่มกลับเมนูหลัก

    void Start()
    {
        dialogueInstance = Instantiate(dialoguePrefab, transform);
        dialogueInstance.SetActive(false);

        if (PlayerPref.instance != null)
        {
            PlayerPref.instance.currentStory = currentStory;
        }

        // ผูกปุ่ม Skip เข้ากับฟังก์ชัน
        skipButton.onClick.AddListener(SkipCutscene);
        returnToMenuButton.onClick.AddListener(ReturnToMainMenu); // ผูกปุ่มกลับเมนูหลัก

        UpdateDialogue();
    }

    public void ShowDialogue()
    {
        dialogueInstance.SetActive(true);
    }

    void UpdateDialogue()
    {
        if (currentLine < dialogueAudioClips.Length)
        {
            PlayDialogueAudio();
            DisplayDialogueImage();
        }
        else
        {
            dialogueInstance.SetActive(false);
            LoadNextScene(sceneName);
        }
    }

    void PlayDialogueAudio()
    {
        if (dialogueAudioClips[currentLine] != null)
        {
            audioSource.clip = dialogueAudioClips[currentLine];
            audioSource.Play();
            isPlayingAudio = true;
            StartCoroutine(WaitForAudioEnd());
        }
    }

    void DisplayDialogueImage()
    {
        if (currentLine < dialogueImageAnimations.Length && dialogueImageAnimations[currentLine] != null)
        {
            Animator animator = dialogueImageDisplay.GetComponent<Animator>();
            animator.runtimeAnimatorController = dialogueImageAnimations[currentLine];
            dialogueImageDisplay.SetActive(true);
        }
        else if (currentLine < dialogueImages.Length && dialogueImages[currentLine] != null)
        {
            SpriteRenderer spriteRenderer = dialogueImageDisplay.GetComponent<SpriteRenderer>();
            Animator animator = dialogueImageDisplay.GetComponent<Animator>();
            animator.runtimeAnimatorController = null;
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = dialogueImages[currentLine];
            }
            dialogueImageDisplay.SetActive(true);
        }
    }

    IEnumerator WaitForAudioEnd()
    {
        yield return new WaitForSeconds(audioSource.clip.length);
        isPlayingAudio = false;
        dialogueImageDisplay.SetActive(false);
        currentLine++;
        UpdateDialogue();
    }

    public void SkipCutscene()
    {
        // หยุดการทำงานทั้งหมดของ cutscene
        StopAllCoroutines();

        // ไปยังฉากถัดไปทันที
        LoadNextScene(sceneName);
    }

    void LoadNextScene(string sceneName)
    {
        StartCoroutine(LoadLevel());
    }

    IEnumerator LoadLevel()
    {
        transitionAnim.SetTrigger("End");
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(sceneName);
        transitionAnim.SetTrigger("Start");
    }

    public void ReturnToMainMenu()
    {
        StartCoroutine(ReturnToMainMenuCoroutine());
    }

    IEnumerator ReturnToMainMenuCoroutine()
    {
        transitionAnim.SetTrigger("End");
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("MainMenu");
        transitionAnim.SetTrigger("Start");
    }
}
