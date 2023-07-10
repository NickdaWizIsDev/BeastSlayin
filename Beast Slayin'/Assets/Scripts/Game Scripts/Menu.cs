using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public Button levels;
    public Button options;
    public Button quit;
    public GameObject mainMenu;
    public GameObject levelList;

    private void Start()
    {
        Time.timeScale = 1f;
        mainMenu.SetActive(true);
    }

    public void LevelList()
    {
        mainMenu.SetActive(false);
        levelList.SetActive(true);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
