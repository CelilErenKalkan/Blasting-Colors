using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using static Actions;

public class Goals : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    { 
        SetGoals();
        SetGoalTexts();
    }

    private void OnEnable()
    {
        GoalAmountChanged += SetGoalTexts;
    }
    
    private void OnDisable()
    {
        GoalAmountChanged -= SetGoalTexts;
    }

    private void SetGoals()
    {
        for (var i = 0; i < 2; i++)
        {
            var random = Random.Range(0, GameManager.Instance.cubes.Length);
            var goalName = "Goal" + i;
            var goalCubeNo = PlayerPrefs.GetInt(goalName, random);
            
            GameManager.Instance.goalList.Add(transform.GetChild(i).gameObject);
            transform.GetChild(i).tag = GameManager.Instance.cubes[goalCubeNo].tag;
            
            if (transform.GetChild(i).TryGetComponent(out Image image) &&
                GameManager.Instance.cubes[goalCubeNo].TryGetComponent(out SpriteRenderer cubeImage))
                image.sprite = cubeImage.sprite;
            PlayerPrefs.SetInt(goalName, goalCubeNo);
        }
    }

    private void SetGoalTexts()
    {
        var count = GameManager.Instance.goalAmounts.Count;
        for (var i = 0; i < count; i++)
        {
            if (transform.GetChild(i).GetChild(0).TryGetComponent(out TMP_Text text))
                text.text = GameManager.Instance.goalAmounts[i].ToString();
        }
    }
}
