using UnityEngine;

/// <summary>
/// TempBox 안에 부착하는 거미 수신 트리거.
/// 거미(Tag="Spider")가 트리거에 들어오면 씬 매니저에 알립니다.
/// </summary>
public class TempBoxReceiver_OVR : MonoBehaviour
{
    [Header("씬별 매니저 - 해당 씬에 맞는 것 하나만 연결하세요")]
    public Scene09_Manager      sceneManager09Clean;  // Step07_CleanSpiderHouse 에서 사용
    public Scene09_Manager      sceneManager09Old;    // 레거시 호환용 (비워도 됨)

    private bool spiderReceived = false;

    private void OnTriggerEnter(Collider other)
    {
        if (spiderReceived) return;
        if (!other.CompareTag("Spider")) return;

        spiderReceived = true;

        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        OVRGrabbable grab = other.GetComponent<OVRGrabbable>();
        if (grab != null) grab.enabled = false;

        other.transform.position = transform.position;
        other.gameObject.SetActive(false);

        if (sceneManager09Clean != null) sceneManager09Clean.OnSpiderInBox(other.gameObject);
        else if (sceneManager09Old != null) sceneManager09Old.OnSpiderInBox(other.gameObject);
    }
}
