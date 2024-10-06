using System.Collections;
using DG.Tweening;
using UnityEngine;

public class RubberRoomBoss : MonoBehaviour
{
    [SerializeField] private Transform _leftEye, _rightEye;

    [SerializeField] private Transform _player;

    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private DialogSystem _dialogSystem;
    [SerializeField] private MusicManager _musicManager;

    [SerializeField] private GameObject _particles;
    [SerializeField] private GameObject _spikes;

    [SerializeField] private UIManager _uiManager;
    
    [SerializeField] private int _maxHealth = 50;
    
    private int _health;
    private int _counter;
    
    private bool _enterThreeQuarterHealth, _enterHalfHealth;

    public static bool isActive;
    
    private IEnumerator Start()
    {
        _health = _maxHealth;
        _leftEye.gameObject.SetActive(false);
        _rightEye.gameObject.SetActive(false);
        yield return new WaitForSeconds(10f);
        _leftEye.gameObject.SetActive(true);
        _rightEye.gameObject.SetActive(true);
        var originalScale = _leftEye.localScale;
        _leftEye.localScale = Vector3.zero;
        _rightEye.localScale = Vector3.zero;
        _leftEye.DOScale(originalScale, 1f);
        yield return _rightEye.DOScale(originalScale, 1f).WaitForCompletion();
        _musicManager.SetIntensity(2);
        isActive = true;
        _uiManager.ToggleBossHint(true);
        this.DoAfter(5f, () => _uiManager.ToggleBossHint(false));
        yield return BehaviorCoroutine();
    }

    private void ShootProjectile(Vector3 pos)
    {
        var projectile = Instantiate(_projectilePrefab, pos, Quaternion.identity);
        projectile.transform.LookAt(_player);
        projectile.GetComponent<Rigidbody>().AddForce(projectile.transform.forward * 10f, ForceMode.Impulse);
    }
    private IEnumerator BehaviorCoroutine()
    {
        _counter++;
        _rightEye.DOLookAt(_player.position, 1f);
        yield return _leftEye.DOLookAt(_player.position, 1f).WaitForCompletion();
        if (_counter % 2 == 0)
            ShootProjectile(_rightEye.position);
        yield return new WaitForSeconds(1f);
        _rightEye.DOLookAt(_player.position, 1f);
        yield return _leftEye.DOLookAt(_player.position, 1f).WaitForCompletion();
        _counter++;
        if (_counter % 2 == 0)
            ShootProjectile(_leftEye.position);
        yield return new WaitForSeconds(1f);
        StartCoroutine(BehaviorCoroutine());
    }

    public void TakeDamage(int damage)
    {
        if (_health <= 0)
            return;
        _health -= damage;
        if (_health <= _maxHealth * 0.75f && !_enterThreeQuarterHealth)
        {
            _enterThreeQuarterHealth = true;
            _dialogSystem.Play(5);
        }
        else if (_health <= _maxHealth * 0.5f && !_enterHalfHealth)
        {
            _enterHalfHealth = true;
            _dialogSystem.Play(6);
            if (PlayerPrefs.GetInt(Globals.PREFS_BLOOD_EFFECTS, 1) == 1)
            {
                _particles.SetActive(true);
                _spikes.SetActive(true);
            }
        }
        else if (_health <= 0)
        {
            _dialogSystem.Play(7);
            _leftEye.DOScale(Vector3.zero, 1f);
            _rightEye.DOScale(Vector3.zero, 1f).OnComplete(() => Destroy(gameObject));
            _gameManager.OnGameWin();
            isActive = false;
        }
    }
}
