using UnityEngine;

public class TestDummyEnemy : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float patrolDistance = 3f;
    public int maxHealth = 100;

    private Vector3 startPos;
    private int currentHealth;
    private int direction = 1;

    void Start()
    {
        startPos = transform.position;
        currentHealth = maxHealth;
    }

    void Update()
    {
        // Movimento simples de vai-e-volta
        transform.Translate(Vector3.right * direction * moveSpeed * Time.deltaTime);

        if (Mathf.Abs(transform.position.x - startPos.x) > patrolDistance)
        {
            direction *= -1;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Dummy HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Test dummy destroyed");
        Destroy(gameObject);
    }
}

