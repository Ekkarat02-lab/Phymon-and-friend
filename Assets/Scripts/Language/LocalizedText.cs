using UnityEngine;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
    public string key; 
    private Text textComponent;

    private void Start()
    {
        textComponent = GetComponent<Text>();
        UpdateText();
    }

    public void UpdateText()
    {
        if (LocalizationManager.Instance != null)
        {
            textComponent.text = LocalizationManager.Instance.GetLocalizedValue(key);
        }
    }

    private void OnEnable()
    {
        UpdateText(); 
    }
}