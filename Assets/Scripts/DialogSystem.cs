using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogSystem : MonoBehaviour
{
    public enum Character
    {
        MC,
        Villain
    }
    
    [System.Serializable]
    public struct Dialog
    {
        public DialogLine[] lines;
    }
    
    [System.Serializable]
    public struct DialogLine
    {
        public Character character;
        public string text;
        public AudioClip audio;
    }
    
    [SerializeField] private Dialog[] _dialogs;
    
    [SerializeField] private GameObject[] _characterBoxes;
    [SerializeField] private TextMeshProUGUI[] _dialogText;
    
    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public Coroutine Play(int index, System.Action onComplete = null)
    {
        return StartCoroutine(PlayDialog(_dialogs[index], onComplete));
    }
    
    private IEnumerator PlayDialog(Dialog dialog, System.Action onComplete)
    {
        foreach (var line in dialog.lines)
        {
            _characterBoxes[(int)line.character].SetActive(true);
            _dialogText[(int)line.character].text = line.text;
            _audioSource.clip = line.audio;
            _audioSource.Play();
            var time = 0f;
            var pressedSpace = false;
            while (time < line.audio.length)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    pressedSpace = true;
                    break;
                }
                time += Time.deltaTime;
                yield return null;
            }
            time = 0f;
            while (time < 0.5f && !Input.GetKeyDown(KeyCode.Space))
            {
                if (pressedSpace)
                    break;
                time += Time.deltaTime;
                yield return null;
            }
            yield return null;
            _characterBoxes[(int)line.character].SetActive(false);
        }
        onComplete?.Invoke();
    }
}