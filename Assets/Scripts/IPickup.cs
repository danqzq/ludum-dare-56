using UnityEngine;

public interface IPickup
{
    Transform OnPickup(Transform player);
    void OnDrop();
    
    void OnThrow(float force, Vector3 direction);
}