using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 거미를 잡을 수 있게 만드는 컴포넌트.
/// EX_OVRInput_Grab(또는 호환되는 grab 스크립트)이 SendMessage로
/// "OnGrabbed" / "OnReleased" 를 호출하면 자동으로 반응한다.
///
/// 잡힐 때 일어나는 일:
///   - NavMeshAgent 비활성화 (그래야 Rigidbody가 손에 따라옴)
///   - SpiderWander / SpiderUserAware / SpiderFacingFix 비활성화 (충돌 방지)
///   - Animator는 그대로 → Speed 파라미터가 0이라 Idle 재생
///
/// 놓일 때 일어나는 일:
///   - 잠깐 기다림 (낙하 + 안정화)
///   - 가장 가까운 NavMesh 위치로 워프
///   - 모든 스크립트/Agent 다시 활성화
///
/// 사용법:
///   1) 거미 prefab의 루트에 이 컴포넌트 추가
///   2) Rigidbody가 없으면 자동 추가됨 (Use Gravity ✓ 권장)
///   3) Collider 필수 (CapsuleCollider 권장)
///   4) GameObject의 Layer를 "Grabbable" 같은 전용 레이어로 설정
///   5) EX_OVRInput_Grab의 GrabLayer에 그 레이어 체크
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class SpiderGrabbable : MonoBehaviour
{
    [Header("재활성화 타이밍")]
    [Tooltip("놓인 후 NavMesh에 복귀하기까지 대기 시간(초). 거미가 떨어져서 멈출 시간.")]
    public float restoreDelay = 1.0f;

    [Tooltip("놓인 위치 주변 이 반경 안에서 NavMesh 위치를 찾는다(m).")]
    public float navMeshSearchRadius = 3.0f;

    [Header("디버그")]
    public bool logEvents = false;

    private NavMeshAgent agent;
    private Rigidbody rb;
    private MonoBehaviour[] disableOnGrab;  // 잡혔을 때 끌 스크립트들
    private bool isGrabbed = false;
    private Coroutine restoreRoutine;
    private bool initialUseGravity;
    private bool initialIsKinematic;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        initialUseGravity = rb.useGravity;
        initialIsKinematic = rb.isKinematic;

        // 잡혔을 때 비활성화할 거미 movement 스크립트들 모으기
        // (이름으로 직접 참조하지 않고 GetComponent로 — SpiderFacingFix 없어도 OK)
        var components = new System.Collections.Generic.List<MonoBehaviour>();
        TryAdd<SpiderWander>(components);
        TryAdd<SpiderUserAware>(components);
        TryAdd<SpiderFacingFix>(components);
        TryAdd<SpiderAnimatorDriver>(components);
        disableOnGrab = components.ToArray();
    }

    void TryAdd<T>(System.Collections.Generic.List<MonoBehaviour> list) where T : MonoBehaviour
    {
        var c = GetComponent<T>();
        if (c != null) list.Add(c);
    }

    /// <summary>EX_OVRInput_Grab이 SendMessage로 호출.</summary>
    public void OnGrabbed()
    {
        if (logEvents) Debug.Log($"[SpiderGrabbable] {name} grabbed.", this);

        // 진행 중인 복원 코루틴 있으면 취소
        if (restoreRoutine != null)
        {
            StopCoroutine(restoreRoutine);
            restoreRoutine = null;
        }

        isGrabbed = true;

        // 모든 movement 스크립트 비활성화
        foreach (var mb in disableOnGrab)
            if (mb != null) mb.enabled = false;

        // NavMeshAgent도 끔 (그래야 위치/회전을 Rigidbody가 결정)
        if (agent != null) agent.enabled = false;
    }

    /// <summary>EX_OVRInput_Grab이 SendMessage로 호출.</summary>
    public void OnReleased()
    {
        if (logEvents) Debug.Log($"[SpiderGrabbable] {name} released.", this);

        isGrabbed = false;

        // EX_OVRInput_Grab은 release 순간 모든 Rigidbody를 던지는 방식으로 복구한다.
        // NavMeshAgent가 있는 거미는 physics와 agent가 싸우지 않도록 즉시 멈춰둔다.
        if (rb != null && agent != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        restoreRoutine = StartCoroutine(RestoreAfterLanding());
    }

    IEnumerator RestoreAfterLanding()
    {
        // 떨어지는 시간 + 안정화 대기
        yield return new WaitForSeconds(restoreDelay);

        // Rigidbody 멈추기 (떨림 방지)
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // 현재 위치 주변에서 가장 가까운 NavMesh 위치 찾기
        if (agent != null)
        {
            if (NavMesh.SamplePosition(transform.position, out var hit, navMeshSearchRadius, NavMesh.AllAreas))
            {
                transform.position = hit.position;
                agent.enabled = true;
                agent.Warp(hit.position);
                rb.useGravity = false;
                rb.isKinematic = true;
                if (logEvents) Debug.Log($"[SpiderGrabbable] Warped to NavMesh at {hit.position}.", this);
            }
            else
            {
                // NavMesh 못 찾으면 일단 Agent만 다시 켜기
                agent.enabled = true;
                rb.useGravity = initialUseGravity;
                rb.isKinematic = initialIsKinematic;
                Debug.LogWarning($"[SpiderGrabbable] {name}: Could not find nearby NavMesh. The spider may be outside the NavMesh.", this);
            }
        }
        else if (rb != null)
        {
            rb.useGravity = initialUseGravity;
            rb.isKinematic = initialIsKinematic;
        }

        // movement 스크립트 다시 켜기
        foreach (var mb in disableOnGrab)
            if (mb != null) mb.enabled = true;

        restoreRoutine = null;
    }

    void OnDisable()
    {
        // 거미가 비활성화될 때 정리
        if (restoreRoutine != null)
        {
            StopCoroutine(restoreRoutine);
            restoreRoutine = null;
        }
    }
}
