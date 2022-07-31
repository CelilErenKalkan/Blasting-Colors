using System.Collections.Generic;
using UnityEngine;
using static Actions;

public class SoundManager : MonoBehaviour
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
        CubeDestroyed += OnCubeDestroyed;
        DuckDestroyed += OnDuckDestroyed;
        GoalAmountChanged += OnGoalAmountChanged;
    }
    
    private void OnDisable()
    {
        CubeDestroyed -= OnCubeDestroyed;
        DuckDestroyed -= OnDuckDestroyed;
        GoalAmountChanged -= OnGoalAmountChanged;
    }
    
    private void OnGoalAmountChanged()
    {
        audioSource.clip = sounds[0];
        audioSource.Play();
    }

    private void OnCubeDestroyed()
    {
        audioSource.clip = sounds[1];
        audioSource.Play();
    }
    
    private void OnDuckDestroyed()
    {
        audioSource.clip = sounds[2];
        audioSource.Play();
    }
}
