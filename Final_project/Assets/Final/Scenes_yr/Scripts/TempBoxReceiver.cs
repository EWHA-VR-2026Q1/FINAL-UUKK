using UnityEngine;

/// <summary>
/// Legacy receiver kept for scenes that still serialize the old TempBoxReceiver script.
/// New scenes can use TempBoxReceiver_OVR, but Step7/Step8 still reference this GUID.
/// </summary>
public class TempBoxReceiver : MonoBehaviour
{
    public Scene09_Manager sceneManager;

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

        OVRGrabbable grab = other.GetComponent<OVRGrabbable>();
        if (grab != null)
        {
            grab.enabled = false;
        }

        other.transform.position = transform.position;

        if (sceneManager != null)
        {
            sceneManager.OnSpiderInBox(other.gameObject);
        }
        else
        {
            Debug.LogWarning("[TempBoxReceiver] sceneManager is not assigned.", this);
        }
    }
}
