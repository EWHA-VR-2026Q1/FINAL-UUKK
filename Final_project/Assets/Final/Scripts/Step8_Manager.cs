using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene08_Manager : MonoBehaviour
{
    public int nextStageNumber = 9;
    public bool loadSceneAfterClear = true;
    public string nextSceneName = "Step9";
    public float loadDelaySeconds = 1f;

    private bool cleared = false;

    public void OnSpiderInHouse(GameObject spider)
    {
        if (cleared) return;
        cleared = true;

        Debug.Log("[Scene08] Spider is in the house. Stage clear!");

        ProgressManager.Instance.UnlockStage(nextStageNumber);

        if (loadSceneAfterClear && !string.IsNullOrWhiteSpace(nextSceneName))
        {
            Invoke(nameof(LoadNextScene), loadDelaySeconds);
        }
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
