using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Step4 씬 — 거미 이름 짓기 클리어 매니저.
/// 이름 버튼 클릭 → UnlockStage(nextStageIndex) → Step5 로드
/// </summary>
public class SpiderNamingManager : MonoBehaviour
{
    [Header("UI")]
    public Button              btnPip;
    public Button              btnVenom;
    public TextMeshProUGUI     spiderNameDisplay;
    public GameObject          namingPanel;

    [Header("HUD (VRSystem_Final 안의 GoalHUD 컴포넌트 연결)")]
    public GoalHUD             goalHUD;

    [Header("버튼 이미지")]
    public Image               pipButtonImage;
    public Image               venomButtonImage;

    [Header("VR Hover Feedback")]
    [SerializeField] private Color hoverColor = new Color(1.0f, 0.86f, 0.28f);
    [SerializeField] private float hoverScale = 1.08f;
    [SerializeField] private float hoverRayDistance = 12f;

    [Header("애니메이터")]
    public Animator            spiderAnimator;

    [Header("다음 씬 이름 (Build Settings 의 씬 이름과 정확히 일치)")]
    public string              nextSceneName  = "Step5";

    [Header("해금할 다음 스테이지 번호")]
    [SerializeField] private int nextStageIndex = 5;

    private Color normalColor   = new Color(0.24f, 0.24f, 0.31f);
    private Color selectedColor = new Color(0.29f, 0.56f, 1.00f);
    private bool  nameConfirmed = false;
    private RectTransform pipRect;
    private RectTransform venomRect;

    void Start()
    {
        btnPip.onClick.AddListener(()   => OnSelectName("Pip"));
        btnVenom.onClick.AddListener(() => OnSelectName("Venom"));
        spiderNameDisplay.gameObject.SetActive(false);

        pipRect = btnPip != null ? btnPip.GetComponent<RectTransform>() : null;
        venomRect = btnVenom != null ? btnVenom.GetComponent<RectTransform>() : null;

        if (pipButtonImage != null) normalColor = pipButtonImage.color;

        if (goalHUD != null)
            goalHUD.ShowGoal("Choose a name for the spider.");
    }

    void Update()
    {
        if (nameConfirmed) return;

        bool pipHovered = pipRect != null &&
                          VRPointerClickUtility.IsPointingAt(pipRect, hoverRayDistance);
        bool venomHovered = venomRect != null &&
                            VRPointerClickUtility.IsPointingAt(venomRect, hoverRayDistance);

        ApplyHoverFeedback(pipButtonImage, pipRect, pipHovered);
        ApplyHoverFeedback(venomButtonImage, venomRect, venomHovered);

        if (!VRPointerClickUtility.WasClickPressed()) return;

        if (pipHovered) OnSelectName("Pip");
        else if (venomHovered) OnSelectName("Venom");
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
        spiderNameDisplay.text = "\"" + spiderName + "\" is a good name!";

        if (goalHUD != null)
            goalHUD.ShowGoal("You named the spider " + spiderName + ".");

        if (spiderAnimator != null)
            spiderAnimator.SetTrigger("React");

        // Step4 클리어 → Inspector의 nextStageIndex 번호 해금
        ProgressManager.Instance.UnlockStage(nextStageIndex);

        Invoke(nameof(LoadNextScene), 3.0f);
    }

    void LoadNextScene() => SceneManager.LoadScene(nextSceneName);

    private void ApplyHoverFeedback(Image image, RectTransform rect, bool hovered)
    {
        if (image != null)
            image.color = hovered ? hoverColor : normalColor;

        if (rect != null)
            rect.localScale = Vector3.one * (hovered ? hoverScale : 1f);
    }
}
