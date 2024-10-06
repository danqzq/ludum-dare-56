using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private int _health, _sanity, _cheese;
    [SerializeField] private int _maxHealth = 100, _maxSanity = 100, _maxCheese = 100;
    
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private UIManager _uiManager;
    
    [SerializeField] private VolumeProfile _volumeProfile;
    
    private Vignette _vignette;
    
    private AudioSource _hitSound;
    
    private Coroutine _hitCoroutine;
    
    private float _invincibilityTime;
    
    private bool _isGameOver;
    
    public int Health
    {
        get => _health;
        set
        {
            var originalHealth = _health;
            _health = value;
            _uiManager.UpdateHealthFill((float)_health / _maxHealth);
            if (_health <= 0)
            {
                _health = 0;
                _uiManager.UpdateHealthFill(0f);
                // Game Over
            }
            else if (originalHealth > _health)
            {
                if (_hitCoroutine != null)
                    StopCoroutine(_hitCoroutine);
                _hitCoroutine = StartCoroutine(OnHit());
                CameraMovement.DoShake(0.25f, 0.5f);
            }
        }
    }
    
    public int Sanity
    {
        get => _sanity;
        set
        {
            _sanity = value; 
            _uiManager.UpdateSanityFill((float)_sanity / _maxSanity);
        }
    }
    
    public int Cheese
    {
        get => _cheese;
        set
        {
            _cheese = value;
            _uiManager.UpdateCheeseFill((float)_cheese / _maxCheese);
        }
    }
    
    public static PlayerStats Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        
        _health = _maxHealth;
        _sanity = _maxSanity;
        _cheese = _maxCheese;
        
        _hitSound = GetComponent<AudioSource>();
        
        _vignette = _volumeProfile.TryGet(out Vignette vignette) ? vignette : null;
    }

    private IEnumerator OnHit()
    {
        _hitSound.Play();
        
        var time = 0f;
        _vignette.color.value = Color.red;
        while (time < 0.25f)
        {
            time += Time.deltaTime;
            _vignette.intensity.value = Mathf.Lerp(0.1f, 0.5f, time / 0.25f);
            yield return null;
        }
        time = 0f;
        while (time < 0.25f)
        {
            time += Time.deltaTime;
            _vignette.intensity.value = Mathf.Lerp(0.5f, 0.1f, time / 0.25f);
            _vignette.color.value = Color.Lerp(Color.red, Color.black, time / 0.25f);
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_invincibilityTime > 0f)
            return;
        if (other.CompareTag("Spike"))
        {
            Health -= 5;
            _invincibilityTime = 0.75f;
        }
    }

    private void Update()
    {
        if (_invincibilityTime > 0f)
            _invincibilityTime -= Time.deltaTime;

        if (_isGameOver)
            return;
        
        if (Health <= 0 || Sanity <= 0 || Cheese <= 0)
        {
            _isGameOver = true;
            _gameManager.OnGameOver();
        }
    }
}