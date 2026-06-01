using UnityEngine;

public class PlaceableItem : MonoBehaviour
{
    [Header("배치 완료 후 나타날 오브젝트 (사육장 내부 배치 시각화)")]
    public GameObject placedVisual;   // 사육장 안에 미리 만들어둔 배치 완료 오브젝트

    [Header("배치 가능한 영역 태그")]
    public string targetZoneTag = "TerrariumInside";

    private bool isPlaced = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isPlaced) return;

        if (other.CompareTag(targetZoneTag))
        {
            PlaceItem();
        }
    }

    void PlaceItem()
    {
        isPlaced = true;

        // 집어다 넣은 오브젝트 숨기기
        gameObject.SetActive(false);

        // 사육장 내부에 배치된 시각 오브젝트 활성화
        if (placedVisual != null)
            placedVisual.SetActive(true);

        Debug.Log(gameObject.name + " 배치 완료");
    }
}