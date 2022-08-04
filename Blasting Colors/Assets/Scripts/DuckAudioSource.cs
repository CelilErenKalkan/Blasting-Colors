using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Actions;

public class DuckAudioSource : MonoBehaviour
{
    private AudioSource audioSource;
    public List<AudioClip> sounds = new List<AudioClip>();
    
    // Start is called before the first frame update
    private void Start()
    {
        //Fetch the AudioSource from the GameObject
        if (TryGetComponent(out AudioSource audio)) audioSource = audio;
    }

    private void OnEnable()
    {
        DuckDestroyed += OnDuckDestroyed;
    }
    
    private void OnDisable()
    {
        DuckDestroyed -= OnDuckDestroyed;
    }
    
    private void OnDuckDestroyed()
    {
        audioSource.clip = sounds[0];
        audioSource.Play();
    }
}
