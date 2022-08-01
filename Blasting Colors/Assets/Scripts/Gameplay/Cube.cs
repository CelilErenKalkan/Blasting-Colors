using System;
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
    private GameManager _manager;

    private bool isDestroyed;
    public bool isBalloon;
    public bool isDuck;

    private void Start()
    {
        _manager = GameManager.Instance;
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
        tempPos = _manager.matrixTransforms[column, row];
        transform.DOMove(tempPos, 0.5f).SetEase(Ease.OutBounce).OnComplete(SetCube);
        if (TryGetComponent(out SpriteRenderer renderer)) renderer.sortingOrder = row;
    }

    private void SetCube()
    {
        transform.position = tempPos;
        _manager.allCubes[column, row] = gameObject;

        if (isDuck && row == 0)
            DestroyThisCube();
    }

    private void OnMouseUp()
    {
        if (_manager.isPlayable)
        {
            _manager.isPlayable = false;
            if (isDuck || isBalloon || transform.parent.childCount <= 1)
            {
                if (TryGetComponent(out Animator animator)) animator.SetTrigger("isWrong");
            }
            else
            {
                if (group.transform.childCount >= 5)
                    _manager.rocketCenter = transform;
                
                StartCoroutine(_manager.DestroyCubes(group));
            }
            
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

        var goalPosition = _manager.goalList[goalNo].transform.position;
        transform.DOJump(goalPosition, -5, 1, 1).OnComplete(DestroyThisCube);
    }

    public void JoinToTheRocket()
    {
        if (TryGetComponent(out SpriteRenderer spriteRenderer))
            spriteRenderer.sortingOrder = 21;
        transform.SetParent(null);
        isDestroyed = true;

        var targetPosition = _manager.rocketCenter.position;
        transform.DOMove(targetPosition, 0.25f).OnComplete(DestroyThisCube);
    }

    private void DestroyThisCube()
    {
        if (isDuck)
        {
            StartCoroutine(_manager.DestroyCubes(group));
        }
        else if (isBalloon)
        {
            Pool.Instance.SpawnObject(transform.position, "BalloonParticle", null, 1f);
            _manager.allCubes[column, row] = null;
            BalloonDestroyed?.Invoke();
        }
        else if (gameObject.CompareTag(_manager.goalList[goalNo].tag))
        {
            Pool.Instance.SpawnObject(transform.position, "StarParticle", null, 1f);
            _manager.goalAmounts[goalNo]--;
            GoalAmountChanged?.Invoke();
        }

        Destroy(gameObject);
    }

    public void SetIsPlayable()
    {
        _manager.isPlayable = true;
    }

}
