using UnityEngine;

/// <summary>
/// Scene08 (무서운 거미 먹이 주기) 전용 OVR 먹이 존 트리거.
/// 거미집 트리거 구역 오브젝트에 부착하세요.
///
/// 필수 세팅:
///   이 오브젝트 : Collider → Is Trigger ☑  / Layer = 먹이와 충돌 가능한 레이어
///   먹이 오브젝트 : Tag = "Food" / Rigidbody 있음 / OVRGrabbable 부착
/// </summary>
public class FoodZoneTrigger_OVR_08 : MonoBehaviour
{
    [Header("FeedingManager_Scene08 연결")]
    public FeedingManager_Scene08 feedingManager;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Food")) return;

        triggered = true;

        // 물리 고정 — 먹이가 트리거 중앙에 안착
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.velocity     = Vector3.zero;
        }

        // OVR Grab 해제 — 이후 잡을 수 없게 처리
        OVRGrabbable grab = other.GetComponent<OVRGrabbable>();
        if (grab != null) grab.enabled = false;

        other.transform.position = transform.position;

        if (feedingManager != null)
            feedingManager.OnFoodPlaced();
    }
}
