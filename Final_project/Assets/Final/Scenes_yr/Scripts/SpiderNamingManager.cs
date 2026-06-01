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

    // ※ Build Settings의 씬 이름과 정확히 일치해야 합니다
    [Header("다음 씬 이름 (Build Settings 기준)")]
    public string nextSceneName = "Scene07_FeedCuteSpider (OVR)";

    private Color normalColor   = new Color(0.24f, 0.24f, 0.31f);
    private Color selectedColor = new Color(0.29f, 0.56f, 1.00f);
    private bool  nameConfirmed = false;

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
        if (nameConfirmed) return;   // 중복 호출 방지
        nameConfirmed = true;

        pipButtonImage.color  = spiderName == "Pip"   ? selectedColor : normalColor;
        venomButtonImage.color = spiderName == "Venom" ? selectedColor : normalColor;

        PlayerPrefs.SetString("SpiderName", spiderName);
        PlayerPrefs.Save();

        namingPanel.SetActive(false);
        spiderNameDisplay.gameObject.SetActive(true);
        spiderNameDisplay.text = "\"" + spiderName + "\" 좋은 이름이에요!";

        if (goalHUD != null)
            goalHUD.ShowGoal(spiderName + "(이)라는 이름을 지어줬어요 :)");

        if (spiderAnimator != null)
            spiderAnimator.SetTrigger("React");

        // Scene06 클리어 → Stage 7 해금
        ProgressManager.Instance.UnlockStage(7);

        Invoke(nameof(LoadNextScene), 3.0f);
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
