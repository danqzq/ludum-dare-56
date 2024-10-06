using UnityEngine;

public class ExplosiveRat : Rat
{
    [SerializeField] private int _damage = 10;
    [SerializeField] private GameObject _explosionPrefab;
    
    protected override void OnContactWithOther()
    {
        base.OnContactWithOther();
        var colliders = new Collider[10];
        if (Physics.OverlapSphereNonAlloc(transform.position, 2f, colliders, LayerMask.GetMask("Rat")) <= 0) 
            return;
        foreach (var t in colliders)
        {
            if (t == null || t.transform == transform)
                continue;
            var rat = t.GetComponent<Rat>();
            if (rat == null)
            {
                var kingRat = t.GetComponent<KingRat>();
                if (kingRat != null)
                {
                    kingRat.Die();
                    continue;
                }
                var bossEye = t.GetComponent<BossEye>();
                if (bossEye != null)
                {
                    bossEye.TakeDamage(5);
                }
                continue;
            }
            rat.Die();
        }
        
        if (Physics.CheckSphere(transform.position, 2f, LayerMask.GetMask("Player")))
        {
            PlayerStats.Instance.Health -= _damage;
        }
        
        if (Physics.CheckSphere(transform.position, 2f, LayerMask.GetMask("Cheese")))
        {
            PlayerStats.Instance.Cheese -= _damage;
            RatManager.SpawnCheeseParticles(transform.position);
        }
        
        CameraMovement.DoShake(0.5f, 0.75f);
        Destroy(Instantiate(_explosionPrefab, transform.position, Quaternion.identity), 2f);
        Destroy(gameObject);
    }
    
    protected override void OnContactWithRat(Rat rat)
    {
        base.OnContactWithRat(rat);
    }
}