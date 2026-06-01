using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Step07_CleanSpiderHouse (OVR) — 거미 사육장 세팅 클리어 매니저.
///
/// ── 클리어 흐름 ────────────────────────────────────────────────
///  Step 1. 거미 → TempBox 드래그  (TempBoxReceiver_OVR → OnSpiderInBox)
///  Step 2. 행주(rag_yellow)로 CleanTrigger 닦기  (Update에서 isCleaned 폴링)
///  Step 3. 토양·나무·이끼 TerrariumInsideZone 에 배치  (PlaceableItem × 3)
///  Step 4. 위 3가지 완료 → TempBox OVR 레이 클릭  (TempBoxClickable_OVR → OnTempBoxClicked)
///           → ProgressManager.UnlockStage(nextStageNumber) 호출
/// ──────────────────────────────────────────────────────────────
/// </summary>
public class Scene09_Manager : MonoBehaviour
{
    public enum Step
    {
        MoveSpiderToBox,
        CleanTerrarium,
        DecorateTerrarium,
        Done
    }

    [Header("Current Step")]
    public Step currentStep = Step.MoveSpiderToBox;

    [Header("Spider Object")]
    public GameObject spiderObject;

    [Header("Terrarium Cleaner")]
    public TerrariumCleaner terrariumCleaner;

    [Header("Placeable Items")]
    public PlaceableItem branchItem;
    public PlaceableItem mossItem;
    public PlaceableItem soilItem;

    [Header("임시 박스 클릭 (TempBox에 TempBoxClickable_OVR 부착 후 여기 연결)")]
    public TempBoxClickable_OVR tempBoxClickable;

    [Header("UI")]
    public TextMeshProUGUI goalText;
    public TextMeshProUGUI subText;

    [Header("해금할 다음 스테이지 번호 (Inspector에서 직접 입력)")]
    [SerializeField] private int nextStageNumber = 10;

    // ※ Build Settings 씬 이름과 정확히 일치해야 합니다
    [Header("Next Scene (Build Settings 기준)")]
    public string nextSceneName = "Scene10_FindSpider";

    private int  decorateCount = 0;
    private bool isCleared     = false;

    // ── 생명주기 ───────────────────────────────────────────────

    void Start()
    {
        string n = PlayerPrefs.GetString("SpiderName", "Spider");
        goalText.text = "Clean " + n + "'s home!";
        subText.text  = "Put " + n + " in the temp box.";

        AddPlaceTracker(branchItem);
        AddPlaceTracker(mossItem);
        AddPlaceTracker(soilItem);
    }

    void Update()
    {
        // TerrariumCleaner 는 내부에서 isCleaned 만 바꾸므로 폴링 감지
        if (currentStep == Step.CleanTerrarium
            && terrariumCleaner != null && terrariumCleaner.isCleaned)
        {
            currentStep = Step.DecorateTerrarium;
            string n = PlayerPrefs.GetString("SpiderName", "Spider");
            subText.text = "Put items back into " + n + "'s home!";
        }
    }

    // ── PlaceTracker 콜백 등록 ─────────────────────────────────

    void AddPlaceTracker(PlaceableItem item)
    {
        if (item == null) return;
        PlaceTracker tracker = item.gameObject.AddComponent<PlaceTracker>();
        tracker.onPlaced = OnDecorateItemPlaced;
    }

    // ── 외부 콜백 ─────────────────────────────────────────────

    /// <summary>
    /// TempBoxReceiver_OVR 의 sceneManager09Clean 필드에 이 컴포넌트를 연결하세요.
    /// 거미가 TempBox 트리거에 들어오면 자동 호출됩니다.
    /// </summary>
    public void OnSpiderInBox(GameObject spider)
    {
        if (currentStep != Step.MoveSpiderToBox) return;
        currentStep = Step.CleanTerrarium;
        subText.text = "Clean the terrarium with the rag!";
    }

    void OnDecorateItemPlaced()
    {
        if (currentStep != Step.DecorateTerrarium) return;
        decorateCount++;
        subText.text = "Items placed: " + decorateCount + " / 3";

        if (decorateCount >= 3)
            OnAllItemsPlaced();
    }

    /// <summary>
    /// TempBoxClickable_OVR 의 sceneManager 필드에 이 컴포넌트를 연결하세요.
    /// OVR 레이저로 TempBox를 클릭하면 자동 호출됩니다.
    /// </summary>
    public void OnTempBoxClicked()
    {
        if (isCleared) return;

        if (currentStep != Step.Done)
        {
            subText.text = GetRemainingHint();   // 조건 미충족 힌트 표시
            return;
        }

        TriggerClear();
    }

    // ── 내부 로직 ─────────────────────────────────────────────

    void OnAllItemsPlaced()
    {
        currentStep = Step.Done;

        // 기존 거미 위치 저장 로직 유지
        if (spiderObject != null)
        {
            PlayerPrefs.SetFloat("SpiderLastX", spiderObject.transform.position.x);
            PlayerPrefs.SetFloat("SpiderLastY", spiderObject.transform.position.y);
            PlayerPrefs.SetFloat("SpiderLastZ", spiderObject.transform.position.z);
            PlayerPrefs.Save();
        }

        string n = PlayerPrefs.GetString("SpiderName", "Spider");
        subText.text  = "All done! Click the temp box to finish.";
        goalText.text = "Click the temp box!";

        // TempBox 클릭 가능 상태 활성화
        if (tempBoxClickable != null)
            tempBoxClickable.EnableClick();
        else
            Debug.LogWarning("[Scene09_Manager] TempBoxClickable_OVR 가 연결되지 않았습니다. " +
                             "TempBox 오브젝트에 TempBoxClickable_OVR 를 부착하고 이 필드에 연결하세요.");
    }

    void TriggerClear()
    {
        isCleared = true;
        string n = PlayerPrefs.GetString("SpiderName", "Spider");
        goalText.text = n + "'s new home is ready!";
        subText.text  = "Great job! :)";

        // Step07 클리어 → Inspector의 nextStageNumber 로 해금
        ProgressManager.Instance.UnlockStage(nextStageNumber);

        Invoke(nameof(LoadNextScene), 4.0f);
    }

    void LoadNextScene() => SceneManager.LoadScene(nextSceneName);

    string GetRemainingHint()
    {
        switch (currentStep)
        {
            case Step.MoveSpiderToBox:   return "Put the spider in the temp box first!";
            case Step.CleanTerrarium:    return "Clean the terrarium with the rag!";
            case Step.DecorateTerrarium: return $"Place all items! ({decorateCount}/3)";
            default:                     return "";
        }
    }
}
