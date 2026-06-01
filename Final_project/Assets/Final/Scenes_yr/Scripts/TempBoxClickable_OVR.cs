using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 임시 박스(TempBox)에 부착하는 OVR 레이캐스트 클릭 감지 컴포넌트.
///
/// ─ OVR 클릭 동작 방식 ─────────────────────────────────────────
///   OVRInputModule + PhysicsRaycaster(카메라) 구성을 전제로,
///   IPointerClickHandler 인터페이스를 통해 OVR 레이저 클릭을 감지합니다.
///
/// ─ 필수 씬 세팅 ───────────────────────────────────────────────
///   1. EventSystem 오브젝트에 OVRInputModule 컴포넌트 부착
///   2. OVRCameraRig > CenterEyeAnchor 에 PhysicsRaycaster 부착
///   3. 이 임시 박스 오브젝트에 Collider(비트리거) + 이 스크립트 부착
///   4. Inspector에서 sceneManager 필드에 Scene09_CleanManager 연결
///
/// ─ EnableClick() 호출 전에는 클릭이 무시됩니다 ───────────────
/// </summary>
public class TempBoxClickable_OVR : MonoBehaviour, IPointerClickHandler
{
    [Header("Scene09_Manager 연결")]
    public Scene09_Manager sceneManager;

    [Header("클릭 가능 상태 시각 피드백 오브젝트 (선택 — 빛나는 아우라 등)")]
    public GameObject clickableIndicator;

    private bool isClickable = false;
    private bool isClicked   = false;

    void Start()
    {
        if (clickableIndicator != null)
            clickableIndicator.SetActive(false);
    }

    /// <summary>Scene09_Manager 가 모든 조건 충족 후 자동 호출합니다.</summary>
    public void EnableClick()
    {
        isClickable = true;
        if (clickableIndicator != null)
            clickableIndicator.SetActive(true);

        Debug.Log("[TempBox] 클릭 가능 상태 활성화");
    }

    // IPointerClickHandler — OVRInputModule + PhysicsRaycaster 가 자동 호출
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isClickable || isClicked) return;
        isClicked = true;

        if (clickableIndicator != null)
            clickableIndicator.SetActive(false);

        Debug.Log("[TempBox] OVR 레이 클릭 감지 → 클리어 처리");

        if (sceneManager != null)
            sceneManager.OnTempBoxClicked();
    }
}
