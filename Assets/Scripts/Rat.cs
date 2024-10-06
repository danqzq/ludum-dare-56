using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Rat : MonoBehaviour, IPickup
{
    [SerializeField] private Transform _leftEye, _rightEye;
    [SerializeField] private MeshRenderer[] _bodyRenderers;
    [SerializeField] private Material _burnedMaterial;
    [SerializeField] private GameObject _deathParticlesPrefab;
    
    private Rigidbody _rb;

    private RatManager _ratManager;

    private bool _isMovementAllowed;
    private bool _isDropped;
    private bool _isDead;
    
    private bool IsGrounded => Physics.Raycast(transform.position, Vector3.down, 0.2f, LayerMask.GetMask("Ground"));
    
    public float Magnitude => _rb.linearVelocity.magnitude;
    
    public float YVelocity => _rb.linearVelocity.y;

    private Coroutine _eatCheeseCoroutine;
    
    protected RatManager RatManager => _ratManager;
    
    private float _soundCooldown;
    private bool _wasEating;
    
    public void Init(RatManager ratManager)
    {
        _ratManager = ratManager;
        _rb = GetComponent<Rigidbody>();
        _isDropped = true;
        _isMovementAllowed = true;
        StartCoroutine(BehaviorCoroutine());
        
        var dir = (_ratManager.GetCheesePosition() - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(dir);
    }

    private void Update()
    {
        _soundCooldown = Mathf.Max(0f, _soundCooldown - Time.deltaTime);
    }

    private IEnumerator EatCheese()
    {
        if (_isDead)
            yield break;
        _wasEating = true;
        if (Cheese.IsPickedUp)
            PlayerStats.Instance.Health--;
        else
        {
            PlayerStats.Instance.Cheese--;
            _ratManager.SpawnCheeseParticles(transform.position);
        }
        var time = 0f;
        var cheesePos = GetCheesePosition();
        yield return new WaitUntil(() =>
        {
            cheesePos = GetCheesePosition();
            time += Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation,
                Quaternion.LookRotation(cheesePos - transform.position), 180f * Time.deltaTime);
            return time >= 1f;
        });
        cheesePos = GetCheesePosition();
        if (Vector3.Distance(transform.position, cheesePos) > 0.5f)
        {
            _eatCheeseCoroutine = null;
            _isMovementAllowed = true;
            yield break;
        }
        _eatCheeseCoroutine = StartCoroutine(EatCheese());
    }
    
    private Vector3 GetCheesePosition() => Cheese.IsPickedUp ? _ratManager.GetPlayerPosition() : _ratManager.GetCheesePosition();

    private IEnumerator BehaviorCoroutine()
    {
        if (_isDead)
            yield break;
        
        _rb.isKinematic = true;
        
        _leftEye.DOKill();
        _leftEye.DOLookAt(GetCheesePosition(), 0.25f);
        _rightEye.DOKill();
        _rightEye.DOLookAt(GetCheesePosition(), 0.25f);
        
        while (_isMovementAllowed && _isDropped)
        {
            while (_rb.linearVelocity.magnitude > 0.1f && !IsGrounded)
            {
                yield return null;
            }
            
            var cheesePos = GetCheesePosition();
            transform.position = Vector3.MoveTowards(transform.position, cheesePos, 1f * Time.deltaTime);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(cheesePos - transform.position), 180f * Time.deltaTime);
            if (Vector3.Distance(transform.position, cheesePos) < 0.5f)
            {
                _isMovementAllowed = false;
                _eatCheeseCoroutine = StartCoroutine(EatCheese());
                break;
            }
            yield return null;
        }
        
        if (_eatCheeseCoroutine != null)
            yield return null;
        
        if (_isDead)
            yield break;
        
        while (!_isDropped)
        {
            yield return null;
        }

        yield return new WaitForSeconds(2f);
        
        if (_isDead)
            yield break;
        
        if (!_isDropped)
        {
            StartCoroutine(BehaviorCoroutine());
            yield break;
        }
        
        while (_rb.linearVelocity.Horizontal().magnitude > 0.2f || !IsGrounded)
        {
            if (_wasEating)
            {
                _wasEating = false;
                break;
            }
            yield return null;
        }
        
        _rb.isKinematic = true;
        var dir = (_ratManager.GetCheesePosition() - transform.position).normalized;
        while (Vector3.Angle(transform.forward, dir) > 1f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(dir), 180f * Time.deltaTime);
            yield return null;
        }
        
        _rb.isKinematic = false;
        while (_rb.linearVelocity.Horizontal().magnitude > 0.2f || !IsGrounded)
        {
            if (_wasEating)
            {
                _wasEating = false;
                break;
            }
            yield return null;
        }
        
        _rb.isKinematic = true;
        
        StartCoroutine(BehaviorCoroutine());
    }

    public Transform OnPickup(Transform player)
    {
        _isDropped = false;
        _isMovementAllowed = false;
        _rb.isKinematic = true;
        _leftEye.DOKill();
        _leftEye.DOLookAt(player.position + Vector3.up, 0.25f);
        _rightEye.DOKill();
        _rightEye.DOLookAt(player.position + Vector3.up, 0.25f);
        _ratManager.OnRatPickup();
        return transform;
    }

    public void OnDrop()
    {
        _isDropped = true;
        _isMovementAllowed = true;
        _rb.isKinematic = false;
    }

    public void OnThrow(float force, Vector3 direction)
    {
        OnDrop();
        _rb.AddForce(direction * force, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (_isDead) 
            return;
        if (Magnitude > 2f)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Ground") || 
                other.collider.TryGetComponent(out KingRat _) ||
                other.collider.TryGetComponent(out BossEye _))
            {
                OnContactWithOther();
            }
            return;
        }
        if (other.collider.TryGetComponent(out Rat rat))
        {
            if (rat.Magnitude > 1f)
            {
                OnContactWithRat(rat);
            }
        }
    }

    public void Die()
    {
        GameManager.Score += 10;
        _isDead = true;
        _isMovementAllowed = false;
        _rb.isKinematic = false;
        foreach (var r in _bodyRenderers)
        {
            r.material = _burnedMaterial;
        }
        if (PlayerPrefs.GetInt(Globals.PREFS_BLOOD_EFFECTS, 1) == 1)
            Destroy(Instantiate(_deathParticlesPrefab, transform.position, Quaternion.identity), 4f);
        var time = 0f;
        this.DoAfterUntil(() => IsGrounded || (time += Time.deltaTime) > 1f, () => this.DoAfter(1f, () =>
        {
            _rb.freezeRotation = true;
            transform.DOKill();
            transform.DOMoveY(0f, 0.95f);
            transform.DOScale(0f, 1f).OnComplete(() =>
            {
                transform.DOKill();
                Destroy(gameObject);
            });
        }));
    }
    
    protected virtual void OnContactWithRat(Rat rat)
    {
        if (_isDead)
            return;
        _rb.isKinematic = false;
        _isMovementAllowed = false;
        _rb.AddForce((transform.position - rat.transform.position).normalized * rat.Magnitude, ForceMode.Impulse);
        GameManager.Score += 1;
        this.DoAfter(2f, () =>
        {
            if (_isDead)
                return;
            _isMovementAllowed = true;
            _rb.isKinematic = true;
        });
        if (_soundCooldown > 0f)
            return;
        _ratManager.OnRatFall();
        _soundCooldown = 1.5f;
    }
    
    protected virtual void OnContactWithOther()
    {
        if (_soundCooldown > 0f)
            return;
        _ratManager.OnRatFall();
        _soundCooldown = 1.5f;
    }

    private void OnParticleCollision(GameObject other)
    {
        if (_isDead)
            return;
        Die();
    }

    private void OnDestroy()
    {
        _leftEye.DOKill();
        _rightEye.DOKill();
    }
}
