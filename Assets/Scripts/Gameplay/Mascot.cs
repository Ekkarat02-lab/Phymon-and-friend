using UnityEngine;

public class Mascot : MonoBehaviour
{
    public LevelManager levelManager;
    public int mascotIndex;

    private void OnMouseDown()
    {
        if (gameObject.CompareTag("Mascot")) 
        {
            levelManager.FoundMascot(mascotIndex); // แจ้งว่า Mascot ถูกพบ
            GameManager.Instance.HideMascotOnUI(mascotIndex); // ซ่อน UI ของ Mascot ที่ถูกพบ
            Destroy(gameObject); // ทำลาย Mascot
        }
    }
}