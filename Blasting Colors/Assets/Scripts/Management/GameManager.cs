using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Actions;
using Random = UnityEngine.Random;

public class GameManager : MonoSingleton<GameManager>
{
    [HideInInspector] public int width;
    [HideInInspector] public int height;
    [HideInInspector]public float offset;
    [HideInInspector]public bool isPlayable;
    [HideInInspector]public List<GameObject> goalList = new List<GameObject>();
    
    public int moves = 30;
    public List<int> goalAmounts;
    private GameObject groups;
    public GameObject[,] allCubes;
    public Vector2[,] matrixTransforms;
    public GameObject[] cubes;

    private void OnEnable()
    {
        TurnEnded += CheckIsGameFinished;
        GoalAmountChanged += CheckIsGameFinished;
        LevelStart += OnGameStarted;
    }
    
    private void OnDisable()
    {
        TurnEnded -= CheckIsGameFinished;
        GoalAmountChanged -= CheckIsGameFinished;
        LevelStart -= OnGameStarted;
    }

    private void OnGameStarted()
    {
        moves = 30;
        allCubes = new GameObject[width, height];
        //InitialSetUp();
        StartCoroutine(SetUp(1));
    }

    private void CheckIsGameFinished()
    {
        var isgoalAmountFinished = true;
        foreach (var goalAmount in goalAmounts)
        {
            if (goalAmount > 0) isgoalAmountFinished = false;
        }
        
        if (isgoalAmountFinished)
        {
            isPlayable = false;
            LevelSuccess?.Invoke();
        }
        else if (moves <= 0)
        {
            isPlayable = false;
            LevelFailed?.Invoke();
        }
    }

    #region GameplayMechanic

    private IEnumerator SetUp(float timeIndex)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allCubes[i, j] == null)
                {
                    Vector2 temPos = matrixTransforms[i, j];
                    temPos.y += offset * j * 0.5f;
                    int cubeToUse = Random.Range(0, cubes.Length);

                    GameObject cube = Instantiate(cubes[cubeToUse], temPos, Quaternion.identity);
                    allCubes[i, j] = cube;
                    if (cube.TryGetComponent(out Cube cubeScript))
                    {
                        cubeScript.column = i;
                        cubeScript.row = j;
                    }

                    yield return new WaitForSeconds(0.005f * timeIndex);
                }
            }
        }

        Grouping();

        isPlayable = true;
    }
    
    private void CheckShuffling() // Shuffles if all the dots are different from each other.
    {
        if (ShouldShuffle())
        {
            Debug.Log("Shuffling the Board.");
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    var dot = allCubes[i, j];
                    allCubes[i, j] = null;
                    Destroy(dot);
                }
            }

            StartCoroutine(SetUp(10));
        }
    }

    private void Grouping() // Resets all the groups.
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allCubes[i, j] != null)
                {
                    RemoveGroups(i, j);
                }
            }
        }

        CheckGroups();
    }

    private void RemoveGroups(int column, int row)
    {
        allCubes[column, row].transform.parent = null;
        if (allCubes[column, row].TryGetComponent(out Cube cube))
        {
            if (cube.group != null)
            {
                cube.group.SetActive(false);
                cube.group = null;
            }

            cube.CreateGroup();
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
                    if (allCubes[i - 1, j] != null && allCubes[i, j] != null)
                    {
                        if (allCubes[i - 1, j].CompareTag(allCubes[i, j].tag))
                        {
                            if (allCubes[i - 1, j].transform.parent.TryGetComponent<Grouping>(out var grouping))
                                grouping.ChangeGroup(allCubes[i, j].transform.parent.gameObject);
                        }
                    }
                }

                //Checking Down.
                if (j > 0)
                {
                    if (allCubes[i, j - 1] != null && allCubes[i, j] != null)
                    {
                        if (allCubes[i, j - 1].CompareTag(allCubes[i, j].tag))
                        {
                            if (allCubes[i, j - 1].transform.parent.TryGetComponent<Grouping>(out var grouping))
                                grouping.ChangeGroup(allCubes[i, j].transform.parent.gameObject);
                        }
                    }
                }
            }
        }

        CheckShuffling();
    }

    public void DestroyCubesAt(int column, int row) // Destroys the selected dot.
    {
        Destroy(allCubes[column, row]);
        allCubes[column, row] = null;
    }

    public IEnumerator DestroyCubes(GameObject group) // Destroy all the dots in the selected dot.
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allCubes[i, j] != null && allCubes[i, j].transform.parent == group.transform)
                {
                    if (allCubes[i, j].CompareTag(goalList[0].tag))
                    {
                        if (allCubes[i, j].TryGetComponent(out Cube cube))
                        {
                            cube.JumpToGoal(0);
                            allCubes[i, j] = null;
                        }

                        yield return new WaitForSeconds(0.05f);
                    }
                    else if (allCubes[i, j].CompareTag(goalList[1].tag))
                    {
                        if (allCubes[i, j].TryGetComponent(out Cube cube))
                        {
                            cube.JumpToGoal(1);
                            allCubes[i, j] = null;
                        }
                        
                        yield return new WaitForSeconds(0.05f);
                    }
                    else
                        DestroyCubesAt(i, j);
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
                if (allCubes[i, j] == null)
                {
                    nullCount++;
                }
                else if (nullCount > 0)
                {
                    if (allCubes[i, j].TryGetComponent(out Cube cube))
                    {
                        cube.row -= nullCount;
                        allCubes[i, cube.row] = allCubes[i, j];
                        allCubes[i, j] = null;
                    }
                }
            }

            nullCount = 0;
        }

        TurnEnded?.Invoke();
        StartCoroutine(SetUp(10));
    }

    private bool ShouldShuffle() // Checking if shuffling necessary.
    {
        var count = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allCubes[i, j].transform.parent.childCount > 1)
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