using DG.Tweening;
using UnityEngine;

public class KingRat : MonoBehaviour
{
    [SerializeField] private float _speed = 2f;
    [SerializeField] private GameObject _deathParticlesPrefab;
    
    private Vector3 _targetPos;

    private RatManager _ratManager;

    private Animator _anim;
    private Rigidbody _rb;
    private AudioSource _audioSource;
    
    private bool _isDead;
    
    private float _cheeseEatTime;
    
    public void Init(RatManager ratManager)
    {
        _ratManager = ratManager;
        _anim = GetComponentInChildren<Animator>();
        _rb = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();
        _rb.constraints = RigidbodyConstraints.FreezeAll;
    }
    
    private void Update()
    {
        if (_isDead) return;
        _targetPos = Cheese.IsPickedUp ? _ratManager.GetPlayerPosition() : _ratManager.GetCheesePosition();
        _targetPos.y = 2.5f;

        if (Mathf.Approximately(_targetPos.x, transform.position.x) &&
            Mathf.Approximately(_targetPos.z, transform.position.z))
        {
            _targetPos.y = 0.5f;
            if (Mathf.Approximately(_targetPos.y, transform.position.y))
            {
                _cheeseEatTime += Time.deltaTime;
                if (_cheeseEatTime >= 1f)
                {
                    if (Cheese.IsPickedUp)
                    {
                        PlayerStats.Instance.Health -= 4;
                        _ratManager.SpawnCheeseParticles(transform.position);
                    }
                    else PlayerStats.Instance.Cheese -= 4;
                    _cheeseEatTime = 0f;
                }
            }
        }
        
        transform.position = Vector3.MoveTowards(transform.position, _targetPos, _speed * Time.deltaTime);
    }

    public void Die()
    {
        _isDead = true;
        _anim.enabled = false;
        _rb.constraints = RigidbodyConstraints.None;
        this.DoAfter(1f, () => transform.DOScaleY(0f, 1f).OnComplete(() => Destroy(gameObject)));
        GameManager.Score += 50;
        _audioSource.Stop();
        if (PlayerPrefs.GetInt(Globals.PREFS_BLOOD_EFFECTS, 1) == 1)
        {
            var particles = Instantiate(_deathParticlesPrefab, transform.position, Quaternion.identity);
            Destroy(particles, 4f);
        }
    }
    
    private void OnParticleCollision(GameObject other)
    {
        if (_isDead)
            return;
        Die();
    }
}