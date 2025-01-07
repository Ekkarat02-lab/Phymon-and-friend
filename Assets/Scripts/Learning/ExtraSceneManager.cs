using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExtraSceneManager : MonoBehaviour
{
    public AudioClip[] dialogueAudioClips;
    public Sprite[] dialogueImages;
    public Sprite[] characterSprites;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] Canvas canvas;
    [SerializeField] private Image dialogueImage;
    [SerializeField] private Image characterImage;
    [SerializeField] private float imageLoopDelayed;
    [SerializeField] private int specificAudioIndex;

    private int currentLine = 0;
    private Coroutine imageLoopCoroutine;

    private void Start()
    {
        StartCoroutine(TypeDialogue());
    }

    private IEnumerator TypeDialogue()
    {

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

        while (audioSource.isPlaying)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        DisplayNextLine();
    }

    private void DisplayNextLine()
    {
        currentLine++;

        if (currentLine < dialogueAudioClips.Length)
        {
            dialogueImage.gameObject.SetActive(false);
            StartCoroutine(TypeDialogue());
        }
        else
        {
            GoToMainMenu();
        }
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

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
