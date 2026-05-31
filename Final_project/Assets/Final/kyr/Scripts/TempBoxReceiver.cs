using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TempBoxReceiver : MonoBehaviour
{
    [Header("씬 매니저")]
    public Scene09_Manager sceneManager;

    private bool spiderReceived = false;

    private void OnTriggerEnter(Collider other)
    {
        if (spiderReceived) return;
        if (!other.CompareTag("Spider")) return;

        spiderReceived = true;

        // 거미 물리/Grab 고정
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        XRGrabInteractable grab =
            other.GetComponent<XRGrabInteractable>();
        if (grab != null) grab.enabled = false;

        // TempBox 안으로 위치 고정
        other.transform.position = transform.position;

        // 씬 매니저에 알림
        sceneManager.OnSpiderInBox(other.gameObject);
    }
}