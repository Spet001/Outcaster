using UnityEngine;
using UnityEngine.InputSystem;
using Game.Input; // <--- AGORA ESTA LINHA É CRÍTICA! Use o SEU namespace (ex: PlayerInputGenerated, etc.)

public class PlayerWeaponController : MonoBehaviour
{
    [Header("Armas do Jogador")]
    public WeaponBase[] weaponPrefabs;
    public int currentWeaponIndex = 0;

    private WeaponBase activeWeaponInstance;

    // A referência à classe C# gerada pelo Input Action Asset (com o namespace correto)
    private Game.Input.StarterAssetsInputs playerInputActions; // <--- QUALIFICANDO COM O NAMESPACE

    [SerializeField] private HUDManager HUDManager;
    void Awake()
    {
        // Instancia a classe gerada do Input System (com o namespace correto)
        playerInputActions = new Game.Input.StarterAssetsInputs();

        // Assina os callbacks (eventos) para as ações de input do Action Map 'Player'
        playerInputActions.Player.Fire.performed += ctx => OnFire();
        playerInputActions.Player.SwitchWeapon1.performed += ctx => EquipWeapon(0);
        playerInputActions.Player.SwitchWeapon2.performed += ctx => EquipWeapon(1);
    }

    void OnEnable()
    {
        playerInputActions.Enable();
    }

    void OnDisable()
    {
        playerInputActions.Disable();
    }

    void OnDestroy()
    {
        if (playerInputActions != null)
        {
            playerInputActions.Player.Fire.performed -= ctx => OnFire();
            playerInputActions.Player.SwitchWeapon1.performed -= ctx => EquipWeapon(0);
            playerInputActions.Player.SwitchWeapon2.performed -= ctx => EquipWeapon(1);

            playerInputActions.Dispose();
            playerInputActions = null;
        }
    }

    void Start()
    {
        if (weaponPrefabs == null || weaponPrefabs.Length == 0)
        {
            Debug.LogError("No weapon prefabs assigned to PlayerWeaponController!");
            return;
        }

        EquipWeapon(currentWeaponIndex);
    }

    void OnFire()
    {
        if (activeWeaponInstance != null)
        {
            activeWeaponInstance.TryFire();
        }
    }

    public void EquipWeapon(int index)
    {
        if (index < 0 || index >= weaponPrefabs.Length)
        {
            Debug.LogWarning("Invalid weapon index: " + index + ". Number of available weapons: " + weaponPrefabs.Length);
            return;
        }

        if (activeWeaponInstance != null)
        {
            Destroy(activeWeaponInstance.gameObject);
            activeWeaponInstance = null;
        }

        GameObject newWeaponGO = Instantiate(weaponPrefabs[index].gameObject, transform);
        activeWeaponInstance = newWeaponGO.GetComponent<WeaponBase>();

        if (activeWeaponInstance == null)
        {
            Debug.LogError("Instantiated weapon prefab does not have a WeaponBase component!", newWeaponGO);
            return;
        }

        currentWeaponIndex = index;
        Debug.Log("Equipped: " + activeWeaponInstance.weaponName);
    

    if (HUDManager != null)
        {
            HUDManager.UpdateWeaponNameDisplay(activeWeaponInstance.weaponName);
            // --- REMOVIDO: Inscrição e atualização de munição ---
            // activeWeaponInstance.OnAmmoChanged += hudManager.UpdateAmmoDisplay;
            // activeWeaponInstance.OnAmmoChanged?.Invoke(activeWeaponInstance.currentAmmo, activeWeaponInstance.maxAmmo);
            // --- FIM REMOVIDO ---

            // --- NOVO: Atualiza o portrait do jogador ---
            HUDManager.UpdatePlayerPortrait(activeWeaponInstance.associatedPortrait);
            // --- FIM NOVO ---
        }
    }



    public void AddWeaponToInventory(WeaponBase newWeaponPrefab)
    {
        Debug.Log("Weapon " + newWeaponPrefab.weaponName + " picked up! (Logic to add to inventory not fully implemented yet)");
    }
    

}