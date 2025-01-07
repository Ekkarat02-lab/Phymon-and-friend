using UnityEngine;

public class LanguageSwitcher : MonoBehaviour
{
    public void SwitchToEnglish()
    {
        LocalizationManager.Instance.LoadLanguage("en");
        LocalizationManager.Instance.SaveLanguagePreference("en");
        UpdateAllTexts();
    }

    public void SwitchToThai()
    {
        LocalizationManager.Instance.LoadLanguage("th");
        LocalizationManager.Instance.SaveLanguagePreference("th");
        UpdateAllTexts();
    }

    private void UpdateAllTexts()
    {
        foreach (var localizedText in FindObjectsOfType<LocalizedText>())
        {
            localizedText.UpdateText();
        }
    }
}