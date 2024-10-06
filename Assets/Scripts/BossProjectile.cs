using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    [SerializeField] private GameObject _explosionPrefab;

    private float _invincibilityTime;
    
    private void Start()
    {
        _invincibilityTime = 0.25f;
    }
    
    private void Update()
    {
        _invincibilityTime -= Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_invincibilityTime > 0f)
            return;
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            var obj = _explosionPrefab;
            var p = transform.position;
            if (Physics.CheckSphere(p, 1.5f, LayerMask.GetMask("Player")))
            {
                PlayerStats.Instance.Health -= 5;
            }
            CameraMovement.DoShake(0.5f, 1f, mon =>
            {
                var origin = p;
                for (int i = 0; i < 5; i++)
                {
                    mon.DoAfter(0.1f * i, () =>
                    {
                        var pos = origin + Random.insideUnitSphere;
                        var explosion = Instantiate(obj, pos, Quaternion.identity);
                        Destroy(explosion, 2f);
                    });
                }
            });
            Destroy(gameObject);
        }
    }
}