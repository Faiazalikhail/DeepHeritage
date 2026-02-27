using UnityEngine;

public class MenuController : MonoBehaviour
{
    public void OnStartGamePressed()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.StartGame();
    }

    public void OnQuitPressed()
    {
        Application.Quit();
    }
}