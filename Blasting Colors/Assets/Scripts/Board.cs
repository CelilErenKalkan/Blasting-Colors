using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int width;
    public int height;
    public int offset;
    public GameObject tilePrefab;
    private BackgroundTile[,] allTiles;
    public GameObject[] dots;
    public GameObject[,] allDots;

    // Start is called before the first frame update
    void Start()
    {
        allTiles = new BackgroundTile[width, height];
        allDots = new GameObject[width, height];
        SetUp();
    }

    private void SetUp()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null)
                {
                    Vector2 temPos = new Vector2(i, j + offset);
                    int dotToUse = Random.Range(0, dots.Length);
                    GameObject dot = Instantiate(dots[dotToUse], temPos, Quaternion.identity);
                    dot.name = "( " + i + ", " + j + " )";
                    allDots[i, j] = dot;
                }
            }
        }
        Grouping();
        CheckGroups();
        CheckIcon();
        CheckShuffling();
    }

    private void CheckShuffling()
    {
        if (ShouldShuffle())
        {
            Debug.Log("Shuffling the Board.");
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    var dot = allDots[i, j];
                    allDots[i, j] = null;
                    Destroy(dot);
                }
            }
            SetUp();
        }
    }

    private void Grouping()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    RemoveGroups(i, j);
                }
            }
        }
    }

    private void RemoveGroups(int column, int row)
    {
        allDots[column, row].GetComponent<Dot>().group = null;
        allDots[column, row].transform.parent = null;
        allDots[column, row].GetComponent<Dot>().CreateGroup();
    }

    private void CheckGroups()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                //Checking Right.
                if (i < width - 1)
                {
                    if (allDots[i, j] != null && allDots[i + 1, j] != null)
                    {
                        if (allDots[i, j].tag == allDots[i + 1, j].tag)
                        {
                            if (allDots[i, j].transform.parent.childCount >=
                                allDots[i + 1, j].transform.parent.childCount)
                            {
                                allDots[i + 1, j].transform.parent.gameObject.GetComponent<Grouping>()
                                    .ChangeGroup(allDots[i, j].transform.parent.gameObject);
                            }
                            else if (allDots[i, j].transform.parent.childCount <
                                     allDots[i + 1, j].transform.parent.childCount)
                            {
                                allDots[i, j].transform.parent.gameObject.GetComponent<Grouping>()
                                    .ChangeGroup(allDots[i + 1, j].transform.parent.gameObject);
                            }
                        }
                    }
                }

                //Checking Up.
                if (j < height - 1)
                {
                    if (allDots[i, j] != null && allDots[i, j + 1] != null)
                    {
                        if (allDots[i, j].tag == allDots[i, j + 1].tag)
                        {
                            if (allDots[i, j].transform.parent.childCount >=
                                allDots[i, j + 1].transform.parent.childCount)
                            {
                                allDots[i, j + 1].transform.parent.gameObject.GetComponent<Grouping>()
                                    .ChangeGroup(allDots[i, j].transform.parent.gameObject);
                            }
                            else if (allDots[i, j].transform.parent.childCount <
                                     allDots[i, j + 1].transform.parent.childCount)
                            {
                                allDots[i, j].transform.parent.gameObject.GetComponent<Grouping>()
                                    .ChangeGroup(allDots[i, j + 1].transform.parent.gameObject);
                            }
                        }
                    }
                }
            }
        }
    }

    private void CheckIcon()
    {
        foreach (var dot in allDots)
        {
            dot.GetComponent<Dot>().CheckIcon();
        }
    }

    private void DestroyDotsAt(int column, int row)
    {
        Destroy(allDots[column, row]);
        allDots[column, row] = null;
    }

    public void DestroyDots(GameObject group)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null && allDots[i, j].transform.parent == group.transform)
                {
                    DestroyDotsAt(i, j);
                }
            }
        }

        Destroy(group, 0.1f);
        StartCoroutine(DecreaseRow());
    }

    private IEnumerator DecreaseRow()
    {
        int nullCount = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null)
                {
                    nullCount++;
                }
                else if (nullCount > 0)
                {
                    allDots[i, j].GetComponent<Dot>().row -= nullCount;
                    allDots[i, allDots[i, j].GetComponent<Dot>().row] = allDots[i, j];
                    allDots[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(.4f);
        SetUp();
        yield return new WaitForSeconds(.1f);
    }

    private bool ShouldShuffle()
    {
        var count = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j].transform.parent.childCount > 1)
                    count++;
            }
        }

        if (count > 1)
            return false;
        else
            return true;
    }
}
