using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Scene09_CleanSpiderHouse (OVR) — 사육장 세팅 복합 조건 클리어 매니저.
///
/// ─ 클리어 순서 ──────────────────────────────────────────────
///  STEP 1. 거미 → 임시 박스 안으로 드래그  (TempBoxReceiver_OVR 호출)
///  STEP 2. 행주로 거미집 닦기              (TerrariumCleaner 자동 감지)
///  STEP 3. 토양 / 나무 / 이끼 3종 배치     (PlaceableItem 각각 콜백)
///  STEP 4. 위 3가지 모두 완료 → 임시 박스 OVR 레이 클릭  (TempBoxClickable_OVR 호출)
///           → ProgressManager.UnlockStage(nextStageNumber)
/// ────────────────────────────────────────────────────────────
///
/// Inspector 연결 체크리스트 (아래 [요청 2] 가이드 참고):
///   spiderObject       거미 오브젝트
///   terrariumCleaner   거미집의 TerrariumCleaner 컴포넌트
///   soilItem           토양 PlaceableItem
///   woodItem           나무 PlaceableItem
///   mossItem           이끼 PlaceableItem
///   tempBoxClickable   임시 박스의 TempBoxClickable_OVR 컴포넌트
///   goalHUD            VR Final > HUD 오브젝트의 GoalHUD 컴포넌트
///   subText            보조 안내 TMP 텍스트 (없으면 비워도 됨)
///   nextStageNumber    기본값 10 (다음 스테이지 번호)
///   nextSceneName      Build Settings 기준 다음 씬 이름
/// </summary>
public class Scene09_CleanManager : MonoBehaviour
{
    // ── 씬 오브젝트 ────────────────────────────────────────────
    [Header("거미 오브젝트")]
    public GameObject spiderObject;

    [Header("거미집 청소 컴포넌트")]
    public TerrariumCleaner terrariumCleaner;

    [Header("배치 아이템 (PlaceableItem 컴포넌트 부착 필수)")]
    public PlaceableItem soilItem;
    public PlaceableItem woodItem;
    public PlaceableItem mossItem;

    [Header("임시 박스 OVR 클릭 컴포넌트")]
    public TempBoxClickable_OVR tempBoxClickable;

    // ── UI ─────────────────────────────────────────────────────
    [Header("HUD — VR Final 프리팹 > HUD 오브젝트의 GoalHUD 컴포넌트 드래그")]
    public GoalHUD goalHUD;

    [Header("보조 안내 텍스트 (선택)")]
    public TextMeshProUGUI subText;

    // ── 씬 전환 / 스테이지 ─────────────────────────────────────
    [Header("다음 스테이지 번호 (기본 10)")]
    public int nextStageNumber = 10;

    // ※ Build Settings의 씬 이름과 정확히 일치해야 합니다
    [Header("다음 씬 이름 (Build Settings 기준)")]
    public string nextSceneName = "Scene10_FindSpider (OVR)";

    // ── 내부 상태 ──────────────────────────────────────────────
    private bool spiderMoved      = false;
    private bool cleaningDone     = false;   // Update에서 TerrariumCleaner 폴링
    private int  decorateCount    = 0;
    private bool isCleared        = false;

    // ── 생명주기 ───────────────────────────────────────────────
    void Start()
    {
        string n = PlayerPrefs.GetString("SpiderName", "거미");

        if (goalHUD != null)
            goalHUD.ShowGoal(n + "의 집을 새로 꾸며 주세요!");

        SetSub("STEP 1 : " + n + "를 임시 박스로 옮겨주세요.");

        RegisterPlaceCallback(soilItem);
        RegisterPlaceCallback(woodItem);
        RegisterPlaceCallback(mossItem);
    }

    void Update()
    {
        // TerrariumCleaner는 내부에서 isCleaned 플래그만 바꾸므로 폴링으로 감지
        if (!cleaningDone && spiderMoved
            && terrariumCleaner != null && terrariumCleaner.isCleaned)
        {
            cleaningDone = true;
            SetSub("STEP 3 : 토양 · 나무 · 이끼를 거미집에 넣어주세요.");
            CheckAllConditions();
        }
    }

    // ── 외부 콜백 ──────────────────────────────────────────────

    /// <summary>TempBoxReceiver_OVR → sceneManager09Clean 필드에 이 컴포넌트 연결.</summary>
    public void OnSpiderInBox(GameObject spider)
    {
        if (spiderMoved) return;
        spiderMoved = true;
        SetSub("STEP 2 : 행주로 거미집을 닦아주세요.");
        CheckAllConditions();
    }

    /// <summary>PlaceableItem 의 PlaceTracker 콜백.</summary>
    public void OnDecorateItemPlaced()
    {
        decorateCount++;
        SetSub($"아이템 배치 : {decorateCount} / 3");
        CheckAllConditions();
    }

    /// <summary>TempBoxClickable_OVR 에서 OVR 레이 클릭 시 호출.</summary>
    public void OnTempBoxClicked()
    {
        if (isCleared) return;
        if (!AreAllConditionsMet())
        {
            SetSub(GetRemainingHint());
            return;
        }
        TriggerClear();
    }

    // ── 내부 로직 ──────────────────────────────────────────────

    void RegisterPlaceCallback(PlaceableItem item)
    {
        if (item == null) return;
        PlaceTracker tracker = item.gameObject.AddComponent<PlaceTracker>();
        tracker.onPlaced = OnDecorateItemPlaced;
    }

    bool AreAllConditionsMet() =>
        spiderMoved && cleaningDone && decorateCount >= 3;

    void CheckAllConditions()
    {
        if (isCleared || !AreAllConditionsMet()) return;

        string n = PlayerPrefs.GetString("SpiderName", "거미");
        SetSub("모든 준비 완료! 임시 박스를 클릭하세요.");

        if (goalHUD != null)
            goalHUD.ShowGoal("임시 박스를 눌러 " + n + "를 돌려보내세요!");

        if (tempBoxClickable != null)
            tempBoxClickable.EnableClick();
    }

    void TriggerClear()
    {
        isCleared = true;
        string n = PlayerPrefs.GetString("SpiderName", "거미");

        if (goalHUD != null)
            goalHUD.ShowGoal(n + "의 새 집이 완성되었어요!");

        SetSub("완료! 잘 했어요 :)");

        // Scene09 클리어 → 다음 스테이지 해금
        ProgressManager.Instance.UnlockStage(nextStageNumber);

        Invoke(nameof(LoadNextScene), 4.0f);
    }

    void LoadNextScene() => SceneManager.LoadScene(nextSceneName);

    void SetSub(string text)
    {
        if (subText != null) subText.text = text;
        Debug.Log("[Scene09_Clean] " + text);
    }

    string GetRemainingHint()
    {
        if (!spiderMoved)   return "먼저 거미를 임시 박스로 옮겨주세요.";
        if (!cleaningDone)  return "행주로 거미집을 닦아야 해요.";
        if (decorateCount < 3) return $"아이템이 아직 부족해요 ({decorateCount}/3).";
        return "";
    }
}
