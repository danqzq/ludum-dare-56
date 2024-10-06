using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private Slider _sensitivitySlider;
    
    [SerializeField] private float _mouseSensitivity = 1.0f;
    
    public static bool IsMovementAllowed { get; set; } = true;
    public float ZValue { get; set; }
        
    private Camera _cam;
        
    private float _horizontalSpeed = 1.5f;
    private float _verticalSpeed = 1.5f;

    private float _xRotation;
    private float _yRotation;

    private static Transform _t;
    private static Vector3 _ogLocalPos;
    
    public static void DoShake(float duration, float strength, System.Action<MonoBehaviour> callback = null)
    {
        _t.DOShakePosition(duration, strength).OnComplete(() => _t.localPosition = _ogLocalPos);
        callback?.Invoke(_t.GetComponent<MonoBehaviour>());
    }

    private void Start()
    {
        IsMovementAllowed = true;
        _t = transform;
        _ogLocalPos = _t.localPosition;
        _cam = GetComponent<Camera>();
        
        _sensitivitySlider.value = PlayerPrefs.GetFloat(Globals.PREFS_MOUSE_SENSITIVITY, 1.0f);
    }
    
    public void SetSensitivity(float value)
    {
        _mouseSensitivity = value;
        PlayerPrefs.SetFloat(Globals.PREFS_MOUSE_SENSITIVITY, value);
    }
    
    private void Update()
    {
        if (_gameManager.IsPaused || !IsMovementAllowed) return;

        var mouseX = Input.GetAxis("Mouse X") * _horizontalSpeed * _mouseSensitivity;
        var mouseY = Input.GetAxis("Mouse Y") * _verticalSpeed * _mouseSensitivity;

        _yRotation += mouseX;
        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90, 90);

        _cam.transform.eulerAngles = new Vector3(_xRotation, _yRotation, ZValue);
        transform.parent.rotation = Quaternion.Euler(0.0f, _yRotation, ZValue);
    }
}