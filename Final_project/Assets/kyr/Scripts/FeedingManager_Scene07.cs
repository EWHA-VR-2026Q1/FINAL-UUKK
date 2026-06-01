using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class FeedingManager_Scene07 : MonoBehaviour
{
    [Header("필요한 먹이 개수")]
    public int requiredFoodCount = 1;

    [Header("거미 Animator")]
    public Animator spiderAnimator;

    [Header("UI")]
    public TextMeshProUGUI goalText;
    public GameObject completionPanel;
    public TextMeshProUGUI completionText;

    [Header("다음 씬")]
    public string nextSceneName = "Scene08_FeedScarySpider";

    private int fedCount = 0;
    private bool isCompleted = false;

    void Start()
    {
        string name = PlayerPrefs.GetString("SpiderName", "Spider");
        goalText.text = "Feed Your Spider";
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
        string name = PlayerPrefs.GetString("SpiderName", "Spider");

        if (spiderAnimator != null)
            spiderAnimator.SetTrigger("Eat");

        completionPanel.SetActive(true);
        completionText.text = name + "Eats Well";

        Invoke("LoadNextScene", 3.5f);
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}