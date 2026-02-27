using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Lives")]
    public int lives = 3;

    [Header("Scenes")]
    public string titleScene = "TitleLevel";
    public string mainScene = "MainLevel";
    public string gameOverScene = "GameOver";

    private TextMeshProUGUI livesText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (SceneManager.GetActiveScene().name != mainScene && SceneManager.GetActiveScene().name != gameOverScene)
        {
            SceneManager.LoadScene(titleScene);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == mainScene)
        {
            GameObject textObj = GameObject.Find("Text-Lives");
            if (textObj != null)
                livesText = textObj.GetComponent<TextMeshProUGUI>();

            UpdateLivesUI();
        }
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == gameOverScene)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ReturnToTitle();
            }
        }
    }

    public void LoseLife()
    {
        lives--;
        UpdateLivesUI();
        
    }

    // Mario will call this exactly 2 seconds after he dies
    public void TriggerGameOver()
    {
        SceneManager.LoadScene(gameOverScene);
    }

    void UpdateLivesUI()
    {
        
        if (livesText == null)
        {
            GameObject textObj = GameObject.Find("Text-Lives");
            if (textObj != null)
            {
                livesText = textObj.GetComponent<TextMeshProUGUI>();
            }
        }

        // Now update the text!
        if (livesText != null)
        {
            livesText.text = "Lives: " + lives;
        }
        else
        {
            // If it STILL can't find it, it will tell us in the console
            Debug.LogWarning("GameManager cannot find Text-Lives!");
        }
    }

    public void StartGame()
    {
        lives = 3;
        SceneManager.LoadScene(mainScene);
    }

    public void ReturnToTitle()
    {
        SceneManager.LoadScene(titleScene);
    }
}