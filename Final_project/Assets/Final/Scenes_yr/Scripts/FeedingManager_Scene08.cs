using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Step6 씬 — 무서운 거미 먹이 주기 클리어 매니저.
/// FoodZoneTrigger_OVR_08 의 feedingManager 필드에 이 컴포넌트를 연결하세요.
/// 먹이(Tag="Food") → 거미집 트리거 진입 → OnFoodPlaced() → UnlockStage(nextStageNumber) → Step7 로드
/// </summary>
public class FeedingManager_Scene08 : MonoBehaviour
{
    [Header("필요한 먹이 개수")]
    public int             requiredFoodCount = 1;

    [Header("거미 Animator")]
    public Animator        spiderAnimator;

    [Header("HUD (VRSystem_Final 안의 GoalHUD 컴포넌트 연결)")]
    public GoalHUD         goalHUD;

    [Header("완료 UI (선택)")]
    public GameObject      completionPanel;
    public TextMeshProUGUI completionText;

    [Header("다음 씬 이름 (Build Settings 의 씬 이름과 정확히 일치)")]
    public string          nextSceneName    = "Step7";

    [Header("해금할 다음 스테이지 번호")]
    [SerializeField] public int nextStageNumber = 7;

    private int  fedCount    = 0;
    private bool isCompleted = false;

    void Start()
    {
        if (goalHUD != null)
            goalHUD.ShowGoal("무서운 거미에게도 먹이를 줘 보세요!");

        if (completionPanel != null)
            completionPanel.SetActive(false);
    }

    /// <summary>FoodZoneTrigger_OVR_08 에서 자동 호출됩니다.</summary>
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

        // Step6 클리어 → Inspector의 nextStageNumber 번호 해금
        ProgressManager.Instance.UnlockStage(nextStageNumber);

        Invoke(nameof(LoadNextScene), 3.5f);
    }

    void LoadNextScene() => SceneManager.LoadScene(nextSceneName);
}
