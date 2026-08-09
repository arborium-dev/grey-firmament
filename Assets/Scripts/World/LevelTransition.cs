using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransition : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player has entered the level transition zone.");

            GlobalVars.PlayerLevel++;
            Time.timeScale = 1;
            SceneManager.LoadScene("Item Selector");
        }
    }
}
