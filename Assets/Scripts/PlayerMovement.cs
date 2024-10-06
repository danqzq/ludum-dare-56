using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _speed = 12f;

    [SerializeField] private Transform _shotgun, _shotgunPoint;
    
    private CharacterController _cc;
    private Vector3 _playerVelocity;

    public static bool IsMovementAllowed { get; set; }

    private Vector3 MoveDirection { get; set; }
    
    private void Start()
    {
        IsMovementAllowed = true;
        _cc = GetComponent<CharacterController>();
    }

    private void MovementInput()
    {
        var isGrounded = _cc.isGrounded;

        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");
            
        var ts = transform;
        MoveDirection = ts.right * horizontal + ts.forward * vertical;

        _playerVelocity.x = Mathf.Lerp(_playerVelocity.x, 0, 0.25f);
        _playerVelocity.z = Mathf.Lerp(_playerVelocity.z, 0, 0.25f);
        
        if (isGrounded && _playerVelocity.y < 0f) _playerVelocity.y = -0.1f;
        _cc.Move((_playerVelocity + MoveDirection) * (_speed * Time.deltaTime));
    }
        
    private void Update()
    {
        if (!IsMovementAllowed) return;

        MovementInput();
    }

    private void LateUpdate()
    {
        _shotgun.position = Vector3.Lerp(_shotgun.position, _shotgunPoint.position, Time.deltaTime * 100f);
        _shotgun.rotation = Quaternion.Lerp(_shotgun.rotation, _shotgunPoint.rotation, Time.deltaTime * 50f);
    }
}
