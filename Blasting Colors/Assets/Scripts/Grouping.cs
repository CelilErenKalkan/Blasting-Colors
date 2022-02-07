using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Assets.Scripts
{
    public class Grouping : MonoBehaviour
    {
        void Update()
        {
            if (transform.childCount == 0)
            {
                Board.singleton.DeactivateEmptyGroups(this.gameObject);
            }
        }

        public void ChangeGroup(GameObject otherGroup) // Changes all members group into the given group.
        {
            if (otherGroup != this.gameObject)
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
            member.GetComponent<Dot>().group = otherGroup;
        }
    }
}
