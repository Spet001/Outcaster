using UnityEngine;
using TMPro; // Para TextMeshProUGUI
using UnityEngine.UI; // Para Image (barras)

public class PlayerStats : MonoBehaviour
{
    public int maxHealth = 100;
    public int health = 100;

    public int maxArmor = 30;
    public int armor = 30;

    [Header("Elementos UI (Texto)")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI armorText;

    // --- NOVO: Elementos UI (Barras) ---
    [Header("Elementos UI (Barras)")]
    public Image healthBarImage; // Arraste a imagem da barra de vida aqui
    public Image armorBarImage;  // Arraste a imagem da barra de armadura aqui
    // --- FIM NOVO ---

    private void Start()
    {
        UpdateHUD();
    }

    public void TakeDamage(int damage)
    {
        if (armor > 0)
        {
            int armorAbsorb = Mathf.Min(damage, armor);
            armor -= armorAbsorb;
            damage -= armorAbsorb;
        }

        health -= damage;
        if (health <= 0)
        {
            Die();
        }

        UpdateHUD();
    }

    void Die()
    {
        Debug.Log("Player morreu!");
        // Aqui você pode desativar o controle, tocar uma animação, etc.
    }

    void UpdateHUD()
    {
        // Atualiza textos
        if (healthText != null) healthText.text = "Health: " + health;
        if (armorText != null) armorText.text = "Armor: " + armor;

        // --- NOVO: Atualiza barras de vida e armadura ---
        if (healthBarImage != null)
        {
            healthBarImage.fillAmount = (float)health / maxHealth;
        }
        if (armorBarImage != null)
        {
            armorBarImage.fillAmount = (float)armor / maxArmor;
        }
        // --- FIM NOVO ---
    }

    public void Heal(int amount)
    {
        health = Mathf.Min(health + amount, maxHealth);
        UpdateHUD();
    }

    public void AddArmor(int amount)
    {
        armor = Mathf.Min(armor + amount, maxArmor);
        UpdateHUD();
    }
}