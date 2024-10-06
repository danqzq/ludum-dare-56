using System;
using DG.Tweening;
using UnityEngine;

public class GroundDecal : MonoBehaviour
{
    [SerializeField] private float _fadeTime = 1f, _fadeDelay = 1f;

    private SpriteRenderer _spriteRenderer;
    
    private void Start()
    {
        transform.SetParent(null);
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.DOFade(0f, _fadeTime).SetDelay(_fadeDelay).OnComplete(() => Destroy(gameObject));
    }

    private void Update()
    {
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out var hit, 2f, LayerMask.GetMask("Ground")))
        {
            transform.position = hit.point + Vector3.up * 0.02f;
        }
        else
        {
            transform.position = Vector3.up * 100f;
        }
    }

    private void OnDestroy()
    {
        _spriteRenderer.DOKill();
    }
}