using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public enum MonsterState { Idle, Detect, Chase }

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class MonsterBehavior : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float chaseSpeed = 3.5f;
    [SerializeField] private float idleSpeed = 1f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float acceleration = 8f;
    [SerializeField] private float stoppingDistance = 1.5f;

    [Header("Detection Settings")]
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private float fieldOfView = 90f;
    [SerializeField] private LayerMask obstructionLayers;
    [SerializeField] private float detectDuration = 1.5f;
    [SerializeField] private float losePlayerTimeout = 3f;

    [Header("Idle Settings")]
    [SerializeField] private float idleWanderRadius = 3f;
    [SerializeField] private float idleWanderInterval = 3f;
    [SerializeField] private float idleSpinSpeed = 30f; // Degrees per second
    [SerializeField] private float idleSpinAngle = 45f; // How much to spin left/right

    private NavMeshAgent agent;
    private Transform player;
    private Animator animator;
    private MonsterState currentState = MonsterState.Idle;
    private Vector3 lastKnownPlayerPosition;
    private float stateTimeElapsed;
    private bool hasReachedDestination;
    private float nextWanderTime;
    private Coroutine lookAroundCoroutine;
    private bool isSpinning;
    private float originalYRotation;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        originalYRotation = transform.eulerAngles.y;

        ConfigureAgent();
    }

    void ConfigureAgent()
    {
        agent.speed = chaseSpeed;
        agent.angularSpeed = 120f;
        agent.acceleration = acceleration;
        agent.stoppingDistance = stoppingDistance;
        agent.updateRotation = false;
    }

    void Update()
    {
        UpdateStateMachine();
        UpdateAnimations();
        CheckPathComplete();
        Debug.DrawLine(transform.position, agent.destination);
    }

    void CheckPathComplete()
    {
        if (agent.pathPending) return;

        hasReachedDestination = agent.remainingDistance <= agent.stoppingDistance
                                && !agent.hasPath
                                && agent.velocity.sqrMagnitude == 0f;
    }

    void UpdateStateMachine()
    {
        switch (currentState)
        {
            case MonsterState.Idle:
                HandleIdleState();
                break;

            case MonsterState.Detect:
                HandleDetectState();
                break;

            case MonsterState.Chase:
                HandleChaseState();
                break;
        }
        stateTimeElapsed += Time.deltaTime;
    }

    void HandleIdleState()
    {
        agent.speed = idleSpeed;
        agent.isStopped = false;

        // Wandering behavior
        if (Time.time >= nextWanderTime && !isSpinning)
        {
            WanderToNewPosition();
            nextWanderTime = Time.time + idleWanderInterval;
        }

        // Spin/look around behavior when not moving
        if (hasReachedDestination && !isSpinning)
        {
            if (lookAroundCoroutine != null)
            {
                StopCoroutine(lookAroundCoroutine);
            }
            lookAroundCoroutine = StartCoroutine(IdleLookAround());
        }

        // Transition to Detect if player is seen
        if (PlayerDetected())
        {
            if (lookAroundCoroutine != null)
            {
                StopCoroutine(lookAroundCoroutine);
                isSpinning = false;
            }
            ChangeState(MonsterState.Detect);
        }
    }

    void WanderToNewPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * idleWanderRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, idleWanderRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    IEnumerator IdleLookAround()
    {
        isSpinning = true;
        float lookDuration = 2f;
        float lookTime = 0f;
        float startRotation = transform.eulerAngles.y;
        float targetRotation = startRotation + Random.Range(-idleSpinAngle, idleSpinAngle);

        while (lookTime < lookDuration)
        {
            lookTime += Time.deltaTime;
            float t = lookTime / lookDuration;
            float currentRotation = Mathf.Lerp(startRotation, targetRotation, t);
            transform.rotation = Quaternion.Euler(0, currentRotation, 0);

            // Check for player detection while looking around
            if (PlayerDetected())
            {
                isSpinning = false;
                ChangeState(MonsterState.Detect);
                yield break;
            }

            yield return null;
        }

        // Return to original rotation
        lookTime = 0f;
        startRotation = transform.eulerAngles.y;

        while (lookTime < lookDuration / 2)
        {
            lookTime += Time.deltaTime;
            float t = lookTime / (lookDuration / 2);
            float currentRotation = Mathf.Lerp(startRotation, originalYRotation, t);
            transform.rotation = Quaternion.Euler(0, currentRotation, 0);
            yield return null;
        }

        isSpinning = false;
    }

    void HandleDetectState()
    {
        agent.isStopped = true;
        FacePlayer();

        if (stateTimeElapsed > detectDuration && PlayerDetected())
        {
            ChangeState(MonsterState.Chase);
        }
        else if (!PlayerDetected())
        {
            ChangeState(MonsterState.Idle);
        }
    }

    void HandleChaseState()
    {
        // Only chase while player is in sight
        if (!PlayerDetected())
        {
            ChangeState(MonsterState.Idle);
            return;
        }

        agent.speed = chaseSpeed;
        agent.isStopped = false;

        // Continuously update destination and face player
        lastKnownPlayerPosition = player.position;
        agent.SetDestination(lastKnownPlayerPosition);
        FacePlayer();
    }

    bool PlayerDetected()
    {
        if (player == null) return false;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > detectionRadius) return false;

        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        if (Vector3.Angle(transform.forward, dirToPlayer) > fieldOfView / 2) return false;

        if (!Physics.Raycast(transform.position, dirToPlayer, distance, obstructionLayers))
        {
            lastKnownPlayerPosition = player.position;
            return true;
        }
        return false;
    }

    void FacePlayer()
    {
        if (player == null) return;

        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    void UpdateAnimations()
    {
        animator.SetBool("Detecting", currentState == MonsterState.Detect);
        animator.SetBool("Chasing", currentState == MonsterState.Chase);
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    void ChangeState(MonsterState newState)
    {
        currentState = newState;
        stateTimeElapsed = 0f;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Vector3 fovLine1 = Quaternion.AngleAxis(fieldOfView / 2, Vector3.up) * transform.forward * detectionRadius;
        Vector3 fovLine2 = Quaternion.AngleAxis(-fieldOfView / 2, Vector3.up) * transform.forward * detectionRadius;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + fovLine1);
        Gizmos.DrawLine(transform.position, transform.position + fovLine2);

        // Draw idle wander radius
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, idleWanderRadius);
    }
}