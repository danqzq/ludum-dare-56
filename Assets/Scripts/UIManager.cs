using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _pickupHint, _throwHint, _shootHint, _bossHint;
    [SerializeField] private Transform _cursor;

    [SerializeField] private Image _healthFill, _cheeseFill, _sanityFill;
    [SerializeField] private TextMeshProUGUI _ammoText;
    [SerializeField] private TextMeshProUGUI _scoreText;

    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private CanvasGroup _gameOverMenu, _gameWinMenu;
    
    private AudioSource _buttonClickSound;
    
    private TextMeshProUGUI _healthText, _cheeseText, _sanityText;
    
    private bool _isPickupHintVisible, _isThrowHintVisible, _isShootHintVisible, _isBossHintVisible;
    
    private void Start()
    {
        _healthText = _healthFill.transform.parent.GetComponentInChildren<TextMeshProUGUI>();
        _cheeseText = _cheeseFill.transform.parent.GetComponentInChildren<TextMeshProUGUI>();
        _sanityText = _sanityFill.transform.parent.GetComponentInChildren<TextMeshProUGUI>();

        _buttonClickSound = GetComponent<AudioSource>();
        foreach (var button in FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            button.onClick.AddListener(() => _buttonClickSound.Play());
        }
    }
    
    public void TogglePauseMenu(bool state)
    {
        _pauseMenu.SetActive(state);
    }
    
    public void ShowAmmoText()
    {
        _ammoText.gameObject.SetActive(true);
    }
    
    public void FlashAmmoText(bool noAmmo)
    {
        if (noAmmo)
        {
            _ammoText.color = Color.red;
            this.DoAfter(0.2f, () => _ammoText.color = Color.white);
        }
        _ammoText.transform.DOKill();
        _ammoText.transform.localScale = Vector3.one;
        _ammoText.transform.DOPunchScale(Vector3.one * 0.25f, 0.25f);
    }
    
    public void UpdateScoreText(int score)
    {
        _scoreText.text = score.ToString();
        _scoreText.transform.DOKill();
        _scoreText.transform.localScale = Vector3.one;
        _scoreText.transform.DOPunchScale(Vector3.one * 0.25f, 0.25f);
    }
    
    public void UpdateAmmoText(int ammo)
    {
        _ammoText.text = ammo.ToString();
    }
    
    public void UpdateHealthFill(float value)
    {
        _healthFill.DOKill();
        _healthFill.DOFillAmount(value, 0.25f);
        _healthText.transform.DOKill();
        _healthText.transform.localScale = Vector3.one;
        _healthText.transform.DOPunchScale(Vector3.one * 0.25f, 0.25f);
    }
    
    public void UpdateCheeseFill(float value)
    {
        _cheeseFill.DOKill();
        _cheeseFill.DOFillAmount(value, 0.25f);
        _cheeseText.transform.DOKill();
        _cheeseText.transform.localScale = Vector3.one;
        _cheeseText.transform.DOPunchScale(Vector3.one * 0.25f, 0.25f);
    }
    
    public void UpdateSanityFill(float value)
    {
        _sanityFill.DOKill();
        _sanityFill.DOFillAmount(value, 0.25f);
        _sanityText.transform.DOKill();
        _sanityText.transform.localScale = Vector3.one;
        _sanityText.transform.DOPunchScale(Vector3.one * 0.25f, 0.25f);
    }
    
    public void TogglePickupHint(bool state)
    {
        if (_isPickupHintVisible == state) 
            return;
        _isPickupHintVisible = state;
        _pickupHint.DOKill();
        _pickupHint.DOFade(state ? 1f : 0f, 0.25f);
        _cursor.DOKill();
        _cursor.DOScale(state ? Vector3.one * 2f : Vector3.one, 0.25f);
    }

    private static int _hintCounter = 0;
    
    public void ToggleThrowHint(bool state)
    {
        if (_isThrowHintVisible == state) 
            return;
        _isThrowHintVisible = state;
        _hintCounter++;
        if (_hintCounter > 5) 
        {
            if (state)
                return;
        }
        _throwHint.DOKill();
        _throwHint.DOFade(state ? 1f : 0f, 0.25f);
    }
    
    private static int _hintCounter2 = 0;
    
    public void ToggleShootHint(bool state)
    {
        if (_isShootHintVisible == state) 
            return;
        _isShootHintVisible = state;
        _hintCounter2++;
        if (_hintCounter2 > 5)
        {
            if (state)
                return;
        }
        _shootHint.DOKill();
        _shootHint.DOFade(state ? 1f : 0f, 0.25f);
    }
    
    public void ToggleBossHint(bool state)
    {
        if (_isBossHintVisible == state) 
            return;
        _isBossHintVisible = state;
        _bossHint.DOKill();
        _bossHint.DOFade(state ? 1f : 0f, 0.25f);
    }
    
    public void ShowGameOverMenu()
    {
        _gameOverMenu.DOFade(1f, 1f).OnComplete(() =>
        {
            _gameOverMenu.interactable = true;
            _gameOverMenu.blocksRaycasts = true;
        });
    }
    
    public void ShowGameWinMenu()
    {
        _gameWinMenu.DOFade(1f, 1f).OnComplete(() =>
        {
            _gameWinMenu.interactable = true;
            _gameWinMenu.blocksRaycasts = true;
        });
    }

    private void OnDestroy()
    {
        _pickupHint.DOKill();
        _throwHint.DOKill();
        _shootHint.DOKill();
        _bossHint.DOKill();
        _healthFill.DOKill();
        _cheeseFill.DOKill();
        _sanityFill.DOKill();
        _ammoText.transform.DOKill();
        _scoreText.transform.DOKill();
    }
}