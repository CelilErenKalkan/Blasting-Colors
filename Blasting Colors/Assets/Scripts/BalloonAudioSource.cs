using System.Collections.Generic;
using UnityEngine;
using static Actions;

public class BalloonAudioSource : MonoBehaviour
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
        BalloonDestroyed += OnBalloonPopped;
    }
    
    private void OnDisable()
    {
        BalloonDestroyed -= OnBalloonPopped;
    }
    
    private void OnBalloonPopped()
    {
        audioSource.clip = sounds[0];
        audioSource.Play();
    }
}
