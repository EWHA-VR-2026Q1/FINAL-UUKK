using UnityEngine;

public class TerrariumCleaner : MonoBehaviour
{
    [Header("더러운 상태 오브젝트들")]
    public GameObject dirtyOverlay;          // DirtyOverlay Quad
    public Renderer[] dirtyWalls;            // Wall_Front, Back, Left, Right

    [Header("깨끗한 머티리얼")]
    public Material cleanGlassMaterial;      // M_Glass

    [Header("상태")]
    public bool isCleaned = false;

    // 행주(Rag)의 Collider가 이 오브젝트에 닿으면 호출됨
    private void OnTriggerEnter(Collider other)
    {
        if (isCleaned) return;

        if (other.CompareTag("Rag"))
        {
            CleanTerrarium();
        }
    }

    public void CleanTerrarium()
    {
        isCleaned = true;

        // 1. DirtyOverlay 숨기기
        if (dirtyOverlay != null)
            dirtyOverlay.SetActive(false);

        // 2. 더러운 벽 4개 머티리얼 교체
        foreach (Renderer r in dirtyWalls)
        {
            if (r != null)
                r.material = cleanGlassMaterial;
        }

        Debug.Log("사육장 청소 완료!");
    }
}