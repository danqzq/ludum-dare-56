using UnityEngine;

public class BossEye : MonoBehaviour
{
    [SerializeField] private RubberRoomBoss _boss;

    private void OnParticleCollision(GameObject other)
    {
        TakeDamage(1);
    }

    public void TakeDamage(int damage)
    {
        _boss.TakeDamage(damage);
    }
}