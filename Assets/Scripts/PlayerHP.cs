using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlayerStats : MonoBehaviour
{
    public int maxHealth = 100;
    public int health = 100;

    public int maxArmor = 30;
    public int armor = 30;
    
public TextMeshProUGUI healthText;
public TextMeshProUGUI armorText;


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
        if (healthText != null) healthText.text = "Health: " + health;
        if (armorText != null) armorText.text = "Armor: " + armor;
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
