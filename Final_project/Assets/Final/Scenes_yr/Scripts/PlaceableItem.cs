using UnityEngine;

public class PlaceableItem : MonoBehaviour
{
    [Header("배치 완료 시 나타날 비주얼 (terrarium 안 미리 배치된 placedVisual)")]
    public GameObject placedVisual;

    [Header("배치 가능한 zone 태그")]
    public string targetZoneTag = "TerrariumInside";

    [Header("Debug")]
    public bool verboseLog = true;

    private bool isPlaced = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isPlaced) return;

        if (verboseLog)
            Debug.Log($"[PlaceableItem:{name}] OnTriggerEnter → other={other.name}, tag={other.tag}", this);

        if (other.CompareTag(targetZoneTag))
            PlaceItem(other);
    }

    void PlaceItem(Collider zone)
    {
        isPlaced = true;

        if (verboseLog)
            Debug.Log($"[PlaceableItem:{name}] PlaceItem fired in zone '{zone.name}'. " +
                $"placedVisual = {(placedVisual != null ? placedVisual.name : "NULL")}", this);

        // 1) 이동 가능한 자기 자신 비활성화
        gameObject.SetActive(false);

        // 2) placedVisual 활성화 + 부모 체인까지 확인
        if (placedVisual == null)
        {
            Debug.LogError($"[PlaceableItem:{name}] placedVisual is NULL! Inspector에서 연결 확인.", this);
            return;
        }

        // 부모 체인 중 비활성이 있으면 placedVisual.SetActive(true) 해도 안 보임
        Transform t = placedVisual.transform;
        while (t != null)
        {
            if (!t.gameObject.activeSelf && t.gameObject != placedVisual)
            {
                Debug.LogWarning($"[PlaceableItem:{name}] placedVisual의 부모 '{t.name}' 가 비활성 상태. " +
                    "이거 때문에 안 보일 수 있음. 부모를 활성화해야 함.", this);
            }
            t = t.parent;
        }

        placedVisual.SetActive(true);

        if (verboseLog)
            Debug.Log($"[PlaceableItem:{name}] placedVisual '{placedVisual.name}' activated. " +
                $"activeSelf={placedVisual.activeSelf}, activeInHierarchy={placedVisual.activeInHierarchy}", this);
    }
}
