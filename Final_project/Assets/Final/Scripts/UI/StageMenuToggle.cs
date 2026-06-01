using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HUD의 Stage 버튼이 클릭됐을 때 Stage 메뉴(팝업)를 열고 닫는 토글.
///
/// 사용법:
///   1) HUD의 Stage 버튼이 있는 GameObject에 부착
///   2) Menu To Toggle 슬롯에 StageMenu GameObject 드래그 (또는 자동 탐색에 맡김)
///   3) Button의 OnClick에 Toggle() 함수 연결 (비어있어도 Awake에서 Open을 자동 연결)
///   4) 메뉴 안의 Back 버튼은 Close()를 호출
///
/// Awake에서 Open listener를 추가해 프리팹 override OnClick이 깨져도 열리게 보강함.
/// </summary>
public class StageMenuToggle : MonoBehaviour
{
    [Tooltip("열고 닫을 메뉴 GameObject. StageMenu의 루트를 드래그. 비워두면 transform.root 자식들 중 'StageMenu' 이름인 GameObject를 자동 탐색.")]
    public GameObject menuToToggle;

    [Tooltip("메뉴가 처음엔 닫혀있어야 하면 체크.")]
    public bool startClosed = true;

    [Tooltip("Inspector / 콘솔에 동작 로그를 출력. 디버깅용.")]
    public bool verboseLog = true;

    private void Awake()
    {
        Button button = GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(Open);
    }

    void Start()
    {
        ResolveMenuReference();

        if (menuToToggle == null)
        {
            Debug.LogError("[StageMenuToggle] menuToToggle 자동 탐색 실패. " +
                "transform.root 자식 중에 'StageMenu' 이름인 GameObject가 없거나, " +
                "Inspector에서 직접 할당이 필요함.", this);
            return;
        }

        if (startClosed) menuToToggle.SetActive(false);

        if (verboseLog)
            Debug.Log($"[StageMenuToggle] Ready. menuToToggle = {menuToToggle.name}", this);
    }

    /// <summary>Stage 버튼의 OnClick에 연결. 켜져있으면 끄고, 꺼져있으면 켬.</summary>
    public void Toggle()
    {
        ResolveMenuReference();
        if (menuToToggle == null)
        {
            Debug.LogWarning("[StageMenuToggle] Toggle: menuToToggle reference is missing.", this);
            return;
        }

        bool nextState = !menuToToggle.activeSelf;
        menuToToggle.SetActive(nextState);

        if (verboseLog)
            Debug.Log($"[StageMenuToggle] Toggle → {nextState} (menu='{menuToToggle.name}')", this);
    }

    /// <summary>외부에서 강제로 열기.</summary>
    public void Open()
    {
        ResolveMenuReference();
        if (menuToToggle == null) return;
        menuToToggle.SetActive(true);
        if (verboseLog) Debug.Log("[StageMenuToggle] Open", this);
    }

    /// <summary>메뉴 안의 Back 버튼의 OnClick에 연결.</summary>
    public void Close()
    {
        ResolveMenuReference();
        if (menuToToggle == null) return;
        menuToToggle.SetActive(false);
        if (verboseLog) Debug.Log("[StageMenuToggle] Close", this);
    }

    private void ResolveMenuReference()
    {
        if (menuToToggle != null) return;

        Transform root = transform.root;
        Transform[] children = root.GetComponentsInChildren<Transform>(true);

        foreach (Transform child in children)
        {
            if (child.name == "StageMenu")
            {
                menuToToggle = child.gameObject;
                if (verboseLog)
                    Debug.Log($"[StageMenuToggle] menuToToggle 자동 탐색 성공: {child.GetInstanceID()}", this);
                return;
            }
        }
    }
}
