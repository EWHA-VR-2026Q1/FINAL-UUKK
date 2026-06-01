using UnityEngine;

public class TempBoxReceiver_OVR : MonoBehaviour
{
    [Header("씬별 매니저 — 해당 씬에 맞는 것만 연결하세요")]
    public Scene09_Manager sceneManager09;   // Scene09(기존 사용)
    public Scene07_Manager sceneManager07;   // Scene07(신규 사용)

    private bool spiderReceived = false;

    private void OnTriggerEnter(Collider other)
    {
        if (spiderReceived) return;
        if (!other.CompareTag("Spider")) return;

        spiderReceived = true;

        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        var grab = other.GetComponent<OVRGrabbable>();
        if (grab != null) grab.enabled = false;

        other.transform.position = transform.position;
        other.gameObject.SetActive(false);

        if (sceneManager07 != null) sceneManager07.OnSpiderInBox(other.gameObject);
        else if (sceneManager09 != null) sceneManager09.OnSpiderInBox(other.gameObject);
    }
}