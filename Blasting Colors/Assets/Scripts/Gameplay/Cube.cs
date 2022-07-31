﻿using System;
using DG.Tweening;
using UnityEngine;
using static Actions;

public class Cube : MonoBehaviour
{
    [HideInInspector]public int column;
    [HideInInspector]public int row;
    [HideInInspector]public GameObject group;
    private Vector2 tempPos;
    private SpriteRenderer spriteRenderer;
    private int goalNo;


    private bool isDestroyed;
    public bool isBalloon;
    public bool isDuck;

    private void Start()
    {
        OnTurnEnded();
    }

    private void OnEnable()
    {
        TurnEnded += OnTurnEnded;
    }
    
    private void OnDisable()
    {
        TurnEnded -= OnTurnEnded;
    }

    private void OnTurnEnded()
    {
        if (isDestroyed) return;
        tempPos = GameManager.Instance.matrixTransforms[column, row];
        transform.DOMove(tempPos, 0.5f).SetEase(Ease.OutBounce).OnComplete(SetCube);
        if (TryGetComponent(out SpriteRenderer renderer)) renderer.sortingOrder = row;
    }

    private void SetCube()
    {
        transform.position = tempPos;
        GameManager.Instance.allCubes[column, row] = gameObject;
        
        if (isDuck && row == 0)
            DestroyThisCube();
    }

    private void OnMouseUp()
    {
        if (GameManager.Instance.isPlayable)
        {
            GameManager.Instance.isPlayable = false;
            if (isDuck || isBalloon || transform.parent.childCount <= 1)
            {
                if (TryGetComponent(out Animator animator)) animator.SetTrigger("isWrong");
                GameManager.Instance.isPlayable = true;
            }
            else
                StartCoroutine(GameManager.Instance.DestroyCubes(group));
            
        }
    }

    public void CreateGroup()
    {
        group = Pool.Instance.SpawnObject(Vector3.zero, "Group", null);
        if (group != null)
        {
            group.SetActive(true);
            transform.SetParent(group.transform);
        }
    }

    public void JumpToGoal(int goalNo)
    {
        this.goalNo = goalNo;
        if (TryGetComponent(out SpriteRenderer spriteRenderer))
            spriteRenderer.sortingOrder = 21;
        transform.SetParent(null);
        isDestroyed = true;

        var goalPosition = GameManager.Instance.goalList[goalNo].transform.position;
        transform.DOJump(goalPosition, -5, 1, 1).OnComplete(DestroyThisCube);
    }

    public void DestroyThisCube()
    {
        if (isDuck || isBalloon)
        {
            StartCoroutine(GameManager.Instance.DestroyCubes(group));
        }
        else
        {
            Pool.Instance.SpawnObject(transform.position, "StarParticle", null, 1f);
            GameManager.Instance.goalAmounts[goalNo]--;
            GoalAmountChanged?.Invoke();
        }
        
        Destroy(gameObject);
    }

}
