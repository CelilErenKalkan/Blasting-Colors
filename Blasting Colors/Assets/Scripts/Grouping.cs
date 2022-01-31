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

        public void ChangeGroup(GameObject otherGroup)
        {
            foreach (Transform member in transform)
            {
                member.transform.parent = otherGroup.transform;
                member.GetComponent<Dot>().group = otherGroup;
            }
        }
    }
}
