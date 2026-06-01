using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Safezone(또는 임의의 다른) 씬으로 이동시키는 버튼 핸들러.
///
/// 사용법:
///   1) HUD의 Safezone 버튼이 있는 GameObject에 이 컴포넌트를 부착
///      (toSafe 버튼 자체 또는 그 부모 어디든 OK)
///   2) Button 컴포넌트의 OnClick () 섹션:
///        - + 버튼 → 왼쪽 칸에 이 컴포넌트가 붙은 GameObject 드래그
///        - 오른쪽 드롭다운: SafezoneButton → LoadSafezone()
///   3) File → Build Settings에 Safezone 씬이 등록돼 있어야 함
///      (안 그러면 LoadScene이 실패하고 Console에 에러 출력)
///
/// 다른 씬으로 보내고 싶으면 Inspector에서 Safezone Scene Name 값만 바꾸면 됨.
/// 예: Stage1, Stage2 등으로 이름 변경해서 각 스테이지 버튼에 같은 스크립트 재사용 가능.
/// </summary>
public class SafezoneButton : MonoBehaviour
{
    [Tooltip("로드할 씬 이름. Build Settings에 등록된 이름과 정확히 같아야 함(대소문자 구분).")]
    public string safezoneSceneName = "Safezone";

    [Tooltip("로드 직전 콘솔에 로그를 남길지 여부. 디버깅용.")]
    public bool logBeforeLoad = true;

    private RectTransform rectTransform;
    private Button button;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        button = GetComponent<Button>();
    }

    private void Update()
    {
        if (button != null && !button.interactable)
        {
            return;
        }

        if (VRPointerClickUtility.WasClickPressed() &&
            VRPointerClickUtility.IsPointingAt(rectTransform))
        {
            LoadSafezone();
        }
    }

    /// <summary>Button의 OnClick에 이 함수를 연결한다.</summary>
    public void LoadSafezone()
    {
        if (string.IsNullOrWhiteSpace(safezoneSceneName))
        {
            Debug.LogWarning("[SafezoneButton] Scene name is empty. Set it in the Inspector.", this);
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(safezoneSceneName))
        {
            Debug.LogError(
                $"[SafezoneButton] Could not find scene '{safezoneSceneName}'.\n" +
                "Check that it is listed in File > Build Settings > Scenes In Build.\n" +
                "Also check that the scene name matches exactly.",
                this);
            return;
        }

        if (logBeforeLoad)
            Debug.Log($"[SafezoneButton] Loading scene: {safezoneSceneName}", this);

        SceneManager.LoadScene(safezoneSceneName);
    }

    /// <summary>외부 코드에서 직접 씬 이름을 넘겨 호출할 수도 있게 노출.</summary>
    public void LoadSceneByName(string sceneName)
    {
        safezoneSceneName = sceneName;
        LoadSafezone();
    }
}
