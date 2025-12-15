using UnityEngine;

/// <summary>
/// Marks a GameObject as a patrol or navigation target.
/// This component is mainly used for editor visualization
/// and can be referenced by other systems (e.g., patrolling logic).
/// 
/// A colored sphere is drawn in the Scene view to indicate
/// the target's position.
/// </summary>
public class Target : MonoBehaviour
{
    /// <summary>
    /// Radius of the gizmo sphere drawn in the Scene view.
    /// </summary>
    [SerializeField] float sphereRadius = 0.3f;

    /// <summary>
    /// Color of the gizmo sphere drawn in the Scene view.
    /// </summary>
    [SerializeField] Color sphereColor = Color.red;

    /// <summary>
    /// Draws a visual marker in the Scene view to represent
    /// this target's position.
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = sphereColor;
        Gizmos.DrawSphere(transform.position, sphereRadius);
    }
}
