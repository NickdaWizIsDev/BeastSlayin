using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public Canvas pauseCanvas;
    public Canvas options;
    public GameObject continueButton;
    public GameObject optionsButton;
    public GameObject mainMenuButton;

    private bool isPaused = false;

    private void Start()
    {
        // Initially hide the pause menu
        pauseCanvas.gameObject.SetActive(false);
    }

    private void Update()
    {
        // Check for the Escape key press
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Toggle the pause state
            isPaused = !isPaused;

            // If the game is paused, show the pause menu
            if (isPaused)
            {
                Time.timeScale = 0f;  // Pause the game
                pauseCanvas.gameObject.SetActive(true);
            }
            else
            {
                // Resume the game
                Time.timeScale = 1f;
                pauseCanvas.gameObject.SetActive(false);
            }
        }
    }

    public void Continue()
    {
        // Resume the game
        Time.timeScale = 1f;
        pauseCanvas.gameObject.SetActive(false);
        isPaused = false;
    }

    public void Options()
    {
        pauseCanvas.gameObject.SetActive(false);
        options.gameObject.SetActive(true);
    }

    public void MainMenu()
    {
        // Load the main menu scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
