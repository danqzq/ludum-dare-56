using UnityEngine;

public class Cheese : MonoBehaviour, IPickup
{
    [SerializeField] private GameObject _full, _half;
    [SerializeField] private Transform _spotlight;
    
    private Rigidbody _rb;

    public static bool IsPickedUp { get; private set; }

    private bool _isDropped;
    
    private Vector3 _targetSpotlightPos;
    
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _isDropped = true;
        
        _full.SetActive(true);
        _half.SetActive(false);
    }

    private void Update()
    {
        if (PlayerStats.Instance.Cheese <= 50)
        {
            _full.SetActive(false);
            _half.SetActive(true);
        }
        
        var spotlightPos = _spotlight.position;
        spotlightPos.x = transform.position.x;
        spotlightPos.z = transform.position.z;
        _targetSpotlightPos = Vector3.MoveTowards(_spotlight.position, spotlightPos, 2f * Time.deltaTime);
        _spotlight.position = _targetSpotlightPos;
        
        if (!_isDropped) return;
        if (Physics.Raycast(transform.position, Vector3.down, 0.3f, LayerMask.GetMask("Ground")))
            IsPickedUp = false;
    }

    public Transform OnPickup(Transform player)
    {
        IsPickedUp = true;
        _isDropped = false;
        _rb.isKinematic = true;
        return transform;
    }

    public void OnDrop()
    {
        _isDropped = true;
        _rb.isKinematic = false;
    }

    public void OnThrow(float force, Vector3 direction)
    {
        OnDrop();
        _rb.AddForce(direction * force, ForceMode.Impulse);
    }
}