using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SpiderHouseReceiver : MonoBehaviour
{
    public Scene08_Manager sceneManager;
    private bool spiderReceived = false;

    private void OnTriggerEnter(Collider other)
    {
        if (spiderReceived) return;
        if (!other.CompareTag("Spider")) return;

        spiderReceived = true;

        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        var xrGrab = other.GetComponent<XRGrabInteractable>();
        if (xrGrab != null) xrGrab.enabled = false;

        var ovrGrab = other.GetComponent<OVRGrabbable>();
        if (ovrGrab != null) ovrGrab.enabled = false;

        other.transform.position = transform.position;

        sceneManager.OnSpiderInHouse(other.gameObject);
    }
}