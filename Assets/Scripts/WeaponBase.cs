using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    [Header("Configuração Básica da Arma")]
    public string weaponName = "Shotgun";
    public float fireRate = 0.5f;
    protected float nextFireTime;

    // --- CAMPOS PARA CONTROLE DE MUNIÇÃO ---
    public int maxAmmo = 30;
    protected int currentAmmo;
    // --- FIM CAMPOS PARA MUNIÇÃO ---

    [Header("Tipo de Disparo")]
    public bool isHitscan = true;
    public float hitscanRange = 500f;
    public int hitscanDamage = 50;

    public GameObject projectilePrefab;
    public int projectileDamage = 40;

    [Header("Referências da Arma")]
    public Transform firePoint;
    public GameObject muzzleFlashPrefab;

    [Header("Som da Arma")]
    public AudioClip shootSound;
    protected AudioSource audioSource;

    public Animator weaponAnimator;
    public string shootAnimationTrigger = "Shoot";

    [Header("Configurações de Auto-Aim (Vertical Assist)")]
    public bool enableVerticalAutoAim = true;
    public float autoAimRange = 30f;
    public float autoAimVerticalTolerance = 5f;
    public float autoAimHorizontalAngle = 15f;

    [HideInInspector]
    public Camera playerCamera;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        currentAmmo = maxAmmo;

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
            if (playerCamera == null)
            {
                Debug.LogError("WeaponBase: Nenhuma câmera com a tag 'MainCamera' encontrada na cena! O auto-aim vertical não funcionará.", this);
            }
        }
    }

    public void TryFire()
    {
        if (Time.time >= nextFireTime && currentAmmo > 0)
        {
            currentAmmo--;
            nextFireTime = Time.time + fireRate;

            if (muzzleFlashPrefab != null)
            {
                GameObject flash = Instantiate(muzzleFlashPrefab, firePoint.position, firePoint.rotation);
                Destroy(flash, 0.1f);
            }
            if (audioSource != null && shootSound != null)
            {
                audioSource.PlayOneShot(shootSound);
            }
            if (weaponAnimator != null)
            {
                weaponAnimator.SetTrigger(shootAnimationTrigger);
            }

            Vector3 shootDirection = firePoint.forward;
            Transform targetToAimAt = FindAutoAimTarget();

            if (targetToAimAt != null)
            {
                shootDirection = (targetToAimAt.position - firePoint.position).normalized;
                Debug.Log(weaponName + ": Auto-aim ativado! Mirando em: " + targetToAimAt.name);
            }

            if (isHitscan)
            {
                RaycastHit hit;
                if (Physics.Raycast(firePoint.position, shootDirection, out hit, hitscanRange))
                {
                    Debug.Log(weaponName + " Hit: " + hit.collider.name + " at " + hit.point);
                    if (hit.collider.CompareTag("Enemy"))
                    {
                        EnemyHealth enemyHealth = hit.collider.GetComponent<EnemyHealth>();
                        if (enemyHealth != null)
                        {
                            enemyHealth.TakeDamage(hitscanDamage);
                        }
                    }
                }
                else
                {
                    Debug.Log(weaponName + " Missed!");
                }
            }
            else
            {
                if (projectilePrefab != null)
                {
                    GameObject projectileInstance = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(shootDirection));

                    PlayerProjectile playerProj = projectileInstance.GetComponent<PlayerProjectile>();
                    if (playerProj != null)
                    {
                        playerProj.damage = projectileDamage;
                    }
                }
                else
                {
                    Debug.LogWarning(weaponName + ": Projectile Prefab not assigned for non-hitscan weapon!");
                }
            }
            Debug.Log(weaponName + " Current Ammo: " + currentAmmo);
        }
        else if (currentAmmo <= 0)
        {
            Debug.Log(weaponName + ": No Ammo!");
        }
    }

    protected Transform FindAutoAimTarget()
    {
        if (!enableVerticalAutoAim || playerCamera == null)
        {
            // DEBUG: Se a câmera for nula, isso deveria ter sido pego pelo Debug.LogError no Awake.
            Debug.LogWarning("Auto-aim desativado ou playerCamera é nula. enableVerticalAutoAim: " + enableVerticalAutoAim + ", playerCamera: " + (playerCamera != null ? playerCamera.name : "NULL"));
            return null;
        }

        // --- DEBUG: LOGA O PONTO DE PARTIDA DO OVERLAPSPHERE E O ALCANCE ---
        Debug.Log("FindAutoAimTarget: Buscando inimigos na Layer 'Enemy' a partir de " + playerCamera.transform.position + " com alcance de " + autoAimRange);
        // --- FIM DEBUG ---

        Collider[] hitColliders = Physics.OverlapSphere(playerCamera.transform.position, autoAimRange, LayerMask.GetMask("Enemy"));

        // --- DEBUG: LOGA QUANTOS COLLIDERS FORAM ENCONTRADOS INICIALMENTE ---
        Debug.Log("FindAutoAimTarget: OverlapSphere encontrou " + hitColliders.Length + " colliders.");
        if (hitColliders.Length == 0)
        {
             // DEBUG: Se nenhum collider for encontrado aqui, o problema é na OverlapSphere ou Layer da torreta.
             Debug.LogWarning("FindAutoAimTarget: Nenhum collider encontrado pela OverlapSphere na Layer 'Enemy'. Verifique a Layer e o Collider da torreta!");
        }
        // --- FIM DEBUG ---

        Transform bestTarget = null;
        float closestAngle = float.MaxValue;

        foreach (var hitCollider in hitColliders)
        {
            // --- DEBUG: LOGA CADA COLLIDER ENCONTRADO ---
            Debug.Log("FindAutoAimTarget: Avaliando collider: " + hitCollider.name + " (Tag: " + hitCollider.tag + ", Layer: " + LayerMask.LayerToName(hitCollider.gameObject.layer) + ")");
            // --- FIM DEBUG ---

            if (!hitCollider.CompareTag("Enemy"))
            {
                // --- DEBUG: LOGA SE O COLLIDER NÃO TEM A TAG "Enemy" ---
                Debug.Log("FindAutoAimTarget: Collider " + hitCollider.name + " não tem a Tag 'Enemy', ignorando.");
                // --- FIM DEBUG ---
                continue;
            }

            Vector3 directionToTarget = (hitCollider.transform.position - playerCamera.transform.position).normalized;

            Vector3 cameraForwardFlat = new Vector3(playerCamera.transform.forward.x, 0, playerCamera.transform.forward.z).normalized;
            Vector3 directionToTargetFlat = new Vector3(directionToTarget.x, 0, directionToTarget.z).normalized;

            float angleToTarget = Vector3.Angle(cameraForwardFlat, directionToTargetFlat);

            // --- DEBUG: LOGA ÂNGULO E DIFERENÇA VERTICAL ---
            float verticalDifference = Mathf.Abs(hitCollider.transform.position.y - playerCamera.transform.position.y);
            Debug.Log("FindAutoAimTarget: " + hitCollider.name + " - Angle: " + angleToTarget + " (Max: " + autoAimHorizontalAngle + "), Vertical Diff: " + verticalDifference + " (Max: " + autoAimVerticalTolerance + ")");
            // --- FIM DEBUG ---

            if (angleToTarget <= autoAimHorizontalAngle)
            {
                if (verticalDifference <= autoAimVerticalTolerance)
                {
                    if (angleToTarget < closestAngle)
                    {
                        closestAngle = angleToTarget;
                        bestTarget = hitCollider.transform;
                        // --- DEBUG: LOGA QUE UM NOVO MELHOR ALVO FOI ENCONTRADO ---
                        Debug.Log("FindAutoAimTarget: Novo melhor alvo encontrado: " + bestTarget.name);
                        // --- FIM DEBUG ---
                    }
                }
                else
                {
                    // --- DEBUG: LOGA SE A DIFERENÇA VERTICAL ESTÁ FORA ---
                    Debug.Log("FindAutoAimTarget: " + hitCollider.name + " fora da tolerância vertical.");
                    // --- FIM DEBUG ---
                }
            }
            else
            {
                // --- DEBUG: LOGA SE O ÂNGULO HORIZONTAL ESTÁ FORA ---
                Debug.Log("FindAutoAimTarget: " + hitCollider.name + " fora do ângulo horizontal.");
                // --- FIM DEBUG ---
            }
        }
        // --- DEBUG: LOGA O ALVO FINAL RETORNADO ---
        Debug.Log("FindAutoAimTarget: Alvo final retornado: " + (bestTarget != null ? bestTarget.name : "NENHUM"));
        // --- FIM DEBUG ---
        return bestTarget;
    }
}