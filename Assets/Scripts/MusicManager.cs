using System;
using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource[] _musics;

    private int _intensity;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(10f);
        _musics[0].Play();
        _musics[1].Play();
        _musics[2].Play();
        var time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime;
            _musics[0].volume = time;
            yield return null;
        }
    }

    public void SetIntensity(int intensity)
    {
        _intensity = intensity;
        foreach (var music in _musics)
        {
            music.volume = 0;
        }
        _musics[_intensity].volume = 1;
    }
    
    public void Mute()
    {
        foreach (var music in _musics)
        {
            music.volume = 0;
        }
    }
    
    public void Unmute()
    {
        _musics[_intensity].Play();
        _musics[_intensity].volume = 1;
    }
}
