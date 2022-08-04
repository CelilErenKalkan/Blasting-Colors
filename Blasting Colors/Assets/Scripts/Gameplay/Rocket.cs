using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [SerializeField]private bool isRight;

    public void Launch(bool isHorizontal)
    {
        var targetTime = 0f;
        var targetDistance = 0f;
        var target = transform.position;
        
        if (isHorizontal)
        {
            targetTime = GameManager.Instance.width * 0.12f;
            targetDistance = GameManager.Instance.width * GameManager.Instance.offset * 2;

            if (isRight)
                target.x += targetDistance;
            else
                target.x -= targetDistance;
            transform.DOMove(target, targetTime);
        }
        else
        {
            targetTime = GameManager.Instance.height * 0.12f;
            targetDistance = GameManager.Instance.height * GameManager.Instance.offset * 2;
            
            if (isRight)
                target.y += targetDistance;
            else
                target.y -= targetDistance;
            transform.DOMove(target, targetTime);
        }
    }
}
