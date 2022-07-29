using System.Collections.Generic;
using UnityEngine;

public class Grouping : MonoBehaviour
{
    void Update()
    {
        if (transform.childCount == 0)
        {
            GameManager.Instance.DeactivateEmptyGroups(this.gameObject);
        }
    }

    public void ChangeGroup(GameObject otherGroup) // Changes all members group into the given group.
    {
        if (otherGroup != gameObject)
        {
            List<Transform> listOfMembers = new List<Transform>();

            foreach (Transform member in transform)
            {
                listOfMembers.Add(member);
            }

            for (int i = 0; i < listOfMembers.Count; i++)
            {
                DeleteGroup(listOfMembers[i].gameObject, otherGroup);
            }
        }
    }

    private void DeleteGroup(GameObject member, GameObject otherGroup)
    {
        member.transform.parent = otherGroup.transform;
        if (member.TryGetComponent(out Dot dot)) dot.group = otherGroup;
    }
}