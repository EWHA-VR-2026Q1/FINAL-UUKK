using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingAutoReturn : MonoBehaviour
{
    [SerializeField] private float delaySeconds = 30f;
    [SerializeField] private string targetSceneName = "Entry";

    private void Start()
    {
        StartCoroutine(ReturnAfterDelay());
    }

    private IEnumerator ReturnAfterDelay()
    {
        yield return new WaitForSeconds(delaySeconds);

        if (string.IsNullOrWhiteSpace(targetSceneName))
        {
            Debug.LogWarning("[EndingAutoReturn] Target scene name is empty.", this);
            yield break;
        }

        SceneManager.LoadScene(targetSceneName);
    }
}
