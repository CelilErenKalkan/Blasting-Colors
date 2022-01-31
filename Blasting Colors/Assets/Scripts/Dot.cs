using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using Group = System.Text.RegularExpressions.Group;

public class Dot : MonoBehaviour
{
    public int column;
    public int row;
    private int speed = 5;
    [HideInInspector] public int targetX;
    [HideInInspector] public int targetY;
    private Board board;
    private Vector2 tempPos;
    public GameObject group;
    private SpriteRenderer spriteRenderer;


    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        targetX = (int)transform.position.x;
        targetY = (int)transform.position.y;
        row = targetY - board.GetComponent<Board>().offset;
        column = targetX;
    }

    // Update is called once per frame
    void Update()
    {
        targetX = column;
        targetY = row;
        if (Mathf.Abs(targetX - transform.position.x) > .1)
        {
            //Move Towards Target.
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPos, Time.deltaTime * speed);
            if (board.allDots[column, row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
            }
        }
        else
        {
            //Directly Set the Position.
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position = tempPos;
            board.allDots[column, row] = this.gameObject;
        }

        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            //Move Towards Target.
            tempPos = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPos, Time.deltaTime * speed);
            if (board.allDots[column, row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
            }
        }
        else
        {
            //Directly Set the Position.
            tempPos = new Vector2(transform.position.x, targetY);
            transform.position = tempPos;
            board.allDots[column, row] = this.gameObject;
        }
    }

    private void OnMouseUp()
    {
        if (transform.parent.childCount > 1)
            board.GetComponent<Board>().DestroyDots(group);
        else
            Debug.Log("You cannot destroy only one dot.");
    }

    public void CreateGroup()
    {
        group = Board.singleton.Get("Group");
        if (group != null)
        {
            group.SetActive(true);
            transform.parent = group.transform;
        }
    }

    public void CheckIcon()
    {
        var amount = transform.parent.childCount;
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        if (amount < 5)
        {
            spriteRenderer.sprite = Resources.Load<Sprite>("2D/" + gameObject.tag.ToString() + "/" + gameObject.tag.ToString() + "_Default") as Sprite;
        }
        else if (amount < 8)
        {
            spriteRenderer.sprite = Resources.Load<Sprite>("2D/" + gameObject.tag.ToString() + "/" + gameObject.tag.ToString() + "_A") as Sprite;
        }
        else if (amount < 10)
        {
            spriteRenderer.sprite = Resources.Load<Sprite>("2D/" + gameObject.tag.ToString() + "/" + gameObject.tag.ToString() + "_B") as Sprite;
        }
        else
        {
            spriteRenderer.sprite = Resources.Load<Sprite>("2D/" + gameObject.tag.ToString() + "/" + gameObject.tag.ToString() + "_C") as Sprite;
        }
    }

}
