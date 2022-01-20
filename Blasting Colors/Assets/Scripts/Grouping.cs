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
        public string color;

        void Update()
        {
            if (transform.childCount == 0)
            {
                Destroy(gameObject, 0.15f);
            }
        }
        public void Add(string color)
        {
            this.color = color;
        }

        public void ChangeGroup(GameObject otherGroup)
        {
            foreach (Transform member in transform)
            {
                member.transform.parent = otherGroup.transform;
                member.GetComponent<Dot>().group = otherGroup;
            }
            if (transform.childCount == 0)
                Destroy(gameObject, 0.1f);
        }
    }
}
