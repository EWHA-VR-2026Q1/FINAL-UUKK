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

    [Header("다음 씬 이름 (Build Settings 기준)")]
    public string nextSceneName = "Step05_FeedCuteSpider (OVR)";

    [Header("해금할 다음 스테이지 번호 (Inspector에서 직접 입력)")]
    [SerializeField] private int nextStageIndex = 7;

    private Color normalColor   = new Color(0.24f, 0.24f, 0.31f);
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
        if (nameConfirmed) return;
        nameConfirmed = true;

        pipButtonImage.color   = spiderName == "Pip"   ? selectedColor : normalColor;
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

        ProgressManager.Instance.UnlockStage(nextStageIndex);

        Invoke(nameof(LoadNextScene), 3.0f);
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
