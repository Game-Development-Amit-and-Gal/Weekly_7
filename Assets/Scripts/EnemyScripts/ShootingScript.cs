using System.Collections;
using UnityEngine;

/// <summary>
/// Handles shooting behavior for an enemy or weapon.
/// Responsible for:
/// - firing bullets at a fixed fire rate,
/// - managing ammunition and reload timing,
/// - spawning bullets from a muzzle point,
/// - applying forward velocity to bullets,
/// - destroying bullets after a fixed lifetime.
/// </summary>
public class ShootingScript : MonoBehaviour
{
    /// <summary>
    /// Bullet prefab to instantiate when shooting.
    /// </summary>
    [SerializeField] private GameObject prefab;

    /// <summary>
    /// Target transform (typically the player).
    /// Retrieved dynamically from the parent Chaser component.
    /// </summary>
    [SerializeField] private Transform target;

    /// <summary>
    /// Transform representing the muzzle position and direction.
    /// </summary>
    [SerializeField] private Transform Muzzle;

    /// <summary>
    /// Current ammunition count.
    /// </summary>
    private int ammoCount = 30;

    /// <summary>
    /// Maximum ammunition per magazine.
    /// </summary>
    private int ammoMag = 30;

    /// <summary>
    /// Time (in seconds) required to reload the weapon.
    /// </summary>
    private float reloadTime = 5f;

    /// <summary>
    /// Initial speed applied to each bullet.
    /// </summary>
    private float bulletSpeed = 20f;

    /// <summary>
    /// Minimum time (in seconds) between consecutive shots.
    /// </summary>
    private float fireCooldown = 0.3f;

    /// <summary>
    /// Internal timer used to enforce fire rate.
    /// </summary>
    private float fireTimer = 0f;

    /// <summary>
    /// Forward offset applied to the muzzle position when spawning bullets.
    /// </summary>
    private float offSet = 0.5f;

    /// <summary>
    /// Lifetime (in seconds) before a bullet is automatically destroyed.
    /// </summary>
    private float bulletDuration = 3f;

    /// <summary>
    /// Initializes the shooting system by retrieving
    /// the target reference from the parent Chaser component.
    /// </summary>
    void Awake()
    {
        target = GetComponentInParent<Chaser>().playerTransform;
    }

    /// <summary>
    /// Handles firing logic every frame:
    /// - checks if shooting is allowed,
    /// - enforces fire rate,
    /// - reloads when out of ammo,
    /// - spawns and launches bullets.
    /// </summary>
    private void Update()
    {
        int empty = 0;
        float cooldownTime = 0f;
        // Do not shoot if the parent Chaser is disabled
        if (!GetComponentInParent<Chaser>().enabled) return;

        fireTimer -= Time.deltaTime;
        if (fireTimer > cooldownTime) return;
        fireTimer = fireCooldown;

        // Reload when out of ammo
        if (ammoCount <= empty)
        {
            StartCoroutine(Reload());
            return;
        }

        // Spawn bullet at muzzle position with forward offset
        GameObject bullet = Instantiate(
            prefab,
            Muzzle.position + Muzzle.forward * offSet,
            Muzzle.rotation
        );

        // Schedule bullet destruction
        DestroyBulletAfterTime(bullet, bulletDuration);

        // Apply velocity to the bullet
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (!rb) return;

        rb.linearVelocity = Muzzle.forward * bulletSpeed;

        ammoCount--;

        // Debug ray to visualize shooting direction
        float debugMultipler = 5f;
        float duration = 1f;
        Debug.DrawRay(Muzzle.position, Muzzle.forward * debugMultipler, Color.red, duration);
    }

    /// <summary>
    /// Reloads the weapon after a fixed delay.
    /// </summary>
    private IEnumerator Reload()
    {
        yield return new WaitForSeconds(reloadTime);
        ammoCount = ammoMag;
    }

    /// <summary>
    /// Starts a coroutine to destroy a bullet after a given lifetime.
    /// </summary>
    /// <param name="bullet">Bullet GameObject to destroy.</param>
    /// <param name="lifetime">Time (in seconds) before destruction.</param>
    private void DestroyBulletAfterTime(GameObject bullet, float lifetime = 3f)
    {
        StartCoroutine(DestroyAfterTime(bullet, lifetime));
    }

    /// <summary>
    /// Coroutine that destroys a GameObject after a specified delay.
    /// </summary>
    private IEnumerator DestroyAfterTime(GameObject bullet, float time)
    {
        yield return new WaitForSeconds(time);
        if (bullet) Destroy(bullet);
    }
}
