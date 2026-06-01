using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Step07_CleanSpiderHouse (OVR) - 거미 사육장 세팅 클리어 매니저.
///
/// 클리어 흐름:
///  1. 거미 -> TempBox 드래그   (TempBoxReceiver_OVR -> OnSpiderInBox)
///  2. 행주로 CleanTrigger 닦기  (Update 폴링 -> isCleaned)
///  3. 토양/나무/이끼 배치 x3    (PlaceableItem -> OnDecorateItemPlaced)
///  4. 위 3개 완료 -> TempBox OVR 레이 클릭  (TempBoxClickable_OVR -> OnTempBoxClicked)
///     -> ProgressManager.UnlockStage(nextStageNumber)
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

    [Header("임시 박스 클릭 - TempBox에 TempBoxClickable_OVR 부착 후 드래그")]
    public TempBoxClickable_OVR tempBoxClickable;

    [Header("UI")]
    public TextMeshProUGUI goalText;
    public TextMeshProUGUI subText;

    [Header("해금할 다음 스테이지 번호 (Inspector에서 직접 입력)")]
    [SerializeField] private int nextStageNumber = 10;

    [Header("Next Scene (Build Settings 기준)")]
    public string nextSceneName = "Scene10_FindSpider";

    private int  decorateCount = 0;
    private bool isCleared     = false;

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
        if (currentStep == Step.CleanTerrarium
            && terrariumCleaner != null && terrariumCleaner.isCleaned)
        {
            currentStep = Step.DecorateTerrarium;
            string n = PlayerPrefs.GetString("SpiderName", "Spider");
            subText.text = "Put items back into " + n + "'s home!";
        }
    }

    void AddPlaceTracker(PlaceableItem item)
    {
        if (item == null) return;
        PlaceTracker tracker = item.gameObject.AddComponent<PlaceTracker>();
        tracker.onPlaced = OnDecorateItemPlaced;
    }

    /// <summary>TempBoxReceiver_OVR 의 sceneManager09Clean 필드에 이 컴포넌트 연결.</summary>
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

    /// <summary>TempBoxClickable_OVR 의 sceneManager 필드에 이 컴포넌트 연결.</summary>
    public void OnTempBoxClicked()
    {
        if (isCleared) return;
        if (currentStep != Step.Done)
        {
            subText.text = GetRemainingHint();
            return;
        }
        TriggerClear();
    }

    void OnAllItemsPlaced()
    {
        currentStep = Step.Done;

        if (spiderObject != null)
        {
            PlayerPrefs.SetFloat("SpiderLastX", spiderObject.transform.position.x);
            PlayerPrefs.SetFloat("SpiderLastY", spiderObject.transform.position.y);
            PlayerPrefs.SetFloat("SpiderLastZ", spiderObject.transform.position.z);
            PlayerPrefs.Save();
            spiderObject.SetActive(false);
        }

        goalText.text = "Click the temp box!";
        subText.text  = "All done! Click the temp box to finish.";

        if (tempBoxClickable != null)
            tempBoxClickable.EnableClick();
        else
            Debug.LogWarning("[Scene09_Manager] TempBoxClickable_OVR 가 연결되지 않았습니다!");
    }

    void TriggerClear()
    {
        isCleared = true;
        string n = PlayerPrefs.GetString("SpiderName", "Spider");
        goalText.text = n + "'s new home is ready!";
        subText.text  = "Great job! :)";

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
            case Step.DecorateTerrarium: return "Place all items! (" + decorateCount + "/3)";
            default: return "";
        }
    }
}
