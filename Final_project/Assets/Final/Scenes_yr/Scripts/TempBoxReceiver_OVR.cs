using UnityEngine;

public class TempBoxReceiver_OVR : MonoBehaviour
{
<<<<<<< HEAD
    [Header("씬별 매니저 — 해당 씬에 맞는 것 하나만 연결하세요")]
    public Scene09_Manager      sceneManager09Old;  // 기존 Scene09_Manager (레거시)
    public Scene09_CleanManager sceneManager09Clean; // Scene09_CleanSpiderHouse (신규)
=======
    [Header("씬별 매니저 — 해당 씬에 맞는 것만 연결하세요")]
    public Scene09_Manager sceneManager09;   // Scene09(기존 사용)
    public Scene07_Manager sceneManager07;   // Scene07(신규 사용)
>>>>>>> f0d904a (Fix: 스테이지 클리어 조건 추가)

    private bool spiderReceived = false;

    private void OnTriggerEnter(Collider other)
    {
        if (spiderReceived) return;
        if (!other.CompareTag("Spider")) return;

        spiderReceived = true;

        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        OVRGrabbable grab = other.GetComponent<OVRGrabbable>();
        if (grab != null) grab.enabled = false;

        other.transform.position = transform.position;
        other.gameObject.SetActive(false);

<<<<<<< HEAD
        if (sceneManager09Clean != null) sceneManager09Clean.OnSpiderInBox(other.gameObject);
        else if (sceneManager09Old  != null) sceneManager09Old.OnSpiderInBox(other.gameObject);
=======
        if (sceneManager07 != null) sceneManager07.OnSpiderInBox(other.gameObject);
        else if (sceneManager09 != null) sceneManager09.OnSpiderInBox(other.gameObject);
>>>>>>> f0d904a (Fix: 스테이지 클리어 조건 추가)
    }
}