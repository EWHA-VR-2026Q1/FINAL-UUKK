using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorTrigger : MonoBehaviour
{
    public string targetSceneName;

    private bool playerNear;

    private void Update()
    {
        if (playerNear && Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadScene(targetSceneName);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        { //Player 오브젝트에 Player 태그 추가 필요
            playerNear = true;
            Debug.Log("Press E to enter");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = false;
        }
    }
}