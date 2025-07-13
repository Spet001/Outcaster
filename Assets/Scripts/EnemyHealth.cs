using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Configurações de Vida")]
    public int maxHealth = 100; // Vida máxima do inimigo
    private int currentHealth;  // Vida atual do inimigo

    void Start()
    {
        currentHealth = maxHealth; // Inicia com vida máxima
        Debug.Log(gameObject.name + " initialized with " + currentHealth + " health.");
    }

    // Método para o inimigo receber dano
    public void TakeDamage(int amount)
    {
        currentHealth -= amount; // Reduz a vida

        Debug.Log(gameObject.name + " took " + amount + " damage. Current health: " + currentHealth);

        // Verifica se a vida chegou a zero ou menos
        if (currentHealth <= 0)
        {
            Die(); // Chama o método de morte
        }
    }

    // Método chamado quando a vida do inimigo chega a zero
    void Die()
    {
        Debug.Log(gameObject.name + " has died!");
        // Aqui você adicionaria a lógica para a morte do inimigo:
        // - Desativar o GameObject
        // - Tocar animação de morte
        // - Instanciar efeito de explosão/partículas
        // - Dropar itens
        // - Destruir o GameObject (se não for reutilizado)
        Destroy(gameObject); // Por enquanto, apenas destrói o GameObject
    }
}