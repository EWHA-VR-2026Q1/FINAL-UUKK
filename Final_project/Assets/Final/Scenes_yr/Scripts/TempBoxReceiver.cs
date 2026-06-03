using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Legacy receiver kept for scenes that still serialize the old TempBoxReceiver script.
/// New scenes can use TempBoxReceiver_OVR, but Step7/Step8 still reference this GUID.
/// </summary>
public class TempBoxReceiver : MonoBehaviour
{
    public Scene09_Manager sceneManager;

    [Header("Temp Spider Swap")]
    public GameObject replacementSpiderObject;
    public string replacementSpiderName = "Black_w_v_thisisfortemp";

    private bool spiderReceived = false;

    private void Awake()
    {
        LockTempBoxPhysics();

        if (replacementSpiderObject == null)
            replacementSpiderObject = FindSceneObjectByName(replacementSpiderName);

        if (replacementSpiderObject != null)
            replacementSpiderObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (spiderReceived) return;

        GameObject spider = FindSpiderObject(other);
        if (spider == null) return;
        if (spider == replacementSpiderObject) return;

        spiderReceived = true;

        DisableOriginalSpider(spider, other);
        ActivateReplacementSpider();

        if (sceneManager != null)
        {
            sceneManager.OnSpiderInBox(spider);
        }
        else
        {
            Debug.LogWarning("[TempBoxReceiver] sceneManager is not assigned.", this);
        }
    }

    private void LockTempBoxPhysics()
    {
        foreach (Rigidbody rb in GetComponentsInParent<Rigidbody>(true))
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = false;
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        foreach (Collider col in GetComponents<Collider>())
            col.isTrigger = true;

        foreach (Behaviour behaviour in GetComponentsInParent<Behaviour>(true))
        {
            string typeName = behaviour.GetType().Name;
            if (typeName == nameof(OVRGrabbable) || typeName == "XRGrabInteractable")
                behaviour.enabled = false;
        }
    }

    private GameObject FindSpiderObject(Collider other)
    {
        if (other.CompareTag("Spider"))
            return other.gameObject;

        if (other.attachedRigidbody != null && other.attachedRigidbody.CompareTag("Spider"))
            return other.attachedRigidbody.gameObject;

        SpiderGrabbable grabbable = other.GetComponentInParent<SpiderGrabbable>();
        if (grabbable != null)
            return grabbable.gameObject;

        Transform root = other.transform.root;
        if (root != null && root.CompareTag("Spider"))
            return root.gameObject;

        return null;
    }

    private void DisableOriginalSpider(GameObject spider, Collider triggerCollider)
    {
        foreach (Rigidbody rb in spider.GetComponentsInChildren<Rigidbody>(true))
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        if (triggerCollider.attachedRigidbody != null)
        {
            triggerCollider.attachedRigidbody.velocity = Vector3.zero;
            triggerCollider.attachedRigidbody.angularVelocity = Vector3.zero;
            triggerCollider.attachedRigidbody.isKinematic = true;
        }

        foreach (NavMeshAgent agent in spider.GetComponentsInChildren<NavMeshAgent>(true))
            agent.enabled = false;

        foreach (SpiderWander wander in spider.GetComponentsInChildren<SpiderWander>(true))
            wander.enabled = false;

        foreach (SpiderGrabbable grabbable in spider.GetComponentsInChildren<SpiderGrabbable>(true))
            grabbable.enabled = false;

        foreach (OVRGrabbable grabbable in spider.GetComponentsInChildren<OVRGrabbable>(true))
            grabbable.enabled = false;

        spider.SetActive(false);
    }

    private void ActivateReplacementSpider()
    {
        if (replacementSpiderObject == null)
            replacementSpiderObject = FindSceneObjectByName(replacementSpiderName);

        if (replacementSpiderObject == null)
        {
            Debug.LogWarning("[TempBoxReceiver] Replacement spider was not found.", this);
            return;
        }

        replacementSpiderObject.SetActive(true);
    }

    private GameObject FindSceneObjectByName(string objectName)
    {
        if (string.IsNullOrEmpty(objectName)) return null;

        foreach (Transform transformInScene in FindObjectsOfType<Transform>(true))
        {
            if (transformInScene.name == objectName)
                return transformInScene.gameObject;
        }

        return null;
    }
}
