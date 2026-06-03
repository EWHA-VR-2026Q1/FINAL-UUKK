using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// VR용 Goal HUD — 씬 시작 시 사용자에게 목표 문구를 띄워주는 World Space UI.
///
/// 구조 (Unity에서 직접 셋업):
///   GoalHUD (이 스크립트 부착)
///    └ Canvas (Render Mode: World Space, 작은 스케일)
///        └ Panel (살짝 어두운 배경)
///            └ Text (TextMeshPro UGUI)
///
/// 사용법:
///   - 이 GameObject를 Main Camera의 자식으로 두면 머리를 따라옴
///   - Position: (0, 0.3, 1.5) 정도 — 살짝 위쪽, 1.5m 앞
///   - 각 씬에서 Inspector의 goalText에 그 씬의 목표 문구 입력
///   - 또는 다른 스크립트에서 SetGoal("문구") 호출
///
/// 권장 옵션:
///   - showOnStart 켜두면 씬 진입 시 자동으로 페이드인
///   - autoHideAfter > 0이면 그 시간 후 자동으로 페이드아웃 (0이면 계속 표시)
/// </summary>
public class GoalHUD : MonoBehaviour
{
    [Header("참조")]
    [Tooltip("화면에 띄울 TextMeshPro 텍스트.")]
    public TextMeshProUGUI goalText;
    [Tooltip("페이드 효과를 적용할 CanvasGroup. 없으면 자동으로 자식에서 찾음.")]
    public CanvasGroup canvasGroup;

    [Header("초기 문구")]
    [Tooltip("씬 시작 시 표시할 문구. 비어있으면 표시 안 함.")]
    [TextArea(2, 5)]
    public string initialGoal = "";

    [Header("표시 동작")]
    [Tooltip("씬 시작 시 자동으로 띄울지 여부.")]
    public bool showOnStart = true;
    [Tooltip("페이드 인 시간(초).")]
    public float fadeInDuration = 1.0f;
    [Tooltip("페이드 아웃 시간(초).")]
    public float fadeOutDuration = 1.0f;
    [Tooltip("표시 후 자동으로 사라지기까지 시간(초). 0이면 계속 표시(기본).")]
    public float autoHideAfter = 0f;

    private Coroutine activeRoutine;

    void Awake()
    {
        if (canvasGroup == null) canvasGroup = GetComponentInChildren<CanvasGroup>();
        if (goalText == null) goalText = FindGoalText();
        if (canvasGroup != null) canvasGroup.alpha = 0f;
    }

    private TextMeshProUGUI FindGoalText()
    {
        Transform goalPanel = transform.Find("Canvas/GoalPanel");
        if (goalPanel != null)
        {
            TextMeshProUGUI panelText = goalPanel.GetComponentInChildren<TextMeshProUGUI>(true);
            if (panelText != null)
            {
                return panelText;
            }
        }

        return GetComponentInChildren<TextMeshProUGUI>(true);
    }

    void Start()
    {
        if (!showOnStart)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(initialGoal) &&
            SceneManager.GetActiveScene().name == "Safezone")
        {
            initialGoal = "Safe Zone\nChoose a stage to continue.";
        }

        if (!string.IsNullOrWhiteSpace(initialGoal))
        {
            ShowGoal(initialGoal);
            return;
        }

        if (goalText != null && !string.IsNullOrWhiteSpace(goalText.text))
        {
            ShowGoal(goalText.text);
        }
    }

    /// <summary>외부에서 새 목표 문구를 띄울 때 호출.</summary>
    public void ShowGoal(string text)
    {
        if (goalText == null || canvasGroup == null) return;
        goalText.text = text;
        if (activeRoutine != null) StopCoroutine(activeRoutine);
        activeRoutine = StartCoroutine(ShowRoutine());
    }

    /// <summary>즉시 숨김.</summary>
    public void HideGoal()
    {
        if (activeRoutine != null) StopCoroutine(activeRoutine);
        activeRoutine = StartCoroutine(FadeTo(0f, fadeOutDuration));
    }

    IEnumerator ShowRoutine()
    {
        // 페이드 인
        yield return FadeTo(1f, fadeInDuration);
        // 자동 숨김
        if (autoHideAfter > 0f)
        {
            yield return new WaitForSeconds(autoHideAfter);
            yield return FadeTo(0f, fadeOutDuration);
        }
        activeRoutine = null;
    }

    IEnumerator FadeTo(float targetAlpha, float duration)
    {
        if (duration <= 0f) { canvasGroup.alpha = targetAlpha; yield break; }
        float start = canvasGroup.alpha;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, targetAlpha, t / duration);
            yield return null;
        }
        canvasGroup.alpha = targetAlpha;
    }
}
