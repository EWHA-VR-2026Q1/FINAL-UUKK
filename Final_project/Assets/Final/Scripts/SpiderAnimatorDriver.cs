using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// NavMeshAgent의 실제 속도를 Animator의 float 파라미터(Speed)로 전달한다.
/// → Animator에서 "Speed > 0.1" 같은 조건으로 Idle ↔ Walk 전환을 자동 처리할 수 있다.
///
/// 사용법:
///  1) Animator 컨트롤러에서 다음을 준비:
///       - Parameter: float "Speed"
///       - States: "Idle"(Idle.anim), "Walk"(Walk.anim)
///       - Transitions:
///           Idle → Walk : Speed Greater 0.1   (Has Exit Time 끔)
///           Walk → Idle : Speed Less 0.1      (Has Exit Time 끔)
///  2) 이 스크립트를 거미 GameObject에 붙임
///  3) animator 슬롯은 자식의 Animator를 자동으로 찾지만, 직접 드래그도 가능
///
/// 귀여운 거미(Spider1)의 AC.controller는 Idle 하나뿐이라 Walk 클립이 없음.
/// 그 경우는 이 스크립트를 안 붙이거나, AC.controller에 Walk 상태를 추가해야 함
/// (애니메이션이 없으면 위치만 움직이고 다리는 안 움직이는 형태가 됨).
/// </summary>
public class SpiderAnimatorDriver : MonoBehaviour
{
    [Header("참조")]
    [Tooltip("거미 모델의 Animator. 비어있으면 자식에서 자동으로 찾는다.")]
    public Animator animator;

    [Header("Animator 파라미터")]
    [Tooltip("Animator에 정의된 float 파라미터 이름.")]
    public string speedParam = "Speed";
    [Tooltip("Speed 값 보간 시간(초). 작을수록 즉각적, 클수록 부드럽게 변함.")]
    public float damping = 0.1f;

    private NavMeshAgent agent;
    private int speedHash;
    private bool hasSpeedParam;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponentInChildren<Animator>();

        if (animator != null)
        {
            speedHash = Animator.StringToHash(speedParam);
            foreach (var p in animator.parameters)
            {
                if (p.nameHash == speedHash && p.type == AnimatorControllerParameterType.Float)
                {
                    hasSpeedParam = true;
                    break;
                }
            }
            if (!hasSpeedParam)
            {
                Debug.LogWarning(
                    $"[SpiderAnimatorDriver] Animator에 float 파라미터 '{speedParam}' 가 없음. " +
                    "Animator 컨트롤러에 추가하거나 speedParam 이름을 바꿔주세요.", this);
            }
        }
    }

    void Update()
    {
        if (!hasSpeedParam || agent == null || animator == null) return;
        // 모델의 forward 방향과 무관하게 항상 실제 이동 속도를 사용
        // (모델이 뒤집혀 있어도 정상 동작 — SpiderFacingFix와 호환)
        float speed = agent.velocity.magnitude;
        animator.SetFloat(speedHash, speed, damping, Time.deltaTime);
    }
}
