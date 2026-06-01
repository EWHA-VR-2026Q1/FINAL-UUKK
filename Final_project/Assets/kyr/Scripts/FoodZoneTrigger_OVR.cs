using UnityEngine;

public class FoodZoneTrigger_OVR : MonoBehaviour
{
    public FeedingManager_Scene07 feedingManager;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Food")) return;

        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
        }

        var grab = other.GetComponent<OVRGrabbable>();
        if (grab != null) grab.enabled = false;

        other.transform.position = transform.position;
        feedingManager.OnFoodPlaced();
    }
}