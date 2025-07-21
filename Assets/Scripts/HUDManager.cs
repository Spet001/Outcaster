using UnityEngine;
using TMPro; // Importante para TextMeshPro
using UnityEngine.UI; // Para Image (portrait)

public class HUDManager : MonoBehaviour
{
    [Header("Elementos UI do HUD (Arma e Portrait)")]
    public TextMeshProUGUI weaponNameText; // Arraste seu TextMeshPro Text para o nome da arma
    // --- REMOVIDO: Elemento de texto para munição ---
    // public TextMeshProUGUI ammoCountText;
    // --- FIM REMOVIDO ---

    // --- NOVO: Elemento de Imagem para o Portrait do Jogador ---
    public Image playerPortraitImage; // Arraste a imagem do portrait aqui
    // --- FIM NOVO ---

    // --- Métodos Públicos para Atualizar o HUD ---

    /// <summary>
    /// Atualiza o texto do nome da arma no HUD.
    /// </summary>
    /// <param name="weaponName">O nome da arma a ser exibido.</param>
    public void UpdateWeaponNameDisplay(string weaponName)
    {
        if (weaponNameText != null)
        {
            weaponNameText.text = "EQUIPPED: " + weaponName.ToUpper(); // Convertendo para maiúsculas como no esboço
        }
        else
        {
            Debug.LogWarning("HUDManager: weaponNameText não atribuído! O nome da arma não será exibido.");
        }
    }

    // --- REMOVIDO: Método para atualizar o display de munição ---
    // public void UpdateAmmoDisplay(int currentAmmo, int maxAmmo)
    // {
    //     if (ammoCountText != null)
    //     {
    //         ammoCountText.text = currentAmmo + " / " + maxAmmo;
    //     }
    //     else
    //     {
    //         Debug.LogWarning("HUDManager: ammoCountText não atribuído! A munição não será exibida.");
    //     }
    // }
    // --- FIM REMOVIDO ---

    // --- NOVO: Método para atualizar o Portrait do Jogador ---
    /// <summary>
    /// Atualiza o sprite da imagem do portrait do jogador no HUD.
    /// </summary>
    /// <param name="portraitSprite">O sprite do portrait a ser exibido.</param>
    public void UpdatePlayerPortrait(Sprite portraitSprite)
    {
        if (playerPortraitImage != null)
        {
            playerPortraitImage.sprite = portraitSprite;
            playerPortraitImage.enabled = (portraitSprite != null); // Esconde a imagem se não houver sprite
        }
        else
        {
            Debug.LogWarning("HUDManager: playerPortraitImage não atribuído! O portrait do jogador não será exibido.");
        }
    }
    // --- FIM NOVO ---
}