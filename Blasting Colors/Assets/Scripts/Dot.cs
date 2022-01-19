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
    public int targetX;
    public int targetY;
    public bool isMatched = false;
    private Board board;
    private GameObject otherDot;
    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;
    private Vector2 tempPos;
    public float swipeAngle = 0;
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
        }
        else
        {
            //Directly Set the Position.
            tempPos = new Vector2(transform.position.x, targetY);
            transform.position = tempPos;
            board.allDots[column, row] = this.gameObject;
        }
    }

    private void OnMouseDown()
    {
        firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

    }

    private void OnMouseUp()
    {
        finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CalculateAngle();

    }

    void CalculateAngle()
    {
        swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180/Mathf.PI;
        MovePieces();
    }

    void MovePieces()
    {
        if (swipeAngle > -45 && swipeAngle <= 45 && column < board.width)
        {
            //Right Swipe.
            otherDot = board.allDots[column + 1, row];
            otherDot.GetComponent<Dot>().column -= 1;
            column += 1;
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height)
        {
            //Up Swipe.
            otherDot = board.allDots[column, row + 1];
            otherDot.GetComponent<Dot>().row -= 1;
            row += 1;
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
        {
            //Left Swipe.
            otherDot = board.allDots[column - 1, row];
            otherDot.GetComponent<Dot>().column += 1;
            column -= 1;
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            //Down Swipe.
            otherDot = board.allDots[column , row - 1];
            otherDot.GetComponent<Dot>().row += 1;
            row -= 1;
        }
    }

    void FindMatches()
    {
        //Checking columns.
        if (column > 0 && column < board.width - 1)
        {
            GameObject leftDot1 = board.allDots[column - 1, row];
            GameObject rightDot1 = board.allDots[column + 1, row];
            if (leftDot1.tag == this.gameObject.tag && rightDot1.tag == this.gameObject.tag)
            {
                leftDot1.GetComponent<Dot>().isMatched = true;
                rightDot1.GetComponent<Dot>().isMatched = true;
                isMatched = true;
            }
        }

        //Checking rows.
        if (row > 0 && row < board.height - 1)
        {
            GameObject upDot1 = board.allDots[column, row + 1];
            GameObject downDot1 = board.allDots[column, row - 1];
            if (upDot1.tag == this.gameObject.tag && downDot1.tag == this.gameObject.tag)
            {
                upDot1.GetComponent<Dot>().isMatched = true;
                downDot1.GetComponent<Dot>().isMatched = true;
                isMatched = true;
            }
        }
    }

    public void CreateGroup()
    {
        Debug.Log("Group Created.");
        group = Instantiate(groupPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
        transform.parent = group.transform;
        group.gameObject.GetComponent<Grouping>().Add(gameObject.tag.ToString());
    }

    //public void FindGroup()
    //{
    //    if (group == null)
    //    {
    //        Debug.Log("Group Created.");
    //        group = Instantiate(groupPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
    //        transform.parent = group.transform;
    //        group.gameObject.GetComponent<Grouping>().Add(gameObject.tag.ToString());
    //    }

    //    //Checking columns.
    //    if (column > 0 && column < board.width - 1)
    //    {
    //        Debug.Log("Column");
    //        GameObject leftDot1 = board.allDots[column - 1, row];
    //        GameObject rightDot1 = board.allDots[column + 1, row];
    //        if (rightDot1.tag == this.gameObject.tag && rightDot1.GetComponent<Dot>().group == null)
    //        {
    //            Debug.Log("Member Added.");
    //            rightDot1.transform.parent = group.transform;
    //        }
    //        else if (rightDot1.tag == this.gameObject.tag && rightDot1.GetComponent<Dot>().group != group)
    //        {
    //            Debug.Log("Group Changed.");
    //            if (rightDot1.GetComponent<Dot>().group.transform.childCount >
    //                group.transform.childCount)
    //            {
    //                group.GetComponent<Grouping>().ChangeGroup(rightDot1.transform.parent.gameObject);
    //            }
    //            else if (rightDot1.GetComponent<Dot>().group.transform.childCount <=
    //                     group.transform.childCount)
    //            {
    //                rightDot1.transform.parent.GetComponent<Grouping>().ChangeGroup(transform.parent.gameObject);
    //            }
    //        }

    //        if (leftDot1.tag == this.gameObject.tag && leftDot1.GetComponent<Dot>().group == null)
    //        {
    //            leftDot1.transform.parent = group.transform;
    //        }
    //        else if (leftDot1.tag == this.gameObject.tag && leftDot1.GetComponent<Dot>().group != group)
    //        {
    //            if (leftDot1.GetComponent<Dot>().group.transform.childCount >
    //                group.transform.childCount)
    //            {
    //                group.GetComponent<Grouping>().ChangeGroup(leftDot1.transform.parent.gameObject);
    //            }
    //        }
    //    }

    //    //Checking rows.
    //    if (row > 0 && row < board.height - 1)
    //    {
    //        Debug.Log("Column");
    //        GameObject upDot1 = board.allDots[column, row + 1];
    //        GameObject downDot1 = board.allDots[column, row - 1];
    //        if (upDot1.tag == this.gameObject.tag && upDot1.GetComponent<Dot>().group == null)
    //        {
    //            upDot1.transform.parent = group.transform;
    //        }
    //        else if (upDot1.tag == this.gameObject.tag && upDot1.GetComponent<Dot>().group != group)
    //        {
    //            if (upDot1.GetComponent<Dot>().group.transform.childCount >
    //                group.transform.childCount)
    //            {
    //                group.GetComponent<Grouping>().ChangeGroup(upDot1.transform.parent.gameObject);
    //            }
    //            else if (upDot1.GetComponent<Dot>().group.transform.childCount <=
    //                     group.transform.childCount)
    //            {
    //                upDot1.transform.parent.GetComponent<Grouping>().ChangeGroup(transform.parent.gameObject);
    //            }
    //        }

    //        if (downDot1.tag == this.gameObject.tag && downDot1.GetComponent<Dot>().group == null)
    //        {
    //            downDot1.transform.parent = group.transform;
    //        }
    //        else if (downDot1.tag == this.gameObject.tag && downDot1.GetComponent<Dot>().group != group)
    //        {
    //            if (downDot1.GetComponent<Dot>().group.transform.childCount >
    //                group.transform.childCount)
    //            {
    //                group.GetComponent<Grouping>().ChangeGroup(downDot1.transform.parent.gameObject);
    //            }
    //        }
    //    }
    //}
}
