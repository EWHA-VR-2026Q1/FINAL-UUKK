using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TempBoxReceiver_XR : MonoBehaviour
{
    public Scene09_Manager sceneManager;
    private bool spiderReceived = false;

    private void OnTriggerEnter(Collider other)
    {
        if (spiderReceived) return;
        if (!other.CompareTag("Spider")) return;

        spiderReceived = true;

        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        var grab = other.GetComponent<XRGrabInteractable>();
        if (grab != null) grab.enabled = false;

        other.transform.position = transform.position;
        other.gameObject.SetActive(false);
        sceneManager.OnSpiderInBox(other.gameObject);
    }
}