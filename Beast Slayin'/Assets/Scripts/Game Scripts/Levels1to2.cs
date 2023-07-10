using UnityEngine;
using UnityEngine.SceneManagement;

public class Levels1to2 : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        SceneManager.LoadScene("Level2");
    }
}