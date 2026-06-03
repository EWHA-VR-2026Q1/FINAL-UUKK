using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene08_Manager : MonoBehaviour
{
    [Header("HUD")]
    public GoalHUD goalHUD;
    public string defaultSpiderName = "Spider";

    public int nextStageNumber = 9;
    public bool loadSceneAfterClear = true;
    public string nextSceneName = "Step9";
    public float loadDelaySeconds = 1f;

    private bool cleared = false;

    private void Start()
    {
        if (goalHUD == null)
            goalHUD = FindObjectOfType<GoalHUD>(true);

        string spiderName = PlayerPrefs.GetString("SpiderName", defaultSpiderName);
        ShowGoal("Find the " + spiderName + " and put it back in the terrarium.");
    }

    public void OnSpiderInHouse(GameObject spider)
    {
        if (cleared) return;
        cleared = true;

        Debug.Log("[Scene08] Spider is in the house. Stage clear!");

        ShowGoal("The spider is back in the terrarium.");

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

    private void ShowGoal(string message)
    {
        if (goalHUD == null)
            goalHUD = FindObjectOfType<GoalHUD>(true);

        if (goalHUD != null)
        {
            goalHUD.gameObject.SetActive(true);
            goalHUD.ShowGoal(message);
        }
        else
        {
            Debug.LogWarning("[Scene08] GoalHUD was not found.", this);
        }
    }
}
