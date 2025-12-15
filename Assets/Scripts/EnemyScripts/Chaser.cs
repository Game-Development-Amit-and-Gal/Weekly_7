using System;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Enemy AI component responsible for chasing the player.
/// Uses Unity's NavMeshAgent for navigation and smoothly rotates
/// to face the player while moving.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class Chaser : MonoBehaviour
{
    /// <summary>
    /// Reference to the player GameObject that this enemy chases.
    /// </summary>
    [Tooltip("The object that this enemy chases after")]
    [SerializeField]
    GameObject player = null;

    /// <summary>
    /// Cached player position (for display/debugging purposes only).
    /// </summary>
    [Header("These fields are for display only")]
    [SerializeField] private Vector3 playerPosition;

    /// <summary>
    /// Reference to the player's transform.
    /// </summary>
    [SerializeField] public Transform playerTransform;

    /// <summary>
    /// Animator controlling the enemy's animations.
    /// </summary>
    private Animator animator;

    /// <summary>
    /// NavMeshAgent used for pathfinding and movement.
    /// </summary>
    private NavMeshAgent navMeshAgent;

    /// <summary>
    /// Controls how fast the enemy rotates to face the player.
    /// </summary>
    private float rotationRatio = 5f;

    /// <summary>
    /// Initializes required components.
    /// </summary>
    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Updates the enemy behavior every frame:
    /// - updates the player's position,
    /// - rotates the enemy to face the player,
    /// - moves the enemy toward the player using NavMesh.
    /// </summary>
    private void Update()
    {
        if (playerPosition == null || !playerTransform)
        {
            return;
        }

        playerPosition = player.transform.position;
        FacePlayer();
        navMeshAgent.SetDestination(playerPosition);
    }

    /// <summary>
    /// Smoothly rotates the enemy to face the player on the horizontal plane.
    /// </summary>
    private void FacePlayer()
    {
        Vector3 direction = (playerPosition - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        // Smooth rotation toward the player
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            lookRotation,
            Time.deltaTime * rotationRatio
        );
    }

    /// <summary>
    /// Returns the current target position.
    /// If the player reference is missing, returns the enemy's own position.
    /// </summary>
    /// <returns>World-space position of the chase target.</returns>
    internal Vector3 TargetObjectPosition()
    {
        if (player == null)
        {
            return transform.position;
        }
        else
        {
            return player.transform.position;
        }
    }
}
