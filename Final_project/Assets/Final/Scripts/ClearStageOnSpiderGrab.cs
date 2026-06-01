using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearStageOnSpiderGrab : MonoBehaviour
{
    [Header("Progress")]
    [SerializeField] private int nextStageNumber = 1;

    [Header("Scene Load")]
    [SerializeField] private bool loadSceneAfterClear = false;
    [SerializeField] private string nextSceneName = "";

    private bool cleared = false;

    public void OnGrabbed()
    {
        if (cleared) return;
        cleared = true;

        if (ProgressManager.Instance == null)
        {
            Debug.LogWarning("[ClearStageOnSpiderGrab] ProgressManager is not ready.", this);
            return;
        }

        ProgressManager.Instance.UnlockStage(nextStageNumber);

        if (loadSceneAfterClear && !string.IsNullOrWhiteSpace(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
