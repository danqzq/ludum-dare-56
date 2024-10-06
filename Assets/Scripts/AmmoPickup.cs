using UnityEngine;

public class AmmoPickup : MonoBehaviour, IInteractable
{
    [SerializeField] private int _amount = 5;
    
    public void OnInteract()
    {
        Shotgun.AddAmmo(_amount);
        Destroy(gameObject);
    }
}