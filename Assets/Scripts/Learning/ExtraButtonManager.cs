using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExtraButtonManager : MonoBehaviour
{
    public void LoadExtra(int extraID)
    {
        string sceneName = "Extra " + extraID;

        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError($"Scene {sceneName} not found in Build Settings!");
        }
    }
}
