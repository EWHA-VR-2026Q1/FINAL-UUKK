using UnityEngine;

/// <summary>
/// HUD의 Stage 버튼이 클릭됐을 때 Stage 메뉴(팝업)를 열고 닫는 토글.
///
/// 사용법:
///   1) HUD의 Stage 버튼이 있는 GameObject(또는 임의 GameObject)에 부착
///   2) Menu To Toggle 슬롯에 StageMenu GameObject 드래그
///   3) Button의 OnClick에 Toggle() 함수 연결
///
/// 메뉴 안의 "Back" 버튼은 Close()를 호출하면 됨.
/// 또는 그냥 메뉴 GameObject.SetActive(false) 호출.
/// </summary>
public class StageMenuToggle : MonoBehaviour
{
    [Tooltip("열고 닫을 메뉴 GameObject. StageMenu의 루트를 드래그.")]
    public GameObject menuToToggle;

    [Tooltip("메뉴가 처음엔 닫혀있어야 하면 체크.")]
    public bool startClosed = true;

    void Start()
    {
        if (menuToToggle != null && startClosed)
            menuToToggle.SetActive(false);
    }

    /// <summary>Stage 버튼의 OnClick에 연결.</summary>
    public void Toggle()
    {
        if (menuToToggle == null) return;
        menuToToggle.SetActive(!menuToToggle.activeSelf);
    }

    /// <summary>외부에서 강제로 열기.</summary>
    public void Open()
    {
        if (menuToToggle != null) menuToToggle.SetActive(true);
    }

    /// <summary>메뉴 안의 Back 버튼의 OnClick에 연결.</summary>
    public void Close()
    {
        if (menuToToggle != null) menuToToggle.SetActive(false);
    }
}
