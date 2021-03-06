using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    [System.Serializable]
    public class PoolGroup
    {
        public GameObject prefab;
        public int amount;
        public bool expandable;
    }

    public class Board : MonoBehaviour
    {

        public int width;
        public int height;
        public int offset;
        public GameObject tilePrefab;
        private BackgroundTile[,] allTiles;
        public GameObject[] dots;
        public GameObject[,] allDots;
        public bool isPlayable;

        public static Board singleton;
        public List<PoolGroup> items;
        public List<GameObject> pooledItems;

        void Awake()
        {
            singleton = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            pooledItems = new List<GameObject>();
            foreach (var item in items)
            {
                for (int i = 0; i < item.amount; i++)
                {
                    GameObject obj = Instantiate(item.prefab);
                    obj.SetActive(false);
                    pooledItems.Add(obj);
                }
            }

            allTiles = new BackgroundTile[width, height];
            allDots = new GameObject[width, height];
            StartCoroutine(SetUp());
        }

        private IEnumerator SetUp()
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
                        //dot.name = "( " + i + ", " + j + " )";
                        allDots[i, j] = dot;
                    }
                }
            }

            yield return new WaitForSeconds(0.1f);
            Grouping();
            yield return new WaitForSeconds(0.1f);
            CheckGroups();
            yield return new WaitForSeconds(0.1f);
            CheckIcon();
            yield return new WaitForSeconds(0.1f);
            CheckShuffling();

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
        }

        private void RemoveGroups(int column, int row)
        {
            allDots[column, row].transform.parent = null;
            if (allDots[column, row].GetComponent<Dot>().group != null)
            {
                allDots[column, row].GetComponent<Dot>().group.SetActive(false);
            }

            allDots[column, row].GetComponent<Dot>().group = null;
            allDots[column, row].GetComponent<Dot>().CreateGroup();
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
                            if (allDots[i - 1, j].tag == allDots[i, j].tag)
                            {
                                allDots[i - 1,j].transform.parent.GetComponent<Grouping>().ChangeGroup(allDots[i, j].transform.parent.gameObject);
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
                                allDots[i, j - 1].transform.parent.GetComponent<Grouping>().ChangeGroup(allDots[i, j].transform.parent.gameObject);
                            }
                        }
                    }
                }
            }
        }

        private void CheckIcon() // Checks for icon change.
        {
            foreach (var dot in allDots)
            {
                dot.GetComponent<Dot>().CheckIcon();
            }
        }

        private void DestroyDotsAt(int column, int row) // Destroys the selected dot.
        {
            Destroy(allDots[column, row]);
            allDots[column, row] = null;
        }

        public void DestroyDots(GameObject group) // Destroy all the dots in the selected dot.
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

            group.SetActive(false);
            StartCoroutine(DecreaseRow());
        }

        private IEnumerator DecreaseRow() // Fills the empty places.
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
            StartCoroutine(SetUp());
            yield return new WaitForSeconds(.1f);
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

            if (count > 1)
                return false;
            else
                return true;
        }

        public GameObject Get(string tag) // Gets a group object from the pool.
        {
            for (int i = 0; i < pooledItems.Count; i++)
            {
                if (!pooledItems[i].activeInHierarchy && pooledItems[i].CompareTag(tag))
                {
                    return pooledItems[i];
                }
            }

            foreach (var item in items)
            {
                if (item.prefab.CompareTag(tag) && item.expandable)
                {
                    GameObject newItem = Instantiate(item.prefab);
                    pooledItems.Add(newItem);
                    return newItem;
                }
            }

            return null;
        }

        public void DeactivateEmptyGroups(GameObject deactive) // Deactivates if the group object is empty.
        {
            deactive.gameObject.SetActive(false);
        }

        #endregion
    }
}
