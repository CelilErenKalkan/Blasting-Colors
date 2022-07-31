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
    private void OnEnable()
    {
        GoalAmountChanged += SetGoalTexts;
        LevelStart += SetGoals;
    }
    
    private void OnDisable()
    {
        GoalAmountChanged -= SetGoalTexts;
        LevelStart -= SetGoals;
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
        var count = GameManager.Instance.goalAmounts.Count;
        for (var i = 0; i < count; i++)
        {
            if (transform.GetChild(i).GetChild(0).TryGetComponent(out TMP_Text text))
            {
                if (GameManager.Instance.goalAmounts[i] > 0)
                    text.text = GameManager.Instance.goalAmounts[i].ToString();
                else
                {
                    transform.GetChild(i).GetChild(1).gameObject.SetActive(true);
                    text.text = " ";
                }
            }
        }
    }
}
