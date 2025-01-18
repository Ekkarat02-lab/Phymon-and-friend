using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class Question
{
    public string questionText;
    public string[] answers;
    public int correctAnswerIndex;
}

public class QuestionManager : MonoBehaviour
{
    public GameObject questionPanel;
    public TextMeshProUGUI questionText;
    public Button[] answerButtons;

    public LevelManager levelManager;
    public Question[] questions;

    private int currentQuestionIndex = 0;

    void Start()
    {
        if (questionPanel != null)
        {
            questionPanel.SetActive(false);
        }
    }

    public void ShowQuestion()
    {
        if (questions.Length == 0)
        {
            Debug.LogError("No questions available!");
            return;
        }

        if (currentQuestionIndex >= questions.Length)
        {
            Debug.LogError("Question index out of range!");
            return;
        }

        if (questionPanel != null)
        {
            questionPanel.SetActive(true);
            SetQuestion(questions[currentQuestionIndex]);
        }
    }

    private void SetQuestion(Question question)
    {
        questionText.text = question.questionText;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (i < question.answers.Length)
            {
                answerButtons[i].gameObject.SetActive(true);
                answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = question.answers[i];
                answerButtons[i].onClick.RemoveAllListeners();

                if (i == question.correctAnswerIndex)
                {
                    answerButtons[i].onClick.AddListener(() => CorrectAnswer());
                }
                else
                {
                    answerButtons[i].onClick.AddListener(() => WrongAnswer());
                }
            }
            else
            {
                answerButtons[i].gameObject.SetActive(false);
            }
        }
    }

    void CorrectAnswer()
    {
        Debug.Log("Correct Answer!");
        questionPanel.SetActive(false);

        if (levelManager != null)
        {
            levelManager.ShowIndicatorArrows();
        }
    }

    void WrongAnswer()
    {
        Debug.Log("Wrong Answer! Try again.");
    }
}
