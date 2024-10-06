using DG.Tweening;
using UnityEngine;

public class Shotgun : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particles;
    [SerializeField] private Transform _model;
    [SerializeField] private GameObject _muzzleFlash;
    [SerializeField] private Light _spotlight;
    
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private AudioSource _shootSound, _ammoCollectSound;

    private static int _ammo;
    private static UIManager _uiManagerInstance;
    private static AudioSource _ammoCollectSoundInstance;

    private float _fireTime;
    
    public static bool isPickedUp;

    private void Start()
    {
        isPickedUp = true;
        
        _ammo = 10;
        _uiManagerInstance = _uiManager;
        _ammoCollectSoundInstance = _ammoCollectSound;
        _uiManager.ShowAmmoText();
        _uiManager.UpdateAmmoText(_ammo);
    }

    private void OnDestroy()
    {
        isPickedUp = false;
    }

    public static void AddAmmo(int amount)
    {
        _ammo += amount;
        _uiManagerInstance.UpdateAmmoText(_ammo);
        _ammoCollectSoundInstance.Play();
    }

    private void Update()
    {
        _spotlight.intensity = Mathf.Clamp(_spotlight.intensity - Time.deltaTime, 0, 1);
        
        if (_fireTime > 0f)
        {
            _uiManager.ToggleShootHint(false);
            _fireTime -= Time.deltaTime;
            return;
        }
        
        _uiManager.ToggleShootHint(true);
        
        if (_ammo <= 0)
        {
            return;
        }
        
        if (Input.GetMouseButtonDown(1))
        {
            _fireTime = 1.5f;
            _model.DOPunchPosition(new Vector3(-0.005f, 0, 0), 0.25f, 10, 1f).OnComplete(() =>
                _model.DOLocalRotate(new Vector3(0, -359.9f, 0), 1f).SetEase(Ease.Linear).SetRelative());
            _particles.Play();
            CameraMovement.DoShake(0.25f, 0.25f);
            _uiManager.UpdateAmmoText(--_ammo);
            _uiManager.FlashAmmoText(_ammo <= 0);
            _muzzleFlash.SetActive(true);
            _shootSound.Play();
            this.DoAfter(0.1f, () => _muzzleFlash.SetActive(false));
        }
    }
}