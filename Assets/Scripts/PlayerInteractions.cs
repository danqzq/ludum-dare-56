using System;
using DG.Tweening;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
    [SerializeField] private Transform _rayProjector;
    
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private Transform _pickupParent;
    
    public bool IsInteractionAllowed { get; set; } = true;
    
    private IPickup _currentPickup;
    private Transform _pickupTransform;

    private bool _isThrowing;
    private float _throwForce;
    private const float THROW_FORCE_MAX = 10f, THROW_FORCE_INCREASE_SPEED = 10f;
    private const float MAX_PULL_BACK_DISTANCE = 0.5f;

    private float _ratHoldingTime;
    
    private PlayerStats _playerStats;
    
    private void Start()
    {
        _currentPickup = null;
        _pickupTransform = null;
        _throwForce = 0f;
        _isThrowing = false;
        
        _playerStats = PlayerStats.Instance;
    }
    
    private void DecreaseSanity()
    {
        _ratHoldingTime += Time.deltaTime;
        if (_ratHoldingTime >= 1f)
        {
            _ratHoldingTime = 0f;
            _playerStats.Sanity--;
        }
    }

    private void HandleInteractions()
    {
        if (_isThrowing)
        {
            DecreaseSanity();
            _throwForce = Mathf.Clamp(_throwForce + THROW_FORCE_INCREASE_SPEED * Time.deltaTime, 0f, THROW_FORCE_MAX);
            _pickupTransform.localPosition = Vector3.back * Mathf.Lerp(0, MAX_PULL_BACK_DISTANCE, _throwForce / THROW_FORCE_MAX);
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                _isThrowing = false;
                _currentPickup.OnThrow(_throwForce, _rayProjector.forward);
                _pickupTransform.SetParent(null);
                _currentPickup = null;
            }
            return;
        }
        
        if (_currentPickup != null)
        {
            _uiManager.ToggleThrowHint(true);
            DecreaseSanity();
            
            if (Input.GetKeyDown(KeyCode.E))
            {
                _pickupTransform.DOKill();
                _pickupTransform.SetParent(null);
                _currentPickup.OnDrop();
                _currentPickup = null;
                _uiManager.ToggleThrowHint(false);
            }
            
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                _isThrowing = true;
                _throwForce = 0f;
                _uiManager.ToggleThrowHint(false);
            }
            
            _uiManager.TogglePickupHint(false);
            return;
        }
        
        _uiManager.ToggleThrowHint(false);
        
        if (Physics.Raycast(_rayProjector.position, _rayProjector.forward, out var hit, 3f))
        {
            if (hit.collider.TryGetComponent(out IPickup pickup))
            {
                _uiManager.TogglePickupHint(true);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    _currentPickup = pickup;
                    (_pickupTransform = pickup.OnPickup(transform)).SetParent(_pickupParent);
                    _pickupTransform.DOLocalMove(Vector3.zero, 0.25f);
                }
            }
            else if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                _uiManager.TogglePickupHint(true);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactable.OnInteract();
                }
            }
            else
            {
                _uiManager.TogglePickupHint(false);
            }
        }
        else
        {
            _uiManager.TogglePickupHint(false);
        }
    }
    
    private void Update()
    {
        if (!IsInteractionAllowed) 
            return;

        HandleInteractions();
    }

    private void OnDestroy()
    {
        if (_pickupTransform != null)
        {
            _pickupTransform.DOKill();
        }
        
        _currentPickup = null;
        _pickupTransform = null;
    }
}