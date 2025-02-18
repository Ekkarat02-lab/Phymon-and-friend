using UnityEngine;

public class Mascot : MonoBehaviour
{
    public LevelManager levelManager;
    public int mascotIndex;

    private void OnMouseDown()
    {
        if (gameObject.CompareTag("Mascot")) 
        {
            levelManager.FoundMascot(mascotIndex);
            GameManager.Instance.HideMascotOnUI(mascotIndex);
            Destroy(gameObject);
        }
    }
}