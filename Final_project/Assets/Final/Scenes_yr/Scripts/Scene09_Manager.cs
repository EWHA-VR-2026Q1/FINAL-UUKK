using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

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

    [Header("UI")]
    public TextMeshProUGUI goalText;
    public TextMeshProUGUI subText;

    [Header("Next Scene")]
    public string nextSceneName = "Scene10_FindSpider";

    private int decorateCount = 0;

    void Start()
    {
        string name = PlayerPrefs.GetString("SpiderName", "Spider");
        goalText.text = "Clean " + name + "'s home!";
        subText.text = "Put " + name + " in the temp box.";

        AddPlaceTracker(branchItem);
        AddPlaceTracker(mossItem);
        AddPlaceTracker(soilItem);
    }

    void AddPlaceTracker(PlaceableItem item)
    {
        if (item == null) return;
        PlaceTracker tracker = item.gameObject.AddComponent<PlaceTracker>();
        tracker.onPlaced = OnDecorateItemPlaced;
    }

    void Update()
    {
        if (currentStep == Step.CleanTerrarium)
        {
            if (terrariumCleaner != null && terrariumCleaner.isCleaned)
            {
                currentStep = Step.DecorateTerrarium;
                string name = PlayerPrefs.GetString("SpiderName", "Spider");
                subText.text = "Put items back into " + name + "'s home!";
            }
        }
    }

    public void OnSpiderInBox(GameObject spider)
    {
        if (currentStep != Step.MoveSpiderToBox) return;
        currentStep = Step.CleanTerrarium;
        string name = PlayerPrefs.GetString("SpiderName", "Spider");
        subText.text = "Clean the terrarium with the rag!";
    }

    void OnDecorateItemPlaced()
    {
        if (currentStep != Step.DecorateTerrarium) return;
        decorateCount++;
        subText.text = "Items placed: " + decorateCount + " / 3";

        if (decorateCount >= 3)
            OnAllComplete();
    }

    void OnAllComplete()
    {
        // 거미 사라짐
        if (spiderObject != null)
        {
            // 씬 10 담당자용 위치 저장
            PlayerPrefs.SetFloat("SpiderLastX", spiderObject.transform.position.x);
            PlayerPrefs.SetFloat("SpiderLastY", spiderObject.transform.position.y);
            PlayerPrefs.SetFloat("SpiderLastZ", spiderObject.transform.position.z);
            PlayerPrefs.Save();

            spiderObject.SetActive(false);
        }

        string name = PlayerPrefs.GetString("SpiderName", "Spider");
        subText.text = name + " is gone...!";

        Invoke("LoadNextScene", 4.0f);
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}