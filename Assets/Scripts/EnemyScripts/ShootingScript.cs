using System.Collections;
using UnityEngine;

public class ShootingScript : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform target;   // FIX 1: Transform, not Vector3
    [SerializeField] private Transform Muzzle;

    private int ammoCount = 30;
    private int ammoMag = 30;
    private float reloadTime = 5f;
    private float bulletSpeed = 20f;

    private float fireCooldown = 0.3f;           // FIX 2: fire rate
    private float fireTimer = 0f;
    private float offSet = 0.5f;
    private float bulletDuration = 3f;

    void Awake()
    {
        // FIX 1: get live target reference
        target = GetComponentInParent<Chaser>().playerTransform;


    }

    private void Update()
    {
        if (!GetComponentInParent<Chaser>().enabled) return;

        fireTimer -= Time.deltaTime;
        if (fireTimer > 0f) return;
        fireTimer = fireCooldown;

        if (ammoCount <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        GameObject bullet = Instantiate(
                prefab,
                Muzzle.position + Muzzle.forward * offSet,
                Muzzle.rotation
        );

        DestroyBulletAfterTime(bullet, bulletDuration);


        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (!rb) return;

        rb.linearVelocity = Muzzle.forward * bulletSpeed;

        ammoCount--;
        Debug.DrawRay(Muzzle.position, Muzzle.forward * 5f, Color.red, 1f);

    }

    private IEnumerator Reload()
    {
        yield return new WaitForSeconds(reloadTime);
        ammoCount = ammoMag;
    }
    private void DestroyBulletAfterTime(GameObject bullet, float lifetime = 3f)
    {
        StartCoroutine(DestroyAfterTime(bullet, lifetime));
    }

    private IEnumerator DestroyAfterTime(GameObject bullet, float time)
    {
        yield return new WaitForSeconds(time);
        if (bullet) Destroy(bullet);
    }

}
