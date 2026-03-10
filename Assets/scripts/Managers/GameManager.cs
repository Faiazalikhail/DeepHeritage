using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

[RequireComponent(typeof(AudioSource))]
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
    private GameObject pauseMenuPanel;
    public bool isPaused = false;

    [Header("Audio Settings")]
    public AudioClip titleMusic;
    public AudioClip gameMusic;
    public AudioClip gameOverMusic;
    public AudioClip pauseSound; 
    private AudioSource bgmSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            bgmSource = GetComponent<AudioSource>();
            bgmSource.loop = true;
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

    void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1f;
        isPaused = false;

        if (scene.name == titleScene)
        {
            PlayMusic(titleMusic);
        }
        else if (scene.name == mainScene)
        {
            PlayMusic(gameMusic);

            GameObject textObj = GameObject.Find("Text-Lives");
            if (textObj != null) livesText = textObj.GetComponent<TextMeshProUGUI>();

            GameObject pausePanel = GameObject.Find("PauseMenuPanel");
            if (pausePanel != null)
            {
                pauseMenuPanel = pausePanel;
                pauseMenuPanel.SetActive(false);
            }

            UpdateLivesUI();
        }
        else if (scene.name == gameOverScene)
        {
            if (bgmSource != null)
            {
                bgmSource.Stop();
                bgmSource.loop = false;
                if (gameOverMusic != null) bgmSource.PlayOneShot(gameOverMusic);
            }
        }
    }

    private void PlayMusic(AudioClip clip)
    {
        if (clip == null || bgmSource == null) return;
        if (bgmSource.clip == clip) return;

        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == gameOverScene && Input.GetKeyDown(KeyCode.Escape)) ReturnToTitle();
        if (SceneManager.GetActiveScene().name == mainScene && Input.GetKeyDown(KeyCode.P)) TogglePause();
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        // Play Pause Sound
        if (pauseSound != null) AudioSource.PlayClipAtPoint(pauseSound, Camera.main.transform.position);

        if (isPaused)
        {
            Time.timeScale = 0f;
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
            if (bgmSource != null) bgmSource.Pause();
        }
        else
        {
            Time.timeScale = 1f;
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
            if (bgmSource != null) bgmSource.UnPause();
        }
    }

    public void LoseLife()
    {
        lives--;
        UpdateLivesUI();
    }

    public void TriggerGameOver() { SceneManager.LoadScene(gameOverScene); }

    void UpdateLivesUI()
    {
        if (livesText == null)
        {
            GameObject textObj = GameObject.Find("Text-Lives");
            if (textObj != null) livesText = textObj.GetComponent<TextMeshProUGUI>();
        }

        if (livesText != null) livesText.text = "Lives: " + lives;
        else Debug.LogWarning("GameManager cannot find Text-Lives!");
    }

    public void StartGame()
    {
        lives = 3;
        SceneManager.LoadScene(mainScene);
    }

    public void ReturnToTitle() { SceneManager.LoadScene(titleScene); }
}