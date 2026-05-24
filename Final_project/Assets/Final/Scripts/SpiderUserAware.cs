using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 사용자(플레이어 / VR 카메라)와의 거리에 따라 거미가 정중하게 반응한다.
/// 절대 사용자를 쫓아가거나 공격하지 않는다.
///
///   거리 d  >= freezeRadius : 평상시 SpiderWander 그대로 동작
///   freezeRadius > d > retreatRadius : 그 자리에 멈춤(원하면 사용자 쪽으로 시선만 천천히 돌림)
///   d <= retreatRadius : 천천히 사용자 반대 방향으로 조금만 물러남(공포 표현 X, 그냥 공간 양보)
///
/// 사용법:
///  1) SpiderWander와 같은 GameObject에 붙임
///  2) user 슬롯에 플레이어/카메라 Transform 드래그
///  3) freezeRadius / retreatRadius 값을 시뮬레이션 단계에 맞춰 조정
///     (친해지는 단계 초반엔 크게, 후반엔 작게)
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(SpiderWander))]
public class SpiderUserAware : MonoBehaviour
{
    [Header("참조")]
    [Tooltip("사용자(플레이어/VR 카메라)의 Transform. 비어있으면 Camera.main을 자동 사용.")]
    public Transform user;

    [Header("Comfort Zone (사용자 친화)")]
    [Tooltip("이 거리 안에 사용자가 들어오면 거미가 그 자리에 멈춘다(m).")]
    public float freezeRadius = 1.5f;
    [Tooltip("이 거리 안에 사용자가 들어오면 거미가 천천히 뒤로 물러난다(m).")]
    public float retreatRadius = 0.7f;
    [Tooltip("물러나는 속도(m/s). 느릴수록 덜 위협적.")]
    public float retreatSpeed = 0.35f;
    [Tooltip("한 번에 물러나는 거리(m). 짧을수록 패닉 같지 않고 자연스러움.")]
    public float retreatStep = 0.6f;

    [Header("Curious Look (멈췄을 때만)")]
    [Tooltip("멈춰 있을 때 사용자 쪽으로 천천히 시선을 돌릴지 여부. 친밀감 단계에서 유용.")]
    public bool lookAtUserWhenStill = true;
    [Tooltip("시선을 돌리는 속도. 크면 천천히 돌아봄(부드러움).")]
    public float lookSmoothing = 1.5f;

    private NavMeshAgent agent;
    private SpiderWander wander;
    private enum Mode { Normal, Frozen, Retreating }
    private Mode mode = Mode.Normal;
    private float retreatCooldown;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        wander = GetComponent<SpiderWander>();
        if (user == null && Camera.main != null) user = Camera.main.transform;
    }

    void Update()
    {
        if (user == null) return;

        float d = Vector3.Distance(transform.position, user.position);

        if (d <= retreatRadius)        SetMode(Mode.Retreating);
        else if (d <= freezeRadius)    SetMode(Mode.Frozen);
        else                           SetMode(Mode.Normal);

        switch (mode)
        {
            case Mode.Retreating: TickRetreat(); break;
            case Mode.Frozen:     TickFrozen();  break;
            case Mode.Normal:     /* SpiderWander가 알아서 함 */ break;
        }
    }

    void SetMode(Mode next)
    {
        if (mode == next) return;
        mode = next;

        switch (mode)
        {
            case Mode.Normal:
                wander.enabled = true;
                agent.isStopped = false;
                agent.speed = wander.DesignedMoveSpeed; // 원래 속도로 복원
                break;
            case Mode.Frozen:
                wander.enabled = false;
                agent.isStopped = true;
                agent.ResetPath();
                break;
            case Mode.Retreating:
                wander.enabled = false;
                agent.isStopped = false;
                agent.speed = retreatSpeed;
                retreatCooldown = 0f;
                break;
        }
    }

    void TickFrozen()
    {
        if (lookAtUserWhenStill) SmoothLookAt(user.position);
    }

    void TickRetreat()
    {
        retreatCooldown -= Time.deltaTime;
        // 도착했거나 한 step 끝났으면 다음 목적지 갱신
        bool needNew = retreatCooldown <= 0f
                       || (!agent.pathPending && agent.remainingDistance < 0.05f);
        if (!needNew) return;

        Vector3 away = transform.position - user.position;
        away.y = 0f;
        if (away.sqrMagnitude < 0.0001f) away = Vector3.back;
        away.Normalize();

        Vector3 target = transform.position + away * retreatStep;
        if (NavMesh.SamplePosition(target, out var hit, 0.8f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        retreatCooldown = 0.4f; // 너무 자주 갱신하지 않게
    }

    void SmoothLookAt(Vector3 worldPoint)
    {
        Vector3 flat = worldPoint - transform.position;
        flat.y = 0f;
        if (flat.sqrMagnitude < 0.001f) return;
        Quaternion target = Quaternion.LookRotation(flat);
        transform.rotation = Quaternion.Slerp(
            transform.rotation, target,
            Time.deltaTime / Mathf.Max(0.01f, lookSmoothing));
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.85f, 0.2f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, freezeRadius);
        Gizmos.color = new Color(1f, 0.3f, 0.3f, 0.6f);
        Gizmos.DrawWireSphere(transform.position, retreatRadius);
    }
}
