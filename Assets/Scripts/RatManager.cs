using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class RatManager : MonoBehaviour
{
    [SerializeField] private GameObject _ratPrefab, _explosiveRatPrefab, _kingRatPrefab;
    [SerializeField] private Transform _player, _cheese;
    [SerializeField] private GameObject _cheeseParticlesPrefab;

    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private Transform[] _kingRatSpawnPoints;

    [SerializeField] private GameObject _basketRat;
    [SerializeField] private MusicManager _musicManager;
    
    [SerializeField] private AudioSource[] _ratFallSounds, _ratPickupSounds, _ratSpawnSounds;
    [SerializeField] private DialogSystem _dialogSystem;

    [SerializeField] private AudioSource _introSound;
    
    [SerializeField] private TMP_Text[] _tutorialTexts;
    
    private KingRat _kingRat;

    private IEnumerator Start()
    {
        if (PlayerPrefs.GetInt("Tutorial", 0) == 0)
        {
            PlayerPrefs.SetInt("Tutorial", 1);
            _introSound.Play();
            foreach (var text in _tutorialTexts)
            {
                text.gameObject.SetActive(true);
                text.DOFade(0f, 5f).SetDelay(20f);
            }
            yield return new WaitForSeconds(10f);
        }
        
        _dialogSystem.Play(0, () => StartCoroutine(BehaviorCoroutine()));
        yield return null;
    }
    
    public void OnRatFall()
    {
        _ratFallSounds[Random.Range(0, _ratFallSounds.Length)].Play();
    }
    
    public void OnRatPickup()
    {
        _ratPickupSounds[Random.Range(0, _ratPickupSounds.Length)].Play();
    }
    
    private IEnumerator BehaviorCoroutine()
    {
        for (int i = 0; i < 4; i++)
        {
            SpawnRat(Random.Range(0, _spawnPoints.Length), false);
            yield return new WaitForSeconds(2f);
        }
        
        yield return new WaitForSeconds(5f);
        
        for (int i = 0; i < 4; i++)
        {
            SpawnRat(Random.Range(0, _spawnPoints.Length), false);
            yield return new WaitForSeconds(1f);
        }
        
        SpawnRat(Random.Range(0, _spawnPoints.Length), true);
        
        _dialogSystem.Play(1);
        
        yield return new WaitForSeconds(5f);
        
        for (int i = 0; i < 8; i++)
        {
            SpawnRat(Random.Range(0, _spawnPoints.Length), i % 3 == 0);
            yield return new WaitForSeconds(2f);
        }
        
        yield return new WaitForSeconds(5f);
        
        if (_kingRat == null)
        {
            SpawnKingRat();
        }
        
        for (int i = 0; i < 8; i++)
        {
            SpawnRat(Random.Range(0, _spawnPoints.Length), i % 3 == 0);
            yield return new WaitForSeconds(2f);
        }
        
        if (_kingRat == null)
        {
            SpawnKingRat();
        }
        
        for (int i = 0; i < 8; i++)
        {
            SpawnRat(Random.Range(0, _spawnPoints.Length), i % 3 == 0);
            yield return new WaitForSeconds(1f);
        }
        
        _basketRat.SetActive(true);
        yield return _dialogSystem.Play(2);
        _musicManager.SetIntensity(1);
        
        for (int i = 0; i < 8; i++)
        {
            SpawnRat(Random.Range(0, _spawnPoints.Length), i % 3 == 0);
            yield return new WaitForSeconds(3f);
        }
        
        if (_kingRat == null)
        {
            SpawnKingRat();
        }
        
        for (int i = 0; i < 8; i++)
        {
            SpawnRat(Random.Range(0, _spawnPoints.Length), i % 3 == 0);
            yield return new WaitForSeconds(3f);
        }

        while (!Shotgun.isPickedUp)
        {
            for (int i = 0; i < 4; i++)
            {
                SpawnRat(Random.Range(0, _spawnPoints.Length), i % 3 == 0);
                yield return new WaitForSeconds(3f);
            }
            
            if (_kingRat == null)
            {
                SpawnKingRat();
            }
            
            yield return new WaitForSeconds(3f);
        }
        
        for (int i = 0; i < 8; i++)
        {
            SpawnRat(Random.Range(0, _spawnPoints.Length), i % 4 == 0);
            yield return new WaitForSeconds(2f);
        }
        
        yield return new WaitUntil(() => RubberRoomBoss.isActive);

        while (RubberRoomBoss.isActive)
        {
            for (int i = 0; i < 4; i++)
            {
                SpawnRat(Random.Range(0, _spawnPoints.Length), i % 2 == 0);
                yield return new WaitForSeconds(5f);
            }
        }
    }

    private void SpawnKingRat()
    {
        _kingRat = Instantiate(_kingRatPrefab, _kingRatSpawnPoints[Random.Range(0, _kingRatSpawnPoints.Length)].position, Quaternion.identity).GetComponent<KingRat>();
        _kingRat.Init(this);
        _ratSpawnSounds[Random.Range(0, _ratSpawnSounds.Length)].Play();
    }
    
    public void SpawnCheeseParticles(Vector3 position)
    {
        position.y = 0;
        Destroy(Instantiate(_cheeseParticlesPrefab, position, Quaternion.identity), 4);
    }
    
    private void SpawnRat(int index, bool isExplosive)
    {
        var rat = Instantiate(isExplosive ? _explosiveRatPrefab : _ratPrefab, _spawnPoints[index].position, Quaternion.identity);
        rat.GetComponent<Rat>().Init(this);
    }

    public Vector3 GetCheesePosition()
    {
        return _cheese.position;
    }
    
    public Vector3 GetPlayerPosition()
    {
        var pos = _player.position;
        pos.y = 0.1f;
        return pos;
    }
}