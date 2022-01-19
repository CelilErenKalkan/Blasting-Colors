using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int width;
    public int height;
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
        CheckGroups();
    }

    private void SetUp()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 temPos = new Vector2(i, j);
                GameObject backgroundTile = Instantiate(tilePrefab, temPos, Quaternion.identity) as GameObject;
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "( Tile " + i + ", " + j + " )";
                int dotToUse = Random.Range(0, dots.Length);
                GameObject dot = Instantiate(dots[dotToUse], temPos, Quaternion.identity);
                dot.transform.parent = this.transform;
                dot.name = "( " + i + ", " + j + " )";
                allDots[i, j] = dot;
                dot.GetComponent<Dot>().CreateGroup();
            }
        }
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
                    if (allDots[i, j].tag == allDots[i + 1, j].tag)
                    {
                        allDots[i + 1, j].transform.parent.gameObject.GetComponent<Grouping>().ChangeGroup(allDots[i,j].transform.parent.gameObject, allDots[i + 1, j]);
                    }
                }

                //Checking Left.
                if (j < height - 1)
                {
                    if (allDots[i, j].tag == allDots[i, j + 1].tag)
                    {
                        allDots[i, j + 1].transform.parent.gameObject.GetComponent<Grouping>().ChangeGroup(allDots[i, j].transform.parent.gameObject, allDots[i, j + 1]);
                    }
                }
            }
        }
    }
}
