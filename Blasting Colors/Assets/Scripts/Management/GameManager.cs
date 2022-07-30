using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using static Actions;

public class GameManager : MonoSingleton<GameManager>
{
    [HideInInspector] public int width;
    [HideInInspector] public int height;
    public int moves = 30;
    public List<int> goalAmounts;
    [HideInInspector]public float offset;
    private GameObject groups;
    public GameObject[,] allDots;
    public Vector2[,] matrixTransforms;
    [HideInInspector]public bool isPlayable;

    public GameObject[] dots;
    [HideInInspector]public List<GameObject> goalList = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        moves = 30;
        allDots = new GameObject[width, height];
        InitialSetUp();
    }

    private void InitialSetUp()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null)
                {
                    //Vector2 temPos = new Vector2(i, j + offset);
                    Vector2 temPos = matrixTransforms[i, j];
                    temPos.y += offset * j * 0.5f;
                    int dotToUse = Random.Range(0, dots.Length);

                    GameObject dot = Instantiate(dots[dotToUse], temPos, Quaternion.identity);
                    //dot.name = "( " + i + ", " + j + " )";
                    allDots[i, j] = dot;
                    if (dot.TryGetComponent(out Dot dotScript))
                    {
                        dotScript.column = i;
                        dotScript.row = j;
                    }
                }
            }
        }

        Grouping();

        isPlayable = true;
    }

    private IEnumerator SetUp()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null)
                {
                    //Vector2 temPos = new Vector2(i, j + offset);
                    Vector2 temPos = matrixTransforms[i, j];
                    temPos.y += offset * j * 0.5f;
                    int dotToUse = Random.Range(0, dots.Length);

                    GameObject dot = Instantiate(dots[dotToUse], temPos, Quaternion.identity);
                    //dot.name = "( " + i + ", " + j + " )";
                    allDots[i, j] = dot;
                    if (dot.TryGetComponent(out Dot dotScript))
                    {
                        dotScript.column = i;
                        dotScript.row = j;
                    }

                    yield return new WaitForSeconds(0.1f);
                }
            }
        }

        Grouping();

        isPlayable = true;
    }

    #region SetUp

    private void CheckShuffling() // Shuffles if all the dots are different from each other.
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

            StartCoroutine(SetUp());
        }
    }

    private void Grouping() // Resets all the groups.
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

        CheckGroups();
    }

    private void RemoveGroups(int column, int row)
    {
        allDots[column, row].transform.parent = null;
        if (allDots[column, row].TryGetComponent(out Dot dot))
        {
            if (dot.group != null)
            {
                dot.group.SetActive(false);
                dot.group = null;
            }

            dot.CreateGroup();
        }
    }

    private void CheckGroups() // Gather all adjacent dots with the same color under one group.
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                //Checking Left.
                if (i > 0)
                {
                    if (allDots[i - 1, j] != null && allDots[i, j] != null)
                    {
                        if (allDots[i - 1, j].CompareTag(allDots[i, j].tag))
                        {
                            if (allDots[i - 1, j].transform.parent.TryGetComponent<Grouping>(out var grouping))
                                grouping.ChangeGroup(allDots[i, j].transform.parent.gameObject);
                        }
                    }
                }

                //Checking Down.
                if (j > 0)
                {
                    if (allDots[i, j - 1] != null && allDots[i, j] != null)
                    {
                        if (allDots[i, j - 1].tag == allDots[i, j].tag)
                        {
                            if (allDots[i, j - 1].transform.parent.TryGetComponent<Grouping>(out var grouping))
                                grouping.ChangeGroup(allDots[i, j].transform.parent.gameObject);
                        }
                    }
                }
            }
        }

        CheckShuffling();
    }

    public void DestroyDotsAt(int column, int row) // Destroys the selected dot.
    {
        Destroy(allDots[column, row]);
        allDots[column, row] = null;
    }

    public IEnumerator DestroyDots(GameObject group) // Destroy all the dots in the selected dot.
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null && allDots[i, j].transform.parent == group.transform)
                {
                    if (allDots[i, j].CompareTag(goalList[0].tag))
                    {
                        if (allDots[i, j].TryGetComponent(out Dot dot))
                        {
                            dot.JumpToGoal(goalList[0].transform.position);
                            allDots[i, j] = null;
                            goalAmounts[0]--;
                        }

                        yield return new WaitForSeconds(0.05f);
                    }
                    else if (allDots[i, j].CompareTag(goalList[1].tag))
                    {
                        if (allDots[i, j].TryGetComponent(out Dot dot))
                        {
                            dot.JumpToGoal(goalList[1].transform.position);
                            allDots[i, j] = null;
                            goalAmounts[1]--;
                        }
                        
                        yield return new WaitForSeconds(0.05f);
                    }
                    else
                        DestroyDotsAt(i, j);
                }
            }
        }

        Pool.Instance.DeactivateObject(group);
        DecreaseRow();
    }

    private void DecreaseRow() // Fills the empty places.
    {
        var nullCount = 0;
        for (var i = 0; i < width; i++)
        {
            for (var j = 0; j < height; j++)
            {
                if (allDots[i, j] == null)
                {
                    nullCount++;
                }
                else if (nullCount > 0)
                {
                    if (allDots[i, j].TryGetComponent(out Dot dot))
                    {
                        dot.row -= nullCount;
                        allDots[i, dot.row] = allDots[i, j];
                        allDots[i, j] = null;
                    }
                }
            }

            nullCount = 0;
        }

        DotDestroyed?.Invoke();
        StartCoroutine(SetUp());
    }

    private bool ShouldShuffle() // Checking if shuffling necessary.
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

        return count <= 1;
    }

    public void DeactivateEmptyGroups(GameObject deactive) // Deactivates if the group object is empty.
    {
        deactive.gameObject.SetActive(false);
    }

    #endregion
}