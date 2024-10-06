using DG.Tweening;
using UnityEngine;

public class Hoop : MonoBehaviour
{
    [SerializeField] private Transform _loop;
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private float _rotationSpeed = 0.2f;
    
    [SerializeField] private ParticleSystem _confetti;

    [SerializeField] private GameObject _shotgunPickup;
    [SerializeField] private GameObject _chargers, _rubberRoomBoss;
    
    [SerializeField] private GameObject _ammoPickupPrefab;
    [SerializeField] private DialogSystem _dialogSystem;

    [SerializeField] private Light[] _lights;
    
    private Vector3 _positionOnEnter;
    private Rat _rat;
    
    private int _hoopsPassed;

    private float _timer;
    
    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (_hoopsPassed >= 15)
            return;
        
        if (_hoopsPassed < 3) 
            return;
        _timer += Time.deltaTime;
        var position = transform.position;
        position.z = Mathf.Sin(_timer * _moveSpeed) * 2f;
        position.y = Mathf.Lerp(position.y, 3.75f, Time.deltaTime * 2f);
        transform.position = position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Rat rat))
        {
            if (_loop.position.y > rat.transform.position.y + 0.125f)
                return;
            _rat = rat;
            _positionOnEnter = rat.transform.position;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Rat rat))
        {
            if (_rat != rat) 
                return;
            var positionOnExit = rat.transform.position;
            if (_positionOnEnter.y > positionOnExit.y + 0.125f && rat.YVelocity < 0f && positionOnExit.y < _loop.position.y)
            {
                _confetti.Play();
                _hoopsPassed++;
                GameManager.Score += 25;
                _audioSource.Play();
                switch (_hoopsPassed)
                {
                    case 5:
                        _shotgunPickup.SetActive(true);
                        foreach (var l in _lights)
                        {
                            l.DOIntensity(0f, 0.5f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutSine);
                        }
                        this.DoAfter(60f, () =>
                        {
                            this.DoAfter(1f, () => _dialogSystem.Play(4));
                            _chargers.SetActive(true);
                            _rubberRoomBoss.SetActive(true);
                        });
                        break;
                    case 15:
                        transform.DOLocalMoveZ(0f, 0.75f).OnComplete(() =>
                            transform.parent.DORotate(new Vector3(0f, 359.99f, 0f), 1f / _rotationSpeed)
                                .SetRelative().SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear));
                        break;
                }

                if (Shotgun.isPickedUp && _hoopsPassed > 5)
                {
                    Instantiate(_ammoPickupPrefab, transform.position + transform.forward + Vector3.up * 5f, Quaternion.identity);
                }
            }
            
            _rat = null;
        }
    }
}
