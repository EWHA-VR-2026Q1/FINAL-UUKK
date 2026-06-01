using UnityEngine;

/// <summary>
/// 임시 박스(TempBox)에 부착하는 컴포넌트.
/// Scene07_Manager 가 모든 조건이 충족되면 EnableClick() 을 호출하고,
/// 플레이어가 박스를 클릭(Ray/Poke/Collider)하면 OnBoxClicked() 를 실행합니다.
///
/// VR 레이캐스트 클릭 연동 방법:
///   OVR: OVRRaycaster 또는 Physics.Raycast → OnBoxClicked() 직접 호출
///   XR : XR UI Input Module 또는 XRSimpleInteractable의 SelectEntered 이벤트 → OnBoxClicked() 연결
///   PC 테스트: Update의 Mouse 클릭 코드 활성화
/// </summary>
public class TempBoxClickable : MonoBehaviour
{
    [Header("Scene07_Manager 연결")]
    public Scene07_Manager sceneManager;

    [Header("클릭 가능 상태 시각 피드백 (선택)")]
    public GameObject clickableIndicator;   // 예: 빛나는 아우라 이펙트 오브젝트

    private bool isClickable = false;
    private bool isClicked = false;

    void Start()
    {
        if (clickableIndicator != null)
            clickableIndicator.SetActive(false);
    }

    /// <summary>Scene07_Manager 가 모든 조건 충족 후 호출합니다.</summary>
    public void EnableClick()
    {
        isClickable = true;
        if (clickableIndicator != null)
            clickableIndicator.SetActive(true);

        Debug.Log("[TempBox] Click is now enabled.");
    }

    /// <summary>
    /// VR 이벤트(XR SelectEntered / OVR OnPointerClick 등) 또는
    /// Inspector의 UnityEvent에서 이 메서드를 직접 연결하세요.
    /// </summary>
    public void OnBoxClicked()
    {
        if (!isClickable || isClicked) return;
        isClicked = true;

        if (clickableIndicator != null)
            clickableIndicator.SetActive(false);

        Debug.Log("[TempBox] Temp box clicked. Clearing stage...");

        if (sceneManager != null)
            sceneManager.OnTempBoxClicked();
    }

    // ── PC 디버그용 (빌드 전에 제거하거나 #if UNITY_EDITOR 로 감싸세요) ──
#if UNITY_EDITOR
    void Update()
    {
        if (!isClickable || isClicked) return;
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.gameObject == gameObject)
                OnBoxClicked();
        }
    }
#endif
}
