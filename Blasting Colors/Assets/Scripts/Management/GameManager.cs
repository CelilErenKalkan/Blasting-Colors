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
    [HideInInspector] public float offset;
    [HideInInspector] public bool isPlayable;
    [HideInInspector] public List<GameObject> goalList = new List<GameObject>();

    public int moves = 30;
    public List<int> goalAmounts;
    private GameObject groups;
    public GameObject[,] allCubes;
    public Vector2[,] matrixTransforms;
    public GameObject[] cubes;
    public Transform rocketCenter;

    private bool _isOnce;
    
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
        isPlayable = true;
        StartCoroutine(SetUp(1));
    }

    private void CheckIsGameFinished()
    {
        var isGoalAmountFinished = true;
        foreach (var goalAmount in goalAmounts)
        {
            if (goalAmount > 0) isGoalAmountFinished = false;
        }

        if (isGoalAmountFinished)
        {
            LevelSuccess?.Invoke();
        }
        else if (moves <= 0)
        {
            LevelFailed?.Invoke();
        }
        else
        {
            StartCoroutine(SetUp(10));
        }
    }


    #region SetUp

    private IEnumerator SetUp(float timeIndex)
    {
        for (var i = 0; i < width; i++)
        {
            for (var j = 0; j < height; j++)
            {
                if (allCubes[i, j] == null)
                {
                    var temPos = matrixTransforms[i, j];
                    temPos.y += offset * j * 0.5f;

                    var cubeToUse = 0;
                    var duckOrBalloonChance = Random.Range(0, 100);

                    if (duckOrBalloonChance <= 10 && j > 0)
                        cubeToUse = 5;
                    else if (duckOrBalloonChance <= 20 && duckOrBalloonChance > 10)
                        cubeToUse = 6;
                    else
                        cubeToUse = Random.Range(0, cubes.Length - 4);

                    var cube = Instantiate(cubes[cubeToUse], temPos, Quaternion.identity);
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
    }

    private void CheckShuffling() // Shuffles if all the cubes are different from each other.
    {
        if (ShouldShuffle())
        {
            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    var cube = allCubes[i, j];
                    allCubes[i, j] = null;
                    Destroy(cube);
                }
            }

            TurnEnded?.Invoke();
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
                if (allCubes[i, j] != null && allCubes[i, j].TryGetComponent(out Cube cube))
                {
                    if (!cube.isBalloon && !cube.isDuck)
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
            }
        }

        CheckDucks();
    }

    private void CheckDucks()
    {
        var duckDestroyed = false;
        for (var column = 0; column < width; column++)
        {
            if (allCubes[column, 0].TryGetComponent(out Cube cube))
                if (cube.isDuck)
                {
                    duckDestroyed = true;
                    cube.DestroyDuck();
                }
        }
        if (duckDestroyed)
            DecreaseRow();
        else
        {
            CheckShuffling();
            isPlayable = true;
        }
    }

    #endregion

    #region Destroying Cubes

    private void DestroyCubesAt(int column, int row) // Destroys the selected cube.
    {
        if (allCubes[column, row].TryGetComponent(out Cube cube))
        {
            cube.isDestroyed = true;
            
            if (cube.isBalloon)
            {
                Pool.Instance.SpawnObject(cube.transform.position, "BalloonParticle", null, 1f);
                BalloonDestroyed?.Invoke();
            }
        }

        Destroy(allCubes[column, row]);
        allCubes[column, row] = null;
    }

    private IEnumerator DestructionCheck(int column, int row)
    {
        if (allCubes[column, row] != null && allCubes[column, row].TryGetComponent(out Cube cube))
        {

            if (allCubes[column, row].CompareTag(goalList[0].tag) && goalAmounts[0] > 0)
            {
                CheckForBalloon(column, row);
                cube.JumpToGoal(0);
                allCubes[column, row] = null;

                yield return new WaitForSeconds(0.1f);
            }
            else if (allCubes[column, row].CompareTag(goalList[1].tag) && goalAmounts[1] > 0)
            {
                CheckForBalloon(column, row);
                cube.JumpToGoal(1);
                allCubes[column, row] = null;

                yield return new WaitForSeconds(0.1f);
            }
            else if (rocketCenter != cube.transform)
            {
                if (rocketCenter != null)
                {
                    CubeDestroyed?.Invoke();
                    cube.JoinToTheRocket();
                    allCubes[column, row] = null;
                }
                else if (cube.isDuck)
                {
                    DestroyCubesAt(column, row);
                }
                else
                {
                    CheckForBalloon(column, row);
                    
                    if (!cube.isRocket && !cube.isBalloon)
                    {
                        var particleName = cube.tag + "Rocks";
                        Pool.Instance.SpawnObject(cube.transform.position, particleName, null, 1f);
                    }
                    
                    CubeDestroyed?.Invoke();
                    DestroyCubesAt(column, row);
                }
            }
            else
            {
                CheckForBalloon(column, row);
            }
        }
        
        _isOnce = false;
    }

    public IEnumerator DestroyCubes(int column, int row, GameObject group) // Destroy all the cubes in the selected cube.
    {
        for (var i = 0; i < width; i++)
        {
            for (var j = 0; j < height; j++)
            {
                if (allCubes[i, j] != null && allCubes[i, j] != allCubes[column, row] && allCubes[i, j].transform.parent == group.transform)
                {
                    yield return new WaitUntil(() => !_isOnce);
                    _isOnce = true;
                    StartCoroutine(DestructionCheck(i, j));
                }
            }
        }

        yield return new WaitForSeconds(0.01f);
        StartCoroutine(DestructionCheck(column, row));
        
        if (rocketCenter == null)
            Pool.Instance.DeactivateObject(group);
        else
        {
            rocketCenter = null;
            yield return new WaitForSeconds(0.5f);
        }

        DecreaseRow();
    }

    public IEnumerator LaunchRocket(int column, int row, bool isHorizontal) // Launch the rocket to destroy all cubes in the line.
    {
        if (isHorizontal)
        {
            for (var i = 1; i < width; i++)
            {
                if (column + i < width)
                {
                    yield return new WaitUntil(() => !_isOnce);
                    _isOnce = true;
                    StartCoroutine(DestructionCheck(column + i, row));
                }

                if (column - i >= 0)
                {
                    yield return new WaitUntil(() => !_isOnce);
                    _isOnce = true;
                    StartCoroutine(DestructionCheck(column - i, row));
                }
                
                yield return new WaitForSeconds(0.05f);
            }
        }
        else
        {
            for (var i = 1; i < height; i++)
            {
                if (row + i < height)
                {
                    yield return new WaitUntil(() => !_isOnce);
                    _isOnce = true;
                    StartCoroutine(DestructionCheck(column, row + i));
                }

                if (row - i >= 0)
                {
                    yield return new WaitUntil(() => !_isOnce);
                    _isOnce = true;
                    StartCoroutine(DestructionCheck(column, row - i));
                }
                
                yield return new WaitForSeconds(0.05f);
            }
        }
        
        StartCoroutine(DestructionCheck(column, row));
        DecreaseRow();
    }

    private void CheckForBalloon(int column, int row)
    {
        if (column + 1 < width && allCubes[column + 1, row] != null &&
            allCubes[column + 1, row].TryGetComponent(out Cube rightCube))
        {
            if (rightCube.isBalloon)
            {
                DestroyCubesAt(column + 1, row);
            }
        }

        if (column - 1 >= 0 && allCubes[column - 1, row] != null &&
            allCubes[column - 1, row].TryGetComponent(out Cube leftCube))
        {
            if (leftCube.isBalloon)
            {
                DestroyCubesAt(column - 1, row);
            }
        }

        if (row + 1 < height && allCubes[column, row + 1] != null &&
            allCubes[column, row + 1].TryGetComponent(out Cube upperCube))
        {
            if (upperCube.isBalloon)
            {
                DestroyCubesAt(column, row + 1);
            }
        }

        if (row - 1 >= 0 && allCubes[column, row - 1] != null &&
            allCubes[column, row - 1].TryGetComponent(out Cube downCube))
        {
            if (downCube.isBalloon)
            {
                DestroyCubesAt(column, row - 1);
            }
        }
    }

    public void DecreaseRow() // Fills the empty places.
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
    }

    private bool ShouldShuffle() // Checking if shuffling necessary.
    {
        var count = 0;
        for (var i = 0; i < width; i++)
        {
            for (var j = 0; j < height; j++)
            {
                if (allCubes[i, j] != null && allCubes[i, j].transform.parent.childCount > 1)
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