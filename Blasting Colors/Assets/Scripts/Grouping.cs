using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Assets.Scripts
{
    public class Grouping : MonoBehaviour
    {
        public string color;

        public void Add(string color)
        {
            this.color = color;
        }

        public void ChangeGroup(GameObject otherGroup, GameObject member)
        {
            member.transform.parent = otherGroup.transform;
            if (transform.childCount == 0)
                Destroy(gameObject, 0.1f);
        }
    }
}
