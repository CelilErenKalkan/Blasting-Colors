using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using static Actions;
using Random = UnityEngine.Random;

public class Cube : MonoBehaviour
{
    [HideInInspector]public int column;
    [HideInInspector]public int row;
    [HideInInspector]public GameObject group;
    private Vector2 tempPos;
    private SpriteRenderer spriteRenderer;
    private int goalNo;
    private GameManager _manager;

    [HideInInspector]public bool isDestroyed;
    public bool isBalloon;
    public bool isDuck;
    public bool isRocket;
    public bool isHorizontal;

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
        {
            isDestroyed = true;
            _manager.isPlayable = false;
        }
    }

    private void OnMouseUp()
    {
        if (_manager.isPlayable)
        {
            _manager.isPlayable = false;
            if (isRocket)
                LaunchTheRocket();
            else if (isDuck || isBalloon || transform.parent.childCount <= 1)
            {
                if (TryGetComponent(out Animator animator)) animator.SetTrigger("isWrong");
                _manager.moves++;
            }
            else
            {
                var isGoal = false;
                foreach (var goal in _manager.goalList)
                {
                    if (transform.CompareTag(goal.tag))
                    {
                        isGoal = true;
                        break;
                    }
                }

                if (group.transform.childCount >= 5 && !isGoal)
                {
                    _manager.rocketCenter = transform;
                    var targetScale = new Vector3(0.1f, 0.1f, 0.1f);
                    transform.DOScale(targetScale, 0.4f).SetEase(Ease.InBack).OnComplete(SetRocket);
                }
                
                StartCoroutine(_manager.DestroyCubes(column, row, group));
            }
            
            _manager.moves--;
            
        }
    }

    public void CreateGroup()
    {
        group = Pool.Instance.SpawnObject(Vector3.zero, PoolItemType.Group, null);
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
        transform.DOMove(targetPosition, 0.5f).SetEase(Ease.InBack).OnComplete(DestroyThisCube);
    }

    private void LaunchTheRocket()
    {
        foreach (Transform child in transform.GetChild(0))
        {
            if (child.TryGetComponent(out Rocket rocket)) rocket.Launch(isHorizontal);
        }
        
        StartCoroutine(_manager.LaunchRocket(column, row, isHorizontal));
    }

    private void DestroyThisCube()
    {
        if (isBalloon)
        {
            Pool.Instance.SpawnObject(transform.position, PoolItemType.BalloonPopExplosion, null, 1f);
            _manager.allCubes[column, row] = null;
            BalloonDestroyed?.Invoke();
        }
        else if (gameObject.CompareTag(_manager.goalList[goalNo].tag))
        {
            Pool.Instance.SpawnObject(transform.position, PoolItemType.SmokeExplosionWhite, null, 1f);
            _manager.goalAmounts[goalNo]--;
            GoalAmountChanged?.Invoke();
        }

        Destroy(gameObject);
    }

    public void DestroyDuck()
    {
        if (_manager.allCubes[column, row] != null)
        {
            Pool.Instance.SpawnObject(transform.position, PoolItemType.BalloonPopExplosion, null, 1f);
            _manager.allCubes[column, row] = null;
            DuckDestroyed?.Invoke();
            if (TryGetComponent(out SpriteRenderer renderer)) renderer.enabled = false;
            Destroy(gameObject);
        }
    }
    
    private void SetRocket()
    {
        var randomRocket = Random.Range(1, 3);
        if (randomRocket > 1)
            isHorizontal = true;
        Instantiate(_manager.cubes[_manager.cubes.Length - randomRocket], transform, false);
        isRocket = true;
        gameObject.tag = "Rocket";
        if (TryGetComponent(out SpriteRenderer spriteRenderer))
        {
            var order = spriteRenderer.sortingOrder;
            spriteRenderer.enabled = false;
            foreach (Transform child in transform.GetChild(0))
            {
                if (child.TryGetComponent(out SpriteRenderer renderer)) renderer.sortingOrder = order;
            }
        }
    }

    public void SetIsPlayable()
    {
        _manager.isPlayable = true;
    }

}
