using UnityEngine;

public class DynamicMusicManager : MonoBehaviour
{
    [Header("Referências")]
    public PlayerWeaponController weaponController;

    [Header("Músicas por Arma")]
    public AudioClip rocketStyleMusic;
    public AudioClip shotgunStyleMusic;
    public AudioClip katanaStyleMusic;
    public AudioClip chaingunStyleMusic;

    [Header("Áudio")]
    public AudioSource musicSource;
    [Range(0f, 1f)]
    public float musicVolume = 0.8f;

    private string currentWeaponName;

    void Start()
    {
        if (musicSource == null)
            musicSource = GetComponent<AudioSource>();

        musicSource.loop = true;
        musicSource.volume = musicVolume;
        currentWeaponName = "";
    }

    void Update()
    {
        if (weaponController == null || weaponController.currentWeaponIndex < 0)
            return;

        WeaponBase activeWeapon = weaponController.activeWeaponInstance;
        if (activeWeapon == null)
            return;

        string newWeapon = activeWeapon.weaponName;

        if (newWeapon != currentWeaponName)
        {
            currentWeaponName = newWeapon;
            PlayMusicForWeapon(newWeapon);
        }

        musicSource.volume = musicVolume; // Mantém volume sincronizado
    }

    void PlayMusicForWeapon(string weaponName)
    {
        AudioClip clipToPlay = null;

        switch (weaponName)
        {
            case "Rocket":
                clipToPlay = rocketStyleMusic;
                break;
            case "Shotgun":
                clipToPlay = shotgunStyleMusic;
                break;
            case "Katana":
                clipToPlay = katanaStyleMusic;
                break;
            case "Chaingun":
                clipToPlay = chaingunStyleMusic;
                break;
            default:
                Debug.Log("Nenhuma música atribuída para: " + weaponName);
                return;
        }

        if (clipToPlay != null && musicSource.clip != clipToPlay)
        {
            musicSource.clip = clipToPlay;
            musicSource.Play();
        }
    }
}
