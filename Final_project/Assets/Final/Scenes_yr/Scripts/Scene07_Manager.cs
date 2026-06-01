using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Scene 07 (거미 사육장 세팅) 클리어 매니저.
///
/// 클리어 조건 (모두 충족 후 임시 박스 클릭):
///   1. 거미를 임시 박스 안으로 이동 (TempBoxReceiver_OVR/XR → OnSpiderInBox 호출)
///   2. 행주로 거미집 닦기 (TerrariumCleaner.isCleaned == true)
///   3. 토양/나무/이끼 3종 배치 (PlaceableItem 각각 → OnDecorateItemPlaced 호출)
///   4. 위 조건 모두 충족된 상태에서 임시 박스 클릭 (TempBoxClickable → OnTempBoxClicked 호출)
///
/// Inspector 연결 체크리스트:
///   - spiderObject          : 씬 안의 거미 오브젝트
///   - terrariumCleaner      : 거미집에 부착된 TerrariumCleaner 컴포넌트
///   - soilItem / woodItem / mossItem : PlaceableItem 컴포넌트가 붙은 각 에셋
///   - tempBoxClickable      : 임시 박스에 부착된 TempBoxClickable 컴포넌트
///   - goalHUD               : VR Final > HUD 안의 GoalHUD 컴포넌트
///   - subText (선택)        : 세부 안내 TMP 텍스트
///   - nextSceneName         : 클리어 후 로드할 씬 이름
/// </summary>
public class Scene07_Manager : MonoBehaviour
{
    // ── 씬 오브젝트 참조 ───────────────────────────────────────────────
    [Header("거미 오브젝트")]
    public GameObject spiderObject;

    [Header("거미집 청소 컴포넌트")]
    public TerrariumCleaner terrariumCleaner;

    [Header("배치 아이템 (PlaceableItem 컴포넌트 필요)")]
    public PlaceableItem soilItem;
    public PlaceableItem woodItem;
    public PlaceableItem mossItem;

    [Header("임시 박스 클릭 컴포넌트")]
    public TempBoxClickable tempBoxClickable;

    // ── UI ────────────────────────────────────────────────────────────
    [Header("HUD (VR Final > HUD 안의 GoalHUD 컴포넌트 연결)")]
    public GoalHUD goalHUD;

    [Header("세부 안내 텍스트 (선택 — HUD 아래 서브 텍스트)")]
    public TextMeshProUGUI subText;

    // ── 씬 전환 ───────────────────────────────────────────────────────
    [Header("다음 씬 이름")]
    public string nextSceneName = "Scene10_FindSpider";

    // ── 내부 상태 ─────────────────────────────────────────────────────
    private bool spiderMoved = false;
    private int decorateCount = 0;
    private bool isCleared = false;

    // ── 생명주기 ──────────────────────────────────────────────────────

    void Start()
    {
        string spiderName = PlayerPrefs.GetString("SpiderName", "Spider");

        if (goalHUD != null)
            goalHUD.ShowGoal("Set up " + spiderName + "'s new home.");

        SetSubText("Move " + spiderName + " into the temp box.");

        // PlaceableItem에 콜백 등록
        RegisterPlaceCallback(soilItem);
        RegisterPlaceCallback(woodItem);
        RegisterPlaceCallback(mossItem);
    }

    void Update()
    {
        // 청소 완료 감지 (TerrariumCleaner는 내부에서 isCleaned만 바꾸므로 폴링)
        if (spiderMoved && terrariumCleaner != null && terrariumCleaner.isCleaned)
            OnCleaningConfirmed();
    }

    // ── 외부에서 호출되는 이벤트 ──────────────────────────────────────

    /// <summary>
    /// TempBoxReceiver_OVR 또는 TempBoxReceiver_XR 에서 거미가 박스에 들어오면 호출.
    /// 기존 TempBoxReceiver가 sceneManager로 Scene09_Manager를 참조하고 있다면,
    /// 이 Scene07_Manager를 참조하도록 Inspector에서 바꿔주세요.
    /// </summary>
    public void OnSpiderInBox(GameObject spider)
    {
        if (spiderMoved) return;
        spiderMoved = true;

        SetSubText("Clean the spider house with the rag.");

        CheckAllConditions();
    }

    /// <summary>PlaceableItem 이 거미집 안에 배치되었을 때 호출됩니다.</summary>
    public void OnDecorateItemPlaced()
    {
        decorateCount++;
        SetSubText($"Items placed: {decorateCount} / 3");
        Debug.Log($"[Scene07] Item placed: {decorateCount}/3");

        CheckAllConditions();
    }

    /// <summary>TempBoxClickable 에서 박스가 클릭되면 호출됩니다.</summary>
    public void OnTempBoxClicked()
    {
        if (isCleared) return;
        if (!AreAllConditionsMet())
        {
            // 아직 조건 미충족 — 힌트 표시
            SetSubText(GetRemainingHint());
            return;
        }

        TriggerClear();
    }

    // ── 내부 로직 ─────────────────────────────────────────────────────

    void RegisterPlaceCallback(PlaceableItem item)
    {
        if (item == null) return;
        PlaceTracker tracker = item.gameObject.AddComponent<PlaceTracker>();
        tracker.onPlaced = OnDecorateItemPlaced;
    }

    /// <summary>청소 완료를 한 번만 처리하기 위한 플래그 및 Update 감지 로직.</summary>
    private bool cleaningConfirmed = false;
    void OnCleaningConfirmed()
    {
        if (cleaningConfirmed) return;
        cleaningConfirmed = true;

        SetSubText("Place the soil, branch, and moss in the spider house.");

        CheckAllConditions();
    }

    bool AreAllConditionsMet()
    {
        bool cleaned = terrariumCleaner != null && terrariumCleaner.isCleaned;
        return spiderMoved && cleaned && decorateCount >= 3;
    }

    void CheckAllConditions()
    {
        if (isCleared) return;
        if (!AreAllConditionsMet()) return;

        // 모든 조건 충족 → 임시 박스 클릭 가능 상태로 전환
        string spiderName = PlayerPrefs.GetString("SpiderName", "Spider");
        SetSubText("Everything is ready. Click the temp box.");

        if (goalHUD != null)
            goalHUD.ShowGoal("Click the temp box to return " + spiderName + ".");

        if (tempBoxClickable != null)
            tempBoxClickable.EnableClick();
    }

    void TriggerClear()
    {
        isCleared = true;

        string spiderName = PlayerPrefs.GetString("SpiderName", "Spider");

        if (goalHUD != null)
            goalHUD.ShowGoal(spiderName + "'s new home is ready!");

        SetSubText("Great job!");

        // Stage 7 클리어 → Stage 8 해금
        ProgressManager.Instance.UnlockStage(8);

        Invoke(nameof(LoadNextScene), 4.0f);
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }

    string GetRemainingHint()
    {
        bool cleaned = terrariumCleaner != null && terrariumCleaner.isCleaned;
        if (!spiderMoved) return "Move the spider into the temp box first.";
        if (!cleaned) return "Clean the spider house with the rag.";
        if (decorateCount < 3) return $"More items are needed ({decorateCount}/3).";
        return "";
    }

    void SetSubText(string text)
    {
        if (subText != null) subText.text = text;
        Debug.Log("[Scene07] " + text);
    }
}
