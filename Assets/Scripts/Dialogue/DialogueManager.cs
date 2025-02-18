using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialoguePrefab;
    private GameObject dialogueInstance;
    public string[] dialogueLines;
    public AudioClip[] dialogueAudioClips;
    public Sprite[] dialogueImages;
    private int currentLine = 0;
    public int currentStory;
    public string sceneName;

    private TextMeshProUGUI dialogueText;
    private Button nextButton;
    private bool isTyping = false;
    private bool skipTyping = false;
    public float typingSpeed = 0.05f;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Animator transitionAnim;
    [SerializeField] private Image dialogueImageDisplay;

    void Start()
    {
        dialogueInstance = Instantiate(dialoguePrefab, transform);
        dialogueInstance.SetActive(true);

        dialogueText = dialogueInstance.GetComponentInChildren<TextMeshProUGUI>();

        if (dialogueText == null)
        {
            Debug.LogError("TextMeshProUGUI not found.");
        }

        nextButton = dialogueInstance.GetComponentInChildren<Button>();

        if (nextButton == null)
        {
            Debug.LogError("Button not found.");
        }
        else
        {
            nextButton.onClick.AddListener(OnClickNext);
        }
        if(PlayerPref.instance != null)
        {
            PlayerPref.instance.currentStory = currentStory;
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
            dialogueImageDisplay.enabled = false;
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
        else
        {
            Debug.Log("No audio clip for line " + currentLine);
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
                if (currentLine < dialogueImages.Length)
                {
                    dialogueImageDisplay.sprite = dialogueImages[i];
                    dialogueImageDisplay.enabled = true;
                }
                else
                {
                    dialogueImageDisplay.enabled = false;
                }

                yield return new WaitForSecondsRealtime(3.2f);
                if (i == parts.Length - 2)
                {
                    dialogueImageDisplay.sprite = dialogueImages[6];
                }
                else
                {
                    dialogueImageDisplay.enabled = false;
                }
            }
        }

        isTyping = false;
        skipTyping = false;
    }

    public void OnClickNext()
    {
        if (isTyping)
        {
            skipTyping = true;
        }
        else
        {
            currentLine++;
            UpdateDialogue();
        }
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
}