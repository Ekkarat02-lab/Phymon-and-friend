using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class RewardManager : MonoBehaviour
{
    public static RewardManager Instance;

    public GameObject dialoguePrefab;
    private GameObject dialogueInstance;
    public string[] dialogueLines;
    public AudioClip[] dialogueAudioClips;
    public Sprite[] dialogueImages;
    public Sprite[] characterSprites;

    private TextMeshProUGUI dialogueText;
    private bool isTyping = false;
    private bool skipTyping = false;
    public float typingSpeed = 0.05f;

    public GameObject goToMenuPanel;
    public string nextSceneName;
    
    // เพิ่มตัวแปรสำหรับปุ่ม Skip
    [SerializeField] private Button skipButton;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] Canvas EndCanvas;
    [SerializeField] private Image dialogueImage;
    [SerializeField] private Image characterImage;
    [SerializeField] private float imageLoopDelayed;
    [SerializeField] private int specificAudioIndex;

    private int currentLine = 0;
    private Coroutine imageLoopCoroutine;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        dialogueInstance = Instantiate(dialoguePrefab);
        dialogueText = dialogueInstance.GetComponentInChildren<TextMeshProUGUI>();
        //nextButton.onClick.AddListener(DisplayNextLine);
        dialogueInstance.SetActive(false);
        EndCanvas.enabled = false;
        
        dialogueInstance = Instantiate(dialoguePrefab);
        dialogueText = dialogueInstance.GetComponentInChildren<TextMeshProUGUI>();
        dialogueInstance.SetActive(false);
        EndCanvas.enabled = false;

        // ผูกฟังก์ชัน GoToSkipDialogue กับปุ่ม Skip
        if (skipButton != null)
        {
            skipButton.onClick.AddListener(SkipDialogue);
        }
    }

    public void GiveReward(int rewardAmount)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(rewardAmount);
        }
        ShowDialogue();
        if(PlayerPref.instance != null)
        {
            PlayerPref.instance.SaveClearedStory();
        }
    }

    public void ShowDialogue()
    {
        //dialogueInstance.SetActive(true);
        EndCanvas.enabled = true;
        currentLine = 0;
        characterImage.gameObject.SetActive(true);
        StartCoroutine(TypeDialogue());
    }

    private void DisplayNextLine()
    {
        if (isTyping)
        {
            skipTyping = true;
            return;
        }

        currentLine++;

        if (currentLine < dialogueLines.Length)
        {
            dialogueText.text = "";
            dialogueImage.gameObject.SetActive(false);
            StartCoroutine(TypeDialogue());
        }
        else
        {
            dialogueInstance.SetActive(false);
            ShowGoToMenuPanel();
        }
    }

    private IEnumerator TypeDialogue()
    {
        dialogueText.text = "";
        isTyping = true;
        skipTyping = false;

        bool shouldShowImage = dialogueLines[currentLine].Contains("[img]");
        string dialogueWithoutTag = dialogueLines[currentLine].Replace("[img]", "").Trim();

        if (currentLine < characterSprites.Length && characterSprites[currentLine] != null)
        {
            characterImage.sprite = characterSprites[currentLine];
        }

        if (dialogueAudioClips.Length > currentLine && dialogueAudioClips[currentLine] != null)
        {
            audioSource.clip = dialogueAudioClips[currentLine];
            audioSource.Play();
        }
        else
        {
            Debug.Log("No audio clip and Dialogue for line " + currentLine);
        }

        foreach (char letter in dialogueWithoutTag.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);

            if (skipTyping)
            {
                dialogueText.text = dialogueWithoutTag;
                break;
            }
        }

        if (currentLine == specificAudioIndex && dialogueImages.Length > 0)
        {
            if (imageLoopCoroutine != null)
            {
                StopCoroutine(imageLoopCoroutine);
            }
            imageLoopCoroutine = StartCoroutine(ImageLoop());
        }
        else
        {
            dialogueImage.gameObject.SetActive(false);
        }

        isTyping = false;
        skipTyping = false;

        while (audioSource.isPlaying)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        DisplayNextLine();
    }

    private void ShowGoToMenuPanel()
    {
        EndCanvas.enabled = false;
        if (goToMenuPanel != null)
        {
            goToMenuPanel.SetActive(true);
        }
    }

    public void GoToNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("Scene name is not set in the Inspector!");
        }
    }

    // ฟังก์ชันสำหรับข้ามบทสนทนา
    public void SkipDialogue()
    {
        if (isTyping)
        {
            // ถ้ากำลังพิมพ์ ข้ามไปที่ข้อความเต็มทันที
            skipTyping = true;

            // หยุดเสียงที่กำลังเล่นอยู่
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }

            return;
        }

        // หยุดเสียงที่กำลังเล่นอยู่
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        // ข้ามไปบรรทัดสุดท้ายหรือจบบทสนทนา
        currentLine = dialogueLines.Length; 
        dialogueInstance.SetActive(false);
        ShowGoToMenuPanel();
    }

    
    public void GoToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private IEnumerator ImageLoop()
    {
        int imageIndex = 0;
        while (true)
        {
            dialogueImage.sprite = dialogueImages[imageIndex];
            imageIndex = (imageIndex + 1) % dialogueImages.Length;
            dialogueImage.gameObject.SetActive(true);
            yield return new WaitForSeconds(imageLoopDelayed);
        }
    }
}
