using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField] int maxHealth = 100;
    int currentHealth;

    [SerializeField] Image healthFill;

    void Awake()
    {
        currentHealth = maxHealth;
        UpdateBar();
    }

    public void ReduceHealth(int amount = 10)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);
        UpdateBar();
    }

    void UpdateBar()
    {
        healthFill.fillAmount = (float)currentHealth / maxHealth;
    }

    public int getHealth => currentHealth;
}
