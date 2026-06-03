using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// TempBox 안에 부착하는 거미 수신 트리거.
/// 거미(Tag="Spider")가 트리거에 들어오면 씬 매니저에 알립니다.
/// </summary>
public class TempBoxReceiver_OVR : MonoBehaviour
{
    [Header("씬별 매니저 — 해당 씬에 맞는 것 하나만 연결하세요")]
    public Scene09_Manager      sceneManager09Old;  // 기존 Scene09_Manager (레거시)
    public Scene09_CleanManager sceneManager09Clean; // Scene09_CleanSpiderHouse (신규)

    private bool spiderReceived = false;

    private void OnTriggerEnter(Collider other)
    {
        if (spiderReceived) return;

        GameObject spider = FindSpiderObject(other);
        if (spider == null) return;

        spiderReceived = true;

        Rigidbody rb = spider.GetComponent<Rigidbody>();
        if (rb == null && other.attachedRigidbody != null)
            rb = other.attachedRigidbody;

        if (rb != null) rb.isKinematic = true;

        NavMeshAgent agent = spider.GetComponent<NavMeshAgent>();
        if (agent != null) agent.enabled = false;

        SpiderWander wander = spider.GetComponent<SpiderWander>();
        if (wander != null) wander.enabled = false;

        SpiderGrabbable spiderGrabbable = spider.GetComponent<SpiderGrabbable>();
        if (spiderGrabbable != null) spiderGrabbable.enabled = false;

        OVRGrabbable grab = spider.GetComponent<OVRGrabbable>();
        if (grab != null) grab.enabled = false;

        spider.transform.position = transform.position;
        spider.SetActive(false);

        if (sceneManager09Clean != null) sceneManager09Clean.OnSpiderInBox(spider);
        else if (sceneManager09Old  != null) sceneManager09Old.OnSpiderInBox(spider);
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
}
