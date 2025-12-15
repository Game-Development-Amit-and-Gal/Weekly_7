using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Enemy AI component responsible for random patrolling behavior.
/// The NPC moves between predefined targets using a NavMeshAgent.
/// Targets are collected from children objects containing a Target component
/// under a specified folder.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class Patroller : MonoBehaviour
{
    /// <summary>
    /// Minimum time (in seconds) to wait at a target before moving to the next one.
    /// </summary>
    [Tooltip("Minimum time to wait at target between running to the next target")]
    [SerializeField] private float minWaitAtTarget = 3f;

    /// <summary>
    /// Maximum time (in seconds) to wait at a target before moving to the next one.
    /// </summary>
    [Tooltip("Maximum time to wait at target between running to the next target")]
    [SerializeField] private float maxWaitAtTarget = 7f;

    /// <summary>
    /// Parent object whose children contain Target components.
    /// Each child represents a valid patrol destination.
    /// </summary>
    [Tooltip("A game object whose children have a Target component. Each child represents a target.")]
    [SerializeField] private Transform targetFolder = null;

    /// <summary>
    /// Cached list of all available patrol targets.
    /// </summary>
    private Target[] allTargets = null;

    /// <summary>
    /// Currently selected patrol target (for debugging/inspection).
    /// </summary>
    [Header("For debugging")]
    [SerializeField] private Target currentTarget = null;

    /// <summary>
    /// Remaining time to wait at the current target (for debugging/inspection).
    /// </summary>
    [SerializeField] private float timeToWaitAtTarget = 0;

    /// <summary>
    /// NavMeshAgent used for pathfinding and movement.
    /// </summary>
    private NavMeshAgent navMeshAgent;

    /// <summary>
    /// Animator controlling patrol-related animations.
    /// </summary>
    private Animator animator;

    /// <summary>
    /// Speed factor for smooth rotation toward the next destination.
    /// </summary>
    private float rotationSpeed = 5f;

    /// <summary>
    /// Initializes the patrol system by:
    /// - caching required components,
    /// - collecting all valid patrol targets,
    /// - selecting the initial patrol destination.
    /// </summary>
    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Retrieve all active Target components under the target folder
        allTargets = targetFolder.GetComponentsInChildren<Target>(false);
        Debug.Log("Found " + allTargets.Length + " active targets.");

        SelectNewTarget();
    }

    /// <summary>
    /// Randomly selects a new patrol target and sets it
    /// as the NavMeshAgent's destination.
    /// Also assigns a random waiting time at the target.
    /// </summary>
    private void SelectNewTarget()
    {
        int randomIndex = Random.Range(0, allTargets.Length);
        currentTarget = allTargets[randomIndex];

        Debug.Log("New target: " + currentTarget.name);
        Debug.Log("Index : " + randomIndex);

        navMeshAgent.SetDestination(currentTarget.transform.position);
        timeToWaitAtTarget = Random.Range(minWaitAtTarget, maxWaitAtTarget);
    }

    /// <summary>
    /// Updates the patrol behavior each frame:
    /// - rotates toward the destination while moving,
    /// - waits at the target when arrived,
    /// - selects a new target after the wait time expires.
    /// </summary>
    private void Update()
    {
        if (navMeshAgent.hasPath)
        {
            FaceDestination();
        }
        else
        {
            // Enemy has reached the target
            timeToWaitAtTarget -= Time.deltaTime;
            if (timeToWaitAtTarget <= 0)
                SelectNewTarget();
        }
    }

    /// <summary>
    /// Smoothly rotates the NPC to face its current NavMesh destination.
    /// Rotation is constrained to the horizontal plane.
    /// </summary>
    private void FaceDestination()
    {
        Vector3 directionToDestination =
            (navMeshAgent.destination - transform.position).normalized;

        Quaternion lookRotation =
            Quaternion.LookRotation(
                new Vector3(directionToDestination.x, 0, directionToDestination.z)
            );

        // Gradual rotation toward destination
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            lookRotation,
            Time.deltaTime * rotationSpeed
        );
    }
}
