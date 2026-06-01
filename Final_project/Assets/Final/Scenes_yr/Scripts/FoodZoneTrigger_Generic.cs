using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Scene07 / Scene08 모두에서 사용 가능한 범용 먹이 존 트리거.
/// Inspector에서 feedingTarget에 FeedingManager_Scene07 또는 FeedingManager_Scene08 중
/// 해당 씬의 매니저를 연결하세요.
///
/// OVR 환경: OVRGrabbable 컴포넌트를 비활성화합니다.
/// XR 환경: XRGrabInteractable 컴포넌트를 비활성화합니다.
/// (둘 다 없어도 동작합니다)
/// </summary>
public class FoodZoneTrigger_Generic : MonoBehaviour
{
    [Header("먹이 태그 (Food 오브젝트의 Tag)")]
    public string foodTag = "Food";

    [Header("매니저 연결 — 둘 중 하나만 연결하세요")]
    public FeedingManager_Scene07 feedingManager07;
    public FeedingManager_Scene08 feedingManager08;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag(foodTag)) return;

        triggered = true;

        // 물리 고정
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
        }

        // OVR Grab 비활성화
        var ovrGrab = other.GetComponent("OVRGrabbable") as MonoBehaviour;
        if (ovrGrab != null) ovrGrab.enabled = false;

        // XR Grab 비활성화
        var xrGrab = other.GetComponent<XRGrabInteractable>();
        if (xrGrab != null) xrGrab.enabled = false;

        other.transform.position = transform.position;

        if (feedingManager07 != null) feedingManager07.OnFoodPlaced();
        else if (feedingManager08 != null) feedingManager08.OnFoodPlaced();
    }
}
