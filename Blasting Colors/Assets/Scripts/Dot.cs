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
    [HideInInspector]public int targetX;
    [HideInInspector]public int targetY;
    private Board board;
    private Vector2 tempPos;
    public GameObject groupPrefab;
    public GameObject group;


    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        targetX = (int) transform.position.x;
        targetY = (int) transform.position.y;
        row = targetY;
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
        board.GetComponent<Board>().DestroyDots(group);
    }

    public void CreateGroup()
    {
        group = Instantiate(groupPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
        transform.parent = group.transform;
        group.gameObject.GetComponent<Grouping>().Add(gameObject.tag.ToString());
    }

}
