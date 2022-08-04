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
    [SerializeField]private List<int> cubeAmount = new List<int>();

    private void Start()
    {
        var amount = 0;
        var count = GameManager.Instance.cubes.Length;
        for (var i = 0; i < count; i++ )
        {
            cubeAmount.Add(amount);
            amount++;
        }
    }

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
            
            random = Random.Range(0, cubeAmount.Count - 2);
            random = cubeAmount[random];
            cubeAmount.Remove(random);
            
            var goalName = "Goal" + i;
            var goalCubeNo = PlayerPrefs.GetInt(goalName, random);

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
            return GetExcludedRandomValue();
        
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
