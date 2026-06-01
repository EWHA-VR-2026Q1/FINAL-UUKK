using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Step05_FeedCuteSpider (OVR) - 귀여운 거미 먹이 주기 클리어 매니저.
/// FoodZoneTrigger_OVR 의 feedingManager 필드에 이 컴포넌트를 연결하세요.
/// </summary>
public class FeedingManager_Scene07 : MonoBehaviour
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

    // ※ Build Settings의 씬 이름과 정확히 일치해야 합니다
    [Header("다음 씬 이름 (Build Settings 기준)")]
    public string nextSceneName = "Scene08_FeddScarySpider (OVR)";

    // Scene07 클리어 → Stage 8 해금
    [Header("해금할 다음 스테이지 번호")]
    public int nextStageNumber = 8;

    private int fedCount = 0;
    private bool isCompleted = false;

    void Start()
    {
        string spiderName = PlayerPrefs.GetString("SpiderName", "거미");
        if (goalHUD != null)
            goalHUD.ShowGoal(spiderName + "에게 먹이를 줘보세요!");

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
            goalHUD.ShowGoal(spiderName + "이(가) 맛있게 먹네요!");

        if (completionPanel != null)
        {
            completionPanel.SetActive(true);
            if (completionText != null)
                completionText.text = spiderName + " 냠냠 :)";
        }

        // 현재 스테이지 클리어 → 다음 스테이지 해금
        ProgressManager.Instance.UnlockStage(nextStageNumber);

        Invoke(nameof(LoadNextScene), 3.5f);
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
