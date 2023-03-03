using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay;
using UnityEngine;
using static Actions;
using Random = UnityEngine.Random;

public class GameManager : MonoSingleton<GameManager>
{
    [HideInInspector] public int width;
    [HideInInspector] public int height;
    [HideInInspector] public float offset;
    [HideInInspector] public bool isPlayable;
    [HideInInspector] public List<Cube> goalList = new List<Cube>();
    [HideInInspector] public Transform rocketCenter;

    public int moves = 30;
    public int duckSpawnChance;
    public int balloonSpawnChance;
    public List<int> goalAmounts;
    private GameObject groups;
    public Cube[,] allCubes;
    public Vector2[,] matrixTransforms;
    public GameObject[] cubes;

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
        allCubes = new Cube[width, height];
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

                    if (duckOrBalloonChance <= duckSpawnChance && j > 0)
                        cubeToUse = 5;
                    else if (duckOrBalloonChance <= duckSpawnChance + balloonSpawnChance &&
                             duckOrBalloonChance > duckSpawnChance)
                        cubeToUse = 6;
                    else
                        cubeToUse = Random.Range(0, cubes.Length - 4);

                    var cube = Instantiate(cubes[cubeToUse], temPos, Quaternion.identity);
                    if (cube.TryGetComponent(out Cube cubeScript))
                    {
                        allCubes[i, j] = cubeScript;
                        cubeScript.column = i;
                        cubeScript.row = j;
                    }

                    yield return new WaitForSeconds(0.005f * timeIndex);
                }
            }
        }


        CheckGroups();
    }

    private void CheckShuffling() // Shuffles if all the cubes are different from each other.
    {
        if (ShouldShuffle())
        {
            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    allCubes[i, j].DestroyThis();
                }
            }

            TurnEnded?.Invoke();
        }
    }

    private void CheckGroups() // Gather all adjacent dots with the same color under one group.
    {
        for (var i = 0; i < width; i++)
        {
            for (var j = 0; j < height; j++)
            {
                if (allCubes[i, j] == null) continue;
                switch (allCubes[i, j].cubeType)
                {
                    case CubeType.Balloon:
                    case CubeType.Duck:
                        continue;
                    default:
                        allCubes[i, j].CheckGroup();
                        break;
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
            if (allCubes[column, 0].gameObject != null)
            {
                if (allCubes[column, 0].cubeType == CubeType.Duck)
                {
                    duckDestroyed = true;
                    allCubes[column, 0].DestroyThis();
                }
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
                    if (allCubes[i, j])
                    {
                        allCubes[i, j].row -= nullCount;
                        allCubes[i, allCubes[i, j].row] = allCubes[i, j];
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
                if (allCubes[i, j] != null && allCubes[i, j].HasGroup())
                    count++;
            }
        }

        return count <= 1;
    }

    #endregion
}