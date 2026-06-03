using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class SpiderHouseReceiver : MonoBehaviour
{
    public Scene08_Manager sceneManager;
    public Vector3 snapOffset = Vector3.zero;
    public bool parentSpiderToReceiver = true;
    public float settleLockSeconds = 3f;
    public float overlapCheckInterval = 0.1f;

    private bool spiderReceived = false;
    private Collider[] receiverColliders;
    private float nextOverlapCheck;

    private void Awake()
    {
        if (sceneManager == null)
            sceneManager = FindObjectOfType<Scene08_Manager>(true);

        receiverColliders = GetComponentsInChildren<Collider>(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryReceiveSpider(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryReceiveSpider(other);
    }

    private void Update()
    {
        if (spiderReceived || Time.time < nextOverlapCheck) return;
        nextOverlapCheck = Time.time + overlapCheckInterval;

        if (receiverColliders == null || receiverColliders.Length == 0)
            receiverColliders = GetComponentsInChildren<Collider>(true);

        foreach (Collider receiverCollider in receiverColliders)
        {
            if (receiverCollider == null || !receiverCollider.enabled) continue;

            foreach (Collider hit in GetOverlaps(receiverCollider))
            {
                if (TryReceiveSpider(hit))
                    return;
            }
        }
    }

    private bool TryReceiveSpider(Collider other)
    {
        if (spiderReceived || other == null) return false;
        if (other.transform == transform || other.transform.IsChildOf(transform)) return false;

        GameObject spider = FindSpiderObject(other);
        if (spider == null) return false;
        if (spider == gameObject || spider.transform.IsChildOf(transform)) return false;

        spiderReceived = true;

        FixSpiderInTerrarium(spider, other);
        StartCoroutine(KeepSpiderFixedForMoment(spider));

        if (sceneManager != null)
            sceneManager.OnSpiderInHouse(spider);
        else
            Debug.LogWarning("[SpiderHouseReceiver] sceneManager is not assigned.", this);

        return true;
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

        NavMeshAgent agent = other.GetComponentInParent<NavMeshAgent>();
        if (agent != null && LooksLikeSpider(agent.gameObject))
            return agent.gameObject;

        Transform root = other.transform.root;
        if (root != null && root.CompareTag("Spider"))
            return root.gameObject;

        if (root != null && LooksLikeSpider(root.gameObject))
            return root.gameObject;

        if (LooksLikeSpider(other.gameObject))
            return other.gameObject;

        return null;
    }

    private void FixSpiderInTerrarium(GameObject spider, Collider triggerCollider)
    {
        foreach (NavMeshAgent agent in spider.GetComponentsInChildren<NavMeshAgent>(true))
            agent.enabled = false;

        foreach (SpiderWander wander in spider.GetComponentsInChildren<SpiderWander>(true))
            wander.enabled = false;

        foreach (SpiderUserAware userAware in spider.GetComponentsInChildren<SpiderUserAware>(true))
            userAware.enabled = false;

        foreach (SpiderGrabbable grabbable in spider.GetComponentsInChildren<SpiderGrabbable>(true))
            grabbable.enabled = false;

        foreach (XRGrabInteractable xrGrab in spider.GetComponentsInChildren<XRGrabInteractable>(true))
            xrGrab.enabled = false;

        foreach (OVRGrabbable ovrGrab in spider.GetComponentsInChildren<OVRGrabbable>(true))
            ovrGrab.enabled = false;

        foreach (Rigidbody rb in spider.GetComponentsInChildren<Rigidbody>(true))
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        if (triggerCollider != null && triggerCollider.attachedRigidbody != null)
        {
            triggerCollider.attachedRigidbody.velocity = Vector3.zero;
            triggerCollider.attachedRigidbody.angularVelocity = Vector3.zero;
            triggerCollider.attachedRigidbody.isKinematic = true;
            triggerCollider.attachedRigidbody.useGravity = false;
            triggerCollider.attachedRigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }

        spider.transform.position = transform.position + snapOffset;

        if (parentSpiderToReceiver)
            spider.transform.SetParent(transform, true);
    }

    private IEnumerator KeepSpiderFixedForMoment(GameObject spider)
    {
        float endTime = Time.time + settleLockSeconds;
        while (spider != null && Time.time < endTime)
        {
            FixSpiderInTerrarium(spider, null);
            yield return null;
        }
    }

    private Collider[] GetOverlaps(Collider receiverCollider)
    {
        if (receiverCollider is BoxCollider box)
        {
            Vector3 center = box.transform.TransformPoint(box.center);
            Vector3 halfExtents = Vector3.Scale(box.size, box.transform.lossyScale) * 0.5f;
            return Physics.OverlapBox(center, halfExtents, box.transform.rotation, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide);
        }

        Bounds bounds = receiverCollider.bounds;
        return Physics.OverlapBox(bounds.center, bounds.extents, Quaternion.identity, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide);
    }

    private bool LooksLikeSpider(GameObject obj)
    {
        if (obj == null) return false;

        string objectName = obj.name.ToLowerInvariant();
        if (objectName.Contains("spiderhouse") || objectName.Contains("insidezone"))
            return false;

        if (objectName.Contains("spider") || objectName.Contains("black widow"))
            return true;

        return obj.GetComponentInChildren<SpiderGrabbable>(true) != null ||
               obj.GetComponentInChildren<SpiderWander>(true) != null;
    }
}
