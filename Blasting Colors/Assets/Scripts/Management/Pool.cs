using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class PoolItem
{
    public List<GameObject> prefabs = new List<GameObject>();
    public int amount;
    public bool expandable;
}

public class Pool : MonoSingleton<Pool>
{
    public List<PoolItem> items;
    [HideInInspector] public List<GameObject> poolObjects;

    private GameObject GetFromPool(string theTag)
    {
        foreach (var item in items)
        {
            if (item.prefabs[0].CompareTag(theTag) && item.expandable)
            {
                var prefabName = item.prefabs[0].tag + "Pool";
                foreach (var pool in poolObjects)
                {
                    if (prefabName == pool.name)
                    {
                        foreach (Transform child in pool.transform)
                        {
                            if (!child.gameObject.activeInHierarchy)
                            {
                                return child.gameObject;
                            }
                        }
                    }
                }
            }
        }

        foreach (var item in items)
        {
            if (item.prefabs[0].CompareTag(theTag) && item.expandable)
            {
                var prefabName = item.prefabs[0].tag + "Pool";
                foreach (var pool in poolObjects)
                {
                    if (prefabName == pool.name)
                    {
                        var randomObjectNo = Random.Range(0, item.prefabs.Count);
                        var newItem = Instantiate(item.prefabs[randomObjectNo], pool.transform);
                        newItem.name = item.prefabs[randomObjectNo].name;
                        return newItem;
                    }
                }
            }
        }

        return null;
    }

    private GameObject GetFromPool(string theTag, int childIndex)
    {
        foreach (var item in items)
        {
            if (item.prefabs[0].CompareTag(theTag) && item.expandable)
            {
                var prefabName = item.prefabs[0].tag + "Pool";
                foreach (var pool in poolObjects)
                {
                    if (prefabName == pool.name)
                    {
                        foreach (Transform child in pool.transform)
                        {
                            if (!child.gameObject.activeInHierarchy && item.prefabs[childIndex].name == child.name)
                            {
                                return child.gameObject;
                            }
                        }
                    }
                }
            }
        }

        foreach (var item in items)
        {
            if (item.prefabs[0].CompareTag(theTag) && item.expandable)
            {
                var prefabName = item.prefabs[0].tag + "Pool";
                foreach (var pool in poolObjects)
                {
                    if (prefabName == pool.name)
                    {
                        var newItem = Instantiate(item.prefabs[childIndex], pool.transform);
                        newItem.name = item.prefabs[childIndex].name;
                        return newItem;
                    }
                }
            }
        }

        return null;
    }

    public GameObject SpawnObject(Vector3 position, string theTag, Transform parent)
    {
        var b = GetFromPool(theTag);
        if (b != null)
        {
            if (parent != null) b.transform.SetParent(parent);
            if (position != null) b.transform.position = position;
            b.SetActive(true);
        }

        return b;
    }

    public GameObject SpawnObject(Vector3 position, string theTag, Transform parent, int childIndex)
    {
        var b = GetFromPool(theTag, childIndex);
        if (b != null)
        {
            if (parent != null) b.transform.SetParent(parent);
            if (position != null) b.transform.position = position;
            b.SetActive(true);
        }

        return b;
    }
    
    public GameObject SpawnObject(Vector3 position, string theTag, Transform parent, float time)
    {
        var b = GetFromPool(theTag);
        if (b != null)
        {
            if (parent != null) b.transform.SetParent(parent);
            if (position != null) b.transform.position = position;
            b.SetActive(true);
        }
        
        StartCoroutine(Timer());
        
        IEnumerator Timer()
        {
            yield return new WaitForSeconds(time);
            DeactivateObject(b);
        }
        
        return b;
    }

    public void DeactivateObject(GameObject member)
    {
        var memberName = member.name + "Pool";
        foreach (var pool in poolObjects)
        {
            if (memberName == pool.name)
            {
                member.transform.SetParent(pool.transform);
                member.transform.position = pool.transform.position;
                member.transform.rotation = pool.transform.rotation;
                member.SetActive(false);
            }
        }
    }

    private void RandomizeSiblings(Transform pool)
    {
        for (var i = 0; i < pool.childCount; i++)
        {
            var random = Random.Range(i, pool.childCount);
            pool.GetChild(random).SetSiblingIndex(i);
        }
    }

    private void Awake()
    {
        var count = items.Count;
        for (var i = 0; i < count; i++)
        {
            var go = new GameObject
            {
                name = items[i].prefabs[0].tag + "Pool",
                transform =
                {
                    position = Vector3.zero
                }
            };

            poolObjects.Add(go);
        }

        foreach (var item in items)
        {
            var prefabName = item.prefabs[0].tag + "Pool";
            foreach (var pool in poolObjects)
            {
                if (prefabName == pool.name)
                {
                    var amount = item.amount / item.prefabs.Count;

                    if (item.amount % item.prefabs.Count != 0)
                    {
                        var extension = item.amount - amount * item.prefabs.Count;
                        for (var i = 0; i < extension; i++)
                        {
                            var random = Random.Range(0, item.prefabs.Count);
                            var obj = Instantiate(item.prefabs[random], pool.transform, true);
                            obj.name = item.prefabs[random].name;
                            obj.SetActive(false);
                        }
                    }

                    foreach (var itemObject in item.prefabs)
                    {
                        for (var i = 0; i < amount; i++)
                        {
                            var obj = Instantiate(itemObject, pool.transform, true);
                            obj.name = itemObject.name;
                            obj.SetActive(false);
                        }
                    }

                    RandomizeSiblings(pool.transform);
                }
            }
        }
    }
}