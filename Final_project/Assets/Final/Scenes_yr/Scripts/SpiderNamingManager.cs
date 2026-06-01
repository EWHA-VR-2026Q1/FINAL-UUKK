using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SpiderNamingManager : MonoBehaviour
{
    [Header("UI")]
    public Button btnPip;
    public Button btnVenom;
    public TextMeshProUGUI spiderNameDisplay;
    public GameObject namingPanel;

    [Header("HUD (VR Final > HUD 안의 GoalHUD 컴포넌트 연결)")]
    public GoalHUD goalHUD;

    [Header("버튼 이미지")]
    public Image pipButtonImage;
    public Image venomButtonImage;

    [Header("애니메이터")]
    public Animator spiderAnimator;

    [Header("다음 씬")]
    public string nextSceneName = "Scene07_FeedCuteSpider";

    private Color normalColor = new Color(0.24f, 0.24f, 0.31f);
    private Color selectedColor = new Color(0.29f, 0.56f, 1.00f);
    private bool  nameConfirmed = false;

    private bool nameConfirmed = false;

    void Start()
    {
        btnPip.onClick.AddListener(()   => OnSelectName("Pip"));
        btnVenom.onClick.AddListener(() => OnSelectName("Venom"));
        spiderNameDisplay.gameObject.SetActive(false);

        if (goalHUD != null)
            goalHUD.ShowGoal("거미의 이름을 지어주세요!");
    }

    void OnSelectName(string spiderName)
    {
        pipButtonImage.color = spiderName == "Pip" ? selectedColor : normalColor;
        venomButtonImage.color = spiderName == "Venom" ? selectedColor : normalColor;

        PlayerPrefs.SetString("SpiderName", spiderName);
        PlayerPrefs.Save();

        namingPanel.SetActive(false);
        spiderNameDisplay.gameObject.SetActive(true);
        spiderNameDisplay.text = "\"" + spiderName + "\" 좋은 이름이에요!";

        if (goalHUD != null)
            goalHUD.ShowGoal(spiderName + "라는 이름을 지어줬어요 :)");

        if (spiderAnimator != null)
            spiderAnimator.SetTrigger("React");

        // Stage 4 클리어 → Stage 5 해금
        ProgressManager.Instance.UnlockStage(5);

        Invoke(nameof(LoadNextScene), 3.0f);
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
