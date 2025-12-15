using UnityEngine;

/// <summary>
/// Handles collision behavior for a projectile or damaging object.
/// When colliding with the player, it reduces the player's health
/// and destroys itself. If the player's health reaches zero,
/// the player object is destroyed.
/// </summary>
public class OnColliding : MonoBehaviour
{
    /// <summary>
    /// Tag name used to identify the player object.
    /// </summary>
    private string playerName = "Player";

    /// <summary>
    /// Called automatically by Unity when this object
    /// collides with another collider.
    /// </summary>
    /// <param name="collision">
    /// Collision data containing information about the
    /// object that was hit.
    /// </param>
    private void OnCollisionEnter(Collision collision)
    {
        int min = 0; //Avoid magic numbers
        // Ignore collisions with objects that are not tagged as the player
        if (!collision.gameObject.CompareTag(playerName)) return;

        // Attempt to retrieve the Health component from the player
        Health playerHealth = collision.gameObject.GetComponent<Health>();
        if (!playerHealth) return;

        // Apply damage to the player
        playerHealth.ReduceHealth();

        // If health reaches zero or below, destroy the player
        if (playerHealth.getHealth <= min)
        {
            Destroy(collision.gameObject);
            Debug.Log("Player was KIA");
        }

        // Destroy this object (e.g., bullet) after collision
        Destroy(gameObject);
    }
}
