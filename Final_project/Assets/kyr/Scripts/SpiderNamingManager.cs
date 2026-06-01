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
    public TextMeshProUGUI goalText;

    [Header("선택 강조용 이미지")]
    public Image pipButtonImage;
    public Image venomButtonImage;

    [Header("거미")]
    public Animator spiderAnimator;

    [Header("다음 씬")]
    public string nextSceneName = "Scene07_FeedCuteSpider";

    // 버튼 색상
    private Color normalColor = new Color(0.24f, 0.24f, 0.31f);
    private Color selectedColor = new Color(0.29f, 0.56f, 1.00f);

    void Start()
    {
        btnPip.onClick.AddListener(() => OnSelectName("Pip"));
        btnVenom.onClick.AddListener(() => OnSelectName("Venom"));
        spiderNameDisplay.gameObject.SetActive(false);
    }

    void OnSelectName(string spiderName)
    {
        // 버튼 색 강조
        pipButtonImage.color =
            spiderName == "Pip" ? selectedColor : normalColor;
        venomButtonImage.color =
            spiderName == "Venom" ? selectedColor : normalColor;

        // 이름 저장
        PlayerPrefs.SetString("SpiderName", spiderName);
        PlayerPrefs.Save();

        // UI 전환
        namingPanel.SetActive(false);
        spiderNameDisplay.gameObject.SetActive(true);
        spiderNameDisplay.text = "\"" + spiderName + "\" 좋은 이름이에요!";
        goalText.text = spiderName + "에게 이름을 지어줬어요 :)";

        // 거미 반응
        if (spiderAnimator != null)
            spiderAnimator.SetTrigger("React");

        Invoke("LoadNextScene", 3.0f);
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}