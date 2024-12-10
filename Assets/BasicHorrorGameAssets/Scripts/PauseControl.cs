using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseControl : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseMenuUI;    // Panel Pause Menu
    public Button resumeButton;       // Tombol Resume
    public Button quitButton;         // Tombol Quit

    [Header("Game Object References")]
    public GameObject player;         // GameObject Player
    public GameObject enemy;          // GameObject Musuh

    [Header("Pause State")]
    private bool isPaused = false;    // Status pause

    private void Awake()
    {
        // Ensure references are set up early
        SetupButtonListeners();
    }

    private void Start()
    {
        // Ensure pause menu is hidden at game start
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);
        else
            Debug.LogError("Pause Menu UI is not assigned!");
    }

    private void Update()
    {
        // Check for Escape key to toggle pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
                PauseGame();
            else
                ResumeGame(); // Close pause menu and resume game when escape is pressed again
        }
    }

    private void SetupButtonListeners()
    {
        // Manually find buttons if not assigned in inspector
        if (resumeButton == null)
            resumeButton = GameObject.Find("ResumeButton")?.GetComponent<Button>();

        if (quitButton == null)
            quitButton = GameObject.Find("QuitButton")?.GetComponent<Button>();

        // Add listeners with null checks
        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(ResumeGame);
        }
        else
            Debug.LogError("Resume Button is not assigned!");

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }
        else
            Debug.LogError("Quit Button is not assigned!");
    }

    private void PauseGame()
    {
        Debug.Log("Game Paused");

        // Show cursor when the game is paused
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Save game state
        SaveGameState();

        // Show pause menu
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);

        // Freeze game time
        Time.timeScale = 0f;
        isPaused = true;
    }


    // Function to save the game state
    private void SaveGameState()
    {
        try
        {
            // Save player position
            if (player != null)
            {
                PlayerPrefs.SetFloat("PlayerPositionX", player.transform.position.x);
                PlayerPrefs.SetFloat("PlayerPositionY", player.transform.position.y);
                PlayerPrefs.SetFloat("PlayerPositionZ", player.transform.position.z);
            }

            // Save enemy position
            if (enemy != null)
            {
                PlayerPrefs.SetFloat("EnemyPositionX", enemy.transform.position.x);
                PlayerPrefs.SetFloat("EnemyPositionY", enemy.transform.position.y);
                PlayerPrefs.SetFloat("EnemyPositionZ", enemy.transform.position.z);
            }

            PlayerPrefs.Save();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error saving game state: " + e.Message);
        }
    }

    // Function to resume the game
    public void ResumeGame()
    {
        Debug.Log("Resuming Game");

        // Hide cursor when the game resumes
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Load game state
        LoadGameState();

        // Hide pause menu
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        // Resume the game time
        Time.timeScale = 1f;
        isPaused = false;
    }


    // Function to quit the game and go to the Main Menu
    public void QuitGame()
    {
        Debug.Log("Quitting Game");

        // Ensure time scale is back to normal before quitting
        Time.timeScale = 1f;

        try
        {
            // Load the Main Menu scene (ensure "MainMenu" is set in Build Settings)
            SceneManager.LoadScene("MainMenu");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error loading Main Menu: " + e.Message);
        }
    }

    // Function to restore the game state
    private void LoadGameState()
    {
        try
        {
            // Restore player position
            if (player != null)
            {
                float playerX = PlayerPrefs.GetFloat("PlayerPositionX");
                float playerY = PlayerPrefs.GetFloat("PlayerPositionY");
                float playerZ = PlayerPrefs.GetFloat("PlayerPositionZ");
                player.transform.position = new Vector3(playerX, playerY, playerZ);
            }

            // Restore enemy position
            if (enemy != null)
            {
                float enemyX = PlayerPrefs.GetFloat("EnemyPositionX");
                float enemyY = PlayerPrefs.GetFloat("EnemyPositionY");
                float enemyZ = PlayerPrefs.GetFloat("EnemyPositionZ");
                enemy.transform.position = new Vector3(enemyX, enemyY, enemyZ);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error loading game state: " + e.Message);
        }
    }
}
