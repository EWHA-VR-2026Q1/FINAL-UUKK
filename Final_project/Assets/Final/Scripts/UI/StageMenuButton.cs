using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Stage 메뉴 안의 개별 스테이지 버튼.
///
/// 이민경(UI 담당)이 만드는 구조:
///   - 9개의 스테이지를 표시할 visual 컴포넌트
///   - 버튼 클릭 시 해당 씬 로드 (기본 동작)
///   - 상태(잠금/완료/현재 등)를 시각적으로 보여줄 hook (SetState)
///
/// 김유민(진도체크 로직 담당)이 채울 부분:
///   - 외부에서 SetState(...) 호출해서 색상/잠금 상태 제어
///   - 필요하면 ProgressManager에서 OnClick을 가로채서 잠긴 스테이지 못 들어가게 막기
///   - 클리어 여부, 현재 위치 등의 상태 저장 (PlayerPrefs 등)
/// </summary>
public class StageMenuButton : MonoBehaviour
{
    public enum State { Locked, Available, Cleared, Current }

    [Header("스테이지 정보")]
    [Tooltip("이 버튼이 대표하는 스테이지 번호 (1~9).")]
    public int stageNumber = 1;

    [Tooltip("클릭 시 이동할 씬 이름. Build Settings에 등록된 이름과 정확히 일치해야 함.")]
    public string targetSceneName = "";

    [Header("시각 컴포넌트 (각각 자동/수동 연결)")]
    [Tooltip("색 변경할 배경 Image. 비워두면 자식에서 자동으로 찾음.")]
    public Image background;
    [Tooltip("스테이지 번호 표시 텍스트. 비어두면 자식에서 자동으로 찾음.")]
    public TextMeshProUGUI label;

    [Header("상태별 색상 (Inspector에서 조정)")]
    public Color lockedColor    = new Color(0.4f, 0.4f, 0.4f, 1f);  // 회색
    public Color availableColor = new Color(0.9f, 0.9f, 0.9f, 1f);  // 흰색
    public Color clearedColor   = new Color(0.4f, 0.8f, 0.4f, 1f);  // 초록
    public Color currentColor   = new Color(1.0f, 0.85f, 0.3f, 1f); // 노랑

    private State currentState = State.Available;
    public State CurrentState => currentState;

    void Awake()
    {
        if (background == null) background = GetComponent<Image>();
        if (label == null) label = GetComponentInChildren<TextMeshProUGUI>();

        // 라벨에 자동으로 스테이지 번호 표시
        if (label != null) label.text = stageNumber.ToString();
    }

    void Start()
    {
        ApplyStateColor();
    }

    /// <summary>
    /// 외부(ProgressManager)에서 호출. 상태를 바꾸면 색상이 자동 갱신됨.
    /// </summary>
    public void SetState(State state)
    {
        currentState = state;
        ApplyStateColor();
    }

    /// <summary>
    /// Button의 OnClick에 이 함수를 연결.
    /// 김유민의 ProgressManager가 가로채고 싶으면 이 함수 대신 자기 함수를 연결하면 됨.
    /// </summary>
    public void OnClick()
    {
        if (currentState == State.Locked)
        {
            Debug.Log($"[StageMenuButton] Stage {stageNumber} 은 아직 잠겨있음.");
            return;
        }

        if (string.IsNullOrWhiteSpace(targetSceneName))
        {
            Debug.LogWarning($"[StageMenuButton] Stage {stageNumber}: targetSceneName 이 비어있음. Inspector에서 설정 필요.", this);
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(targetSceneName))
        {
            Debug.LogError($"[StageMenuButton] '{targetSceneName}' 씬을 찾을 수 없음. Build Settings에 등록됐는지 확인.", this);
            return;
        }

        SceneManager.LoadScene(targetSceneName);
    }

    void ApplyStateColor()
    {
        if (background == null) return;
        switch (currentState)
        {
            case State.Locked:    background.color = lockedColor;    break;
            case State.Available: background.color = availableColor; break;
            case State.Cleared:   background.color = clearedColor;   break;
            case State.Current:   background.color = currentColor;   break;
        }
    }
}
