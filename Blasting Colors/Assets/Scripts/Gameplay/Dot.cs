using UnityEngine;

public class Dot : MonoBehaviour
{
    [HideInInspector]public int column;
    [HideInInspector]public int row;
    [HideInInspector]public GameObject group;
    private int speed = 5;
    private SpriteRenderer spriteRenderer;

    // Update is called once per frame
    void Update()
    {
        var tempPos = GameManager.Instance.matrixTransforms[column, row];
        
        if (Mathf.Abs(tempPos.x - transform.position.x) > .1)
        {
            //Move Towards Target.
            transform.position = Vector2.Lerp(transform.position, tempPos, Time.deltaTime * speed);
        }
        else
        {
            //Directly Set the Position.
            transform.position = tempPos;
            GameManager.Instance.allDots[column, row] = gameObject;
        }

        if (Mathf.Abs(tempPos.y - transform.position.y) > .1)
        {
            //Move Towards Target.
            transform.position = Vector2.Lerp(transform.position, tempPos, Time.deltaTime * speed);
        }
        else
        {
            //Directly Set the Position.
            transform.position = tempPos;
            GameManager.Instance.allDots[column, row] = gameObject;
        }
    }

    private void OnMouseUp()
    {
        if (GameManager.Instance.isPlayable)
        {
            GameManager.Instance.isPlayable = false;
            if (transform.parent.childCount > 1)
                GameManager.Instance.DestroyDots(group);
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

}
