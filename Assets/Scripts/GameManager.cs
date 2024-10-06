using Dan.Demo;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

using static Globals;

public class GameManager : MonoBehaviour
{
    [SerializeField] private bool _isTest;
    [SerializeField] private Image _fade;
    
    [SerializeField] private UIManager _uiManager;

    [SerializeField] private GameObject _exitButton, _exitButton2;

    [SerializeField] private LeaderboardShowcase _leaderboardHandler;
    
    [SerializeField] private GameObject _bloodEffectsOffButton, _soundEffectsOffButton, _musicOffButton;
    
    [SerializeField] private AudioMixerGroup _mixerGroup;
    
    private static bool _isGameOver;

    private static int _score;

    public static int Score
    {
        get => _score;
        set
        {
            if (_isGameOver) return;
            _score = value;
            _instance._uiManager.UpdateScoreText(_score);
        }
    }

    public bool IsPaused { get; private set; }
    
    private static GameManager _instance;

    private bool _isRestarting;

    private void Awake()
    {
        _score = 0;
        _isGameOver = false;
        _instance = this;
        
#if UNITY_WEBGL
        _exitButton.SetActive(false);
        _exitButton2.SetActive(true);
#endif
        
        _bloodEffectsOffButton.SetActive(PlayerPrefs.GetInt(PREFS_BLOOD_EFFECTS, 1) == 0);
        _soundEffectsOffButton.SetActive(PlayerPrefs.GetInt(PREFS_SOUND_EFFECTS, 1) == 0);
        _musicOffButton.SetActive(PlayerPrefs.GetInt(PREFS_MUSIC, 1) == 0);
        
        _mixerGroup.audioMixer.SetFloat("SFXVolume", PlayerPrefs.GetInt(PREFS_SOUND_EFFECTS, 1) == 0 ? -80f : 0f);
        _mixerGroup.audioMixer.SetFloat("MusicVolume", PlayerPrefs.GetInt(PREFS_MUSIC, 1) == 0 ? -80f : 0f);
    }
    
    public void SetBloodEffects(bool state)
    {
        PlayerPrefs.SetInt(PREFS_BLOOD_EFFECTS, !state ? 1 : 0);
    }
    
    public void SetSoundEffects(bool state)
    {
        PlayerPrefs.SetInt(PREFS_SOUND_EFFECTS, !state ? 1 : 0);
        _mixerGroup.audioMixer.SetFloat("SFXVolume", !state ? 0f : -80f);
    }
    
    public void SetMusic(bool state)
    {
        PlayerPrefs.SetInt(PREFS_MUSIC, !state ? 1 : 0);
        _mixerGroup.audioMixer.SetFloat("MusicVolume", !state ? -10f : -80f);
    }

    private void Start()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _fade.DOFade(0f, 4f).From(1f).SetDelay(1f);
    }

    public void Restart()
    {
        _isRestarting = true;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _fade.DOFade(1f, 1f).OnComplete(() => UnityEngine.SceneManagement.SceneManager.LoadScene(0));
    }

    public void OnTogglePause()
    {
        if (_isRestarting)
            return;
        IsPaused = !IsPaused;
        Time.timeScale = IsPaused ? 0f : 1f;
        Cursor.lockState = IsPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = IsPaused;
        _uiManager.TogglePauseMenu(IsPaused);
    }
    
    public void ExitGame()
    {
        Application.Quit();
    }

    public void OnGameOver()
    {
        _isGameOver = true;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        if (_score > PlayerPrefs.GetInt(PREFS_HIGHSCORE, 0))
            PlayerPrefs.SetInt(PREFS_HIGHSCORE, _score);
        
        _leaderboardHandler.UpdateHighscore();
        _uiManager.ShowGameOverMenu();
        PlayerMovement.IsMovementAllowed = false;
        CameraMovement.IsMovementAllowed = false;
    }
    
    public void OnGameWin()
    {
        if (_isGameOver)
            return;
        _isGameOver = true;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        _score += 50000;
        
        if (_score > PlayerPrefs.GetInt(PREFS_HIGHSCORE, 0))
            PlayerPrefs.SetInt(PREFS_HIGHSCORE, _score);
        
        _leaderboardHandler.UpdateHighscore();
        _uiManager.ShowGameWinMenu();
        PlayerMovement.IsMovementAllowed = false;
        CameraMovement.IsMovementAllowed = false;
    }

    private void Update()
    {
        if (_isGameOver)
            return;
        if (Input.GetKeyDown(KeyCode.P))
        {
            OnTogglePause();
        }
        
#if !UNITY_WEBGL
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnTogglePause();
        }
#endif
    }
}