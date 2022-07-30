using System;
using DG.Tweening;
using UnityEngine;
using static Actions;

public class Dot : MonoBehaviour
{
    [HideInInspector]public int column;
    [HideInInspector]public int row;
    [HideInInspector]public GameObject group;
    private Vector2 tempPos;
    private SpriteRenderer spriteRenderer;


    private void Start()
    {
        OnDotDestroyed();
    }

    private void OnEnable()
    {
        DotDestroyed += OnDotDestroyed;
    }
    
    private void OnDisable()
    {
        DotDestroyed -= OnDotDestroyed;
    }

    private void OnDotDestroyed()
    {
        tempPos = GameManager.Instance.matrixTransforms[column, row];
        transform.DOMove(tempPos, 1).OnComplete(SetDot);
        if (TryGetComponent(out SpriteRenderer renderer)) renderer.sortingOrder = row;
    }

    private void SetDot()
    {
        transform.position = tempPos;
        GameManager.Instance.allDots[column, row] = gameObject;
    }

    private void OnMouseUp()
    {
        if (GameManager.Instance.isPlayable)
        {
            GameManager.Instance.isPlayable = false;
            if (transform.parent.childCount > 1)
            {
                StartCoroutine(GameManager.Instance.DestroyDots(group));
            }
            else
            {
                Debug.Log("You cannot destroy only one dot.");
                GameManager.Instance.isPlayable = true;
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

    public void JumpToGoal(Vector3 goalPosition)
    {
        if (TryGetComponent(out SpriteRenderer spriteRenderer))
            spriteRenderer.sortingOrder = 21;
        transform.DOJump(goalPosition, 1, 1, 1).OnComplete(DestroyThisDot);
    }

    public void DestroyThisDot()
    {
        Destroy(gameObject);
    }

}
