using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CutsceneOneManager : MonoBehaviour
{
    public GameObject dialoguePrefab;
    private GameObject dialogueInstance;
    public string[] dialogueLines;
    public AudioClip[] dialogueAudioClips;
    public Sprite[] dialogueImages;
    public RuntimeAnimatorController[] dialogueImageAnimations;
    private int currentLine = 0;
    public int currentStory;
    public string sceneName;

    private TextMeshProUGUI dialogueText;
    private bool isTyping = false;
    private bool skipTyping = false;
    public float typingSpeed = 0.05f;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Animator transitionAnim;
    [SerializeField] private GameObject dialogueImageDisplay;

    // New UI Buttons
    [SerializeField] private Button menuButton;
    [SerializeField] private Button skipButton;
    public string menuSceneName = "MainMenu";

    void Start()
    {
        dialogueInstance = Instantiate(dialoguePrefab, transform);
        dialogueInstance.SetActive(false);
        dialogueText = dialogueInstance.GetComponentInChildren<TextMeshProUGUI>();

        if (dialogueText == null)
        {
            Debug.LogError("TextMeshProUGUI not found.");
        }

        if (PlayerPref.instance != null)
        {
            PlayerPref.instance.currentStory = currentStory;
        }
        
        if (menuButton != null)
        {
            menuButton.onClick.AddListener(ReturnToMenu);
        }

        if (skipButton != null)
        {
            skipButton.onClick.AddListener(SkipToNextScene);
        }

        UpdateDialogue();
    }

    public void ShowDialogue()
    {
        dialogueInstance.SetActive(true);
    }

    void UpdateDialogue()
    {
        if (currentLine < dialogueLines.Length)
        {
            StartCoroutine(TypeDialogue(dialogueLines[currentLine]));
            dialogueImageDisplay.SetActive(true);
        }
        else
        {
            dialogueInstance.SetActive(false);
            LoadNextScene(sceneName);
        }
    }

    IEnumerator TypeDialogue(string line)
    {
        dialogueText.text = "";
        isTyping = true;
        skipTyping = false;

        if (dialogueAudioClips.Length > currentLine && dialogueAudioClips[currentLine] != null)
        {
            audioSource.clip = dialogueAudioClips[currentLine];
            audioSource.Play();
        }

        string[] parts = line.Split(new string[] { "[delay]" }, System.StringSplitOptions.None);

        for (int i = 0; i < parts.Length; i++)
        {
            foreach (char letter in parts[i].ToCharArray())
            {
                if (skipTyping)
                {
                    dialogueText.text = string.Join("", parts);
                    break;
                }

                dialogueText.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }

            if (i < parts.Length - 1)
            {
                if (currentLine < dialogueImageAnimations.Length && dialogueImageAnimations[i] != null)
                {
                    Animator animator = dialogueImageDisplay.GetComponent<Animator>();
                    animator.runtimeAnimatorController = dialogueImageAnimations[i];
                    dialogueImageDisplay.SetActive(true);
                }
                else if (currentLine < dialogueImages.Length && dialogueImages[i] != null) 
                {
                    SpriteRenderer spriteRenderer = dialogueImageDisplay.GetComponent<SpriteRenderer>();
                    Animator animator = dialogueImageDisplay.GetComponent<Animator>();
                    animator.runtimeAnimatorController = null;
                    if (spriteRenderer != null)
                    {
                        spriteRenderer.sprite = dialogueImages[i];
                    }
                    dialogueImageDisplay.SetActive(true);
                }

                yield return new WaitForSecondsRealtime(3.2f);
                if (i == parts.Length - 2 && dialogueImageAnimations[6] != null)
                {
                    Animator animator = dialogueImageDisplay.GetComponent<Animator>();
                    animator.runtimeAnimatorController = dialogueImageAnimations[6];
                    dialogueImageDisplay.SetActive(true);
                }

                else if (i == parts.Length - 2)
                {
                    SpriteRenderer spriteRenderer = dialogueImageDisplay.GetComponent<SpriteRenderer>();
                    Animator animator = dialogueImageDisplay.GetComponent<Animator>();
                    spriteRenderer.sprite = dialogueImages[6];
                    animator.runtimeAnimatorController = null;
                }

                else
                {
                    dialogueImageDisplay.SetActive(false);
                }
            }
        }

        isTyping = false;
        yield return new WaitForSeconds(2f);

        currentLine++;
        UpdateDialogue();
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

    void ReturnToMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }

    void SkipToNextScene()
    {
        LoadNextScene(sceneName);
    }
}
