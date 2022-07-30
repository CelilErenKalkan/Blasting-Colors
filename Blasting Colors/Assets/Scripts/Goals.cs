using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Goals : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    { 
        SetGoals();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetGoals()
    {
        for (var i = 0; i < 2; i++)
        {
            var random = Random.Range(0, GameManager.Instance.dots.Length);
            var goalName = "Goal" + i;
            var goalDotNo = PlayerPrefs.GetInt(goalName, random);
            GameManager.Instance.goalList.Add(transform.GetChild(i).gameObject);
            transform.GetChild(i).tag = GameManager.Instance.dots[goalDotNo].tag;
            if (transform.GetChild(i).TryGetComponent(out Image image) &&
                GameManager.Instance.dots[goalDotNo].TryGetComponent(out SpriteRenderer dotImage))
                image.sprite = dotImage.sprite;
            PlayerPrefs.SetInt(goalName, goalDotNo);
        }
    }
}
