using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Controls the enemy behavior using a state machine.
/// The enemy can:
/// - patrol between predefined points,
/// - chase the player when within a detection radius,
/// - rotate randomly when idle.
/// State transitions are handled probabilistically and by distance checks.
/// </summary>
[RequireComponent(typeof(Patroller))]
[RequireComponent(typeof(Chaser))]
[RequireComponent(typeof(Rotator))]
public class EnemyControllerStateMachine : StateMachine
{
    /// <summary>
    /// Radius within which the enemy detects and starts chasing the player.
    /// </summary>
    [SerializeField] float radiusToWatch = 5f;

    /// <summary>
    /// Probability (per frame) for the enemy to start rotating while patrolling.
    /// </summary>
    [SerializeField] float probabilityToRotate = 0.2f;

    /// <summary>
    /// Probability (per frame) for the enemy to stop rotating and return to patrolling.
    /// </summary>
    [SerializeField] float probabilityToStopRotating = 0.2f;

    /// <summary>
    /// Reference to the player GameObject.
    /// Used to determine visibility and existence.
    /// </summary>
    [SerializeField] private GameObject player;

    /// <summary>
    /// Component responsible for chasing the target.
    /// </summary>
    private Chaser chaser;

    /// <summary>
    /// Component responsible for patrolling behavior.
    /// </summary>
    private Patroller patroller;

    /// <summary>
    /// Component responsible for rotation behavior.
    /// </summary>
    private Rotator rotator;

    /// <summary>
    /// Calculates the distance between the enemy and its current target.
    /// </summary>
    /// <returns>Distance to the target object.</returns>
    private float DistanceToTarget()
    {
        return Vector3.Distance(transform.position, chaser.TargetObjectPosition());
    }

    /// <summary>
    /// Initializes the state machine by:
    /// - retrieving required behavior components,
    /// - registering states,
    /// - defining transitions between states.
    /// </summary>
    private void Awake()
    {
        float upperBound = 1f;
        float lowerBound = 0f;
        chaser = GetComponent<Chaser>();
        patroller = GetComponent<Patroller>();
        rotator = GetComponent<Rotator>();

        this
            // Register states (Patroller is the initial state)
            .AddState(patroller)
            .AddState(chaser)
            .AddState(rotator)

            // Transition to chasing when the player is within detection radius
            .AddTransition(patroller, () => !player.IsDestroyed() && DistanceToTarget() <= radiusToWatch, chaser)
            .AddTransition(rotator, () => !player.IsDestroyed() && DistanceToTarget() <= radiusToWatch, chaser)

            // Return to patrolling when the player is gone or out of range
            .AddTransition(chaser, () => player.IsDestroyed() || DistanceToTarget() > radiusToWatch, patroller)

            // Probabilistic transitions between rotating and patrolling
            .AddTransition(rotator, () => Random.Range(lowerBound, upperBound) < probabilityToStopRotating * Time.deltaTime, patroller)
            .AddTransition(patroller, () => Random.Range(lowerBound, upperBound) < probabilityToRotate * Time.deltaTime, rotator);
    }

    /// <summary>
    /// Draws a visual representation of the detection radius in the editor
    /// when the enemy is selected.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radiusToWatch);
    }
}
