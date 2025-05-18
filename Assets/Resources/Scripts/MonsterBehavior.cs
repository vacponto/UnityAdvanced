using UnityEngine;
using UnityEngine.AI;

public enum MonsterState { Idle, Detect, Chase }

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class MonsterBehavior : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float chaseSpeed = 3.5f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float acceleration = 8f;
    [SerializeField] private float stoppingDistance = 1.5f;

    [Header("Detection Settings")]
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private float fieldOfView = 90f;
    [SerializeField] private LayerMask obstructionLayers;
    [SerializeField] private float detectDuration = 1.5f;
    [SerializeField] private float losePlayerTimeout = 3f;

    private NavMeshAgent agent;
    private Transform player;
    private Animator animator;
    private MonsterState currentState = MonsterState.Idle;
    private Vector3 lastKnownPlayerPosition;
    private float stateTimeElapsed;
    private bool hasReachedDestination;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

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
        agent.isStopped = true;
        if (PlayerDetected())
        {
            ChangeState(MonsterState.Detect);
        }
    }

    void HandleDetectState()
    {
        agent.isStopped = true;
        FacePlayer();

        if (stateTimeElapsed > detectDuration)
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
        agent.isStopped = false;

        if (PlayerDetected())
        {
            lastKnownPlayerPosition = player.position;
            agent.SetDestination(lastKnownPlayerPosition);
        }
        else if (hasReachedDestination || stateTimeElapsed > losePlayerTimeout)
        {
            ChangeState(MonsterState.Idle);
        }
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
    }
}