using UnityEngine;

public class MenuController : MonoBehaviour
{
    public void OnStartGamePressed()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.StartGame();
    }

    public void OnResumePressed()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.TogglePause();
    }

    public void OnQuitPressed()
    {
        Debug.Log("Game Quit!");

        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // If this is a compiled build (.exe), close the application completely
        Application.Quit();
#endif
    }
}