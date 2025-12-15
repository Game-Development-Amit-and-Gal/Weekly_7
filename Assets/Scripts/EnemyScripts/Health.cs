using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the health state of a game object.
/// Handles health initialization, health reduction,
/// and updates a UI health bar accordingly.
/// </summary>
public class Health : MonoBehaviour
{
    /// <summary>
    /// Maximum health value for this entity.
    /// </summary>
    [SerializeField] int maxHealth = 100;

    /// <summary>
    /// Current health value.
    /// </summary>
    int currentHealth;

    /// <summary>
    /// UI Image used to visually represent health (fill-based).
    /// </summary>
    [SerializeField] Image healthFill;

    /// <summary>
    /// Initializes the health system by setting current health
    /// to maximum and updating the UI bar.
    /// </summary>
    void Awake()
    {
        currentHealth = maxHealth;
        UpdateBar();
    }

    /// <summary>
    /// Reduces the current health by the given amount.
    /// Health value is clamped to zero.
    /// </summary>
    /// <param name="amount">
    /// Amount of health to reduce (default is 10).
    /// </param>
    public void ReduceHealth(int amount = 10)
    {
        int min = 0;
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, min);
        UpdateBar();
    }

    /// <summary>
    /// Updates the UI health bar fill amount
    /// based on the current health percentage.
    /// </summary>
    void UpdateBar()
    {
        healthFill.fillAmount = (float)currentHealth / maxHealth;
    }

    /// <summary>
    /// Gets the current health value.
    /// </summary>
    public int getHealth => currentHealth;
}
