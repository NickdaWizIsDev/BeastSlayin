using UnityEngine;
using UnityEngine.SceneManagement;

public class Levels2to3 : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        SceneManager.LoadScene("Level3");
    }
}