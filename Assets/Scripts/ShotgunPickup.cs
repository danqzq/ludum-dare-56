using UnityEngine;

public class ShotgunPickup : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject _shotgun;
    [SerializeField] private DialogSystem _dialogSystem;
    [SerializeField] private GameObject _spotlight;

    private void OnEnable()
    {
        _spotlight.SetActive(true);
    }

    public void OnInteract()
    {
        _dialogSystem.Play(3);
        _shotgun.SetActive(true);
        _shotgun.transform.SetParent(null);
        Destroy(gameObject);
    }
}