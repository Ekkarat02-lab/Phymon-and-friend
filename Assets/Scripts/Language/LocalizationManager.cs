using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class LocalizationData
{
    public LocalizationItem[] items;
}

[System.Serializable]
public class LocalizationItem
{
    public string key;
    public string value;
}

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance;
    private Dictionary<string, string> localizedText;
    private string currentLanguage = "en";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        LoadSavedLanguage(); // โหลดค่าภาษาที่บันทึกไว้
    }

    public void LoadLanguage(string languageCode)
    {
        currentLanguage = languageCode;
        string filePath = Path.Combine(Application.streamingAssetsPath, $"{languageCode}.json");

        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

            localizedText = new Dictionary<string, string>();
            foreach (var item in loadedData.items)
            {
                localizedText[item.key] = item.value;
            }

            Debug.Log($"Language '{languageCode}' loaded successfully.");
        }
        else
        {
            Debug.LogError($"Localization file not found: {filePath}");
        }
    }

    public string GetLocalizedValue(string key)
    {
        if (localizedText == null || !localizedText.ContainsKey(key))
        {
            Debug.LogWarning($"Key '{key}' not found in localization data.");
            return key; // คืนค่า key เดิมในกรณีที่ไม่มีข้อความ
        }
        return localizedText[key];
    }

    public void SaveLanguagePreference(string languageCode)
    {
        PlayerPrefs.SetString("SelectedLanguage", languageCode);
        PlayerPrefs.Save();
    }

    public void LoadSavedLanguage()
    {
        string savedLanguage = PlayerPrefs.GetString("SelectedLanguage", "en");
        LoadLanguage(savedLanguage);
    }
}