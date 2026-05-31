using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class FoodZoneTrigger : MonoBehaviour
{
    [Header("씬 매니저 연결")]
    public FeedingManager_Scene07 feedingManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.velocity = Vector3.zero;
            }

            var grab = other.GetComponent<XRGrabInteractable>();
            if (grab != null)
                grab.enabled = false;

            other.transform.position = transform.position;

            feedingManager.OnFoodPlaced();
        }
    }
}