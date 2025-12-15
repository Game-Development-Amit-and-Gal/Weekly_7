using UnityEngine;

public class OnColliding : MonoBehaviour
{
    private string playerName = "Player";

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag(playerName)) return;

        Health playerHealth = collision.gameObject.GetComponent<Health>();
        if (!playerHealth) return;

        playerHealth.ReduceHealth();

        if (playerHealth.getHealth <= 0)
        {
            Destroy(collision.gameObject);
            Debug.Log("Player was KIA");
        }

        Destroy(gameObject); // destroy bullet
    }

}
