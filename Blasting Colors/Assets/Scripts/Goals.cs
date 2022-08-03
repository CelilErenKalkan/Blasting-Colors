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
    private int random;

    private GameManager manager;
    
    private void OnEnable()
    {
        GoalAmountChanged += SetGoalTexts;
        DuckDestroyed += CheckGoalForDuck;
        BalloonDestroyed += CheckGoalForBalloon;
        LevelStart += SetGoals;
    }
    
    private void OnDisable()
    {
        GoalAmountChanged -= SetGoalTexts;
        DuckDestroyed -= CheckGoalForDuck;
        BalloonDestroyed -= CheckGoalForBalloon;
        LevelStart -= SetGoals;
    }

    private void SetGoals()
    {
        manager = GameManager.Instance;
        
        for (var i = 0; i < 2; i++)
        {
            manager.goalList.Add(transform.GetChild(i).gameObject);
            
            random = Random.Range(0, manager.cubes.Length - 2);
            var goalName = "Goal" + i;
            var goalCubeNo = PlayerPrefs.GetInt(goalName, random);

            if (i > 0 && transform.GetChild(i - 1).CompareTag(manager.cubes[goalCubeNo].tag))
            {
                random = GetExcludedRandomValue();
            }
            
            transform.GetChild(i).tag = manager.cubes[goalCubeNo].tag;

            if (transform.GetChild(i).TryGetComponent(out Image image) &&
                manager.cubes[goalCubeNo].TryGetComponent(out SpriteRenderer cubeImage))
            {
                image.sprite = cubeImage.sprite;
                image.enabled = true;
            }
            PlayerPrefs.SetInt(goalName, goalCubeNo);
        }
        
        SetGoalTexts();
    }

    private void SetGoalTexts()
    {
        var count = manager.goalAmounts.Count;
        for (var i = 0; i < count; i++)
        {
            if (transform.GetChild(i).GetChild(0).TryGetComponent(out TMP_Text text))
            {
                if (manager.goalAmounts[i] > 0)
                    text.text = manager.goalAmounts[i].ToString();
                else
                {
                    transform.GetChild(i).GetChild(1).gameObject.SetActive(true);
                    text.text = " ";
                }
            }
        }
    }

    private int GetExcludedRandomValue()
    {
        var nextRandom = Random.Range(0, manager.cubes.Length - 2);
        if (nextRandom == random)
            GetExcludedRandomValue();
        
        return random;
    }

    private void CheckGoalForDuck()
    {
        for (var i = 0; i < 2; i++)
        {
            if (transform.GetChild(i).CompareTag("Duck"))
            {
                manager.goalAmounts[i]--;
                var goalName = "Goal" + i;
                var goalCubeNo = PlayerPrefs.GetInt(goalName, random);
                PlayerPrefs.SetInt(goalName, goalCubeNo);
            }
        }
        
        SetGoalTexts();
    }
    
    private void CheckGoalForBalloon()
    {
        for (var i = 0; i < 2; i++)
        {
            if (transform.GetChild(i).CompareTag("Balloon"))
            {
                manager.goalAmounts[i]--;
                var goalName = "Goal" + i;
                var goalCubeNo = PlayerPrefs.GetInt(goalName, random);
                PlayerPrefs.SetInt(goalName, goalCubeNo);
            }
        }
        
        SetGoalTexts();
    }
}
