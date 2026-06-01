using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
<<<<<<< HEAD
/// Scene08_FeddScarySpider (OVR) — 무서운 거미 먹이 주기 클리어 매니저.
///
/// 연동 스크립트: FoodZoneTrigger_OVR (먹이 존 트리거)
///   → FoodZoneTrigger_OVR.feedingManager 필드에 이 컴포넌트를 연결하세요.
///   (기존 FoodZoneTrigger_OVR는 FeedingManager_Scene07을 참조하므로,
///    Scene08 씬에서는 FoodZoneTrigger_OVR를 복사해 feedingManager 타입을
///    이 클래스로 바꾸거나, 아래 FoodZoneTrigger_OVR_08 을 사용하세요.)
=======
/// Step06_FeddScarySpider (OVR) - 무서운 거미 먹이 주기 클리어 매니저.
/// FoodZoneTrigger_OVR_08 의 feedingManager 필드에 이 컴포넌트를 연결하세요.
>>>>>>> origin/kyr
/// </summary>
public class FeedingManager_Scene08 : MonoBehaviour
{
    [Header("필요한 먹이 개수")]
    public int requiredFoodCount = 1;

    [Header("거미 Animator")]
    public Animator spiderAnimator;

<<<<<<< HEAD
    [Header("HUD — VR Final 프리팹 > HUD 오브젝트의 GoalHUD 컴포넌트 드래그")]
=======
    [Header("HUD (VR Final > HUD 안의 GoalHUD 컴포넌트 연결)")]
>>>>>>> origin/kyr
    public GoalHUD goalHUD;

    [Header("완료 UI (선택)")]
    public GameObject completionPanel;
    public TextMeshProUGUI completionText;

<<<<<<< HEAD
    // ※ Build Settings의 씬 이름과 정확히 일치해야 합니다
    [Header("다음 씬 이름 (Build Settings 기준)")]
    public string nextSceneName = "Scene09_CleanSpiderHouse (OVR)";
=======
    [Header("다음 씬 이름 (Build Settings 기준)")]
    public string nextSceneName = "Step07_CleanSpiderHouse (OVR)";

    [Header("해금할 다음 스테이지 번호")]
    [SerializeField] public int nextStageNumber = 9;
>>>>>>> origin/kyr

    private int  fedCount    = 0;
    private bool isCompleted = false;

    void Start()
    {
<<<<<<< HEAD
        string spiderName = PlayerPrefs.GetString("SpiderName", "거미");

=======
>>>>>>> origin/kyr
        if (goalHUD != null)
            goalHUD.ShowGoal("무서운 거미에게도 먹이를 줘 보세요!");

        if (completionPanel != null)
            completionPanel.SetActive(false);
    }

<<<<<<< HEAD
    /// <summary>FoodZoneTrigger_OVR_08 에서 호출합니다.</summary>
=======
>>>>>>> origin/kyr
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
<<<<<<< HEAD
        string spiderName = PlayerPrefs.GetString("SpiderName", "거미");
=======
>>>>>>> origin/kyr

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

<<<<<<< HEAD
        // Scene08 클리어 → Stage 9 해금
        ProgressManager.Instance.UnlockStage(9);
=======
        // Step06 클리어 → Stage 9 해금
        ProgressManager.Instance.UnlockStage(nextStageNumber);
>>>>>>> origin/kyr

        Invoke(nameof(LoadNextScene), 3.5f);
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
