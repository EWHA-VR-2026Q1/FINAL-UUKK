using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeToSafeZone : MonoBehaviour
{
    public string targetScene = "SafeZone";

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(targetScene);
        }
    }
}