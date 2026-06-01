using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Scene 06 (무서운 거미 먹이 주기) 클리어 매니저.
/// FoodZoneTrigger_OVR / FoodZoneTrigger_XR 에서 OnFoodPlaced() 를 호출합니다.
/// Inspector에서 feedingManager 필드에 이 컴포넌트를 연결하세요.
/// </summary>
public class FeedingManager_Scene08 : MonoBehaviour
{
    [Header("필요한 먹이 개수")]
    public int requiredFoodCount = 1;

    [Header("거미 Animator")]
    public Animator spiderAnimator;

    [Header("HUD (VR Final > HUD 안의 GoalHUD 컴포넌트 연결)")]
    public GoalHUD goalHUD;

    [Header("완료 UI")]
    public GameObject completionPanel;
    public TextMeshProUGUI completionText;

    [Header("다음 씬")]
    public string nextSceneName = "Scene09_CleanTerrarium";

    private int fedCount = 0;
    private bool isCompleted = false;

    void Start()
    {
        string spiderName = PlayerPrefs.GetString("SpiderName", "거미");
        if (goalHUD != null)
            goalHUD.ShowGoal("무서운 거미에게도 먹이를 줘보세요!");

        if (completionPanel != null)
            completionPanel.SetActive(false);
    }

    public void OnFoodPlaced()
    {
        if (isCompleted) return;
        fedCount++;

        if (fedCount >= requiredFoodCount)
            OnFeedingComplete();
    }

    void OnFeedingComplete()
    {
        isCompleted = true;
        string spiderName = PlayerPrefs.GetString("SpiderName", "거미");

        if (spiderAnimator != null)
            spiderAnimator.SetTrigger("Eat");

        if (goalHUD != null)
            goalHUD.ShowGoal("잘 먹는군요! 이제 집을 청소해 봐요.");

        if (completionPanel != null)
        {
            completionPanel.SetActive(true);
            if (completionText != null)
                completionText.text = "먹이 주기 완료!";
        }

        // Stage 6 클리어 → Stage 7 해금
        ProgressManager.Instance.UnlockStage(7);

        Invoke(nameof(LoadNextScene), 3.5f);
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
