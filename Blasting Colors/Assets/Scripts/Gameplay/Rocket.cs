using System.Collections;
using UnityEngine;

namespace Gameplay
{
    public class Rocket : Cube
    {
        public virtual void ActivateRocket()
        {
            for (var i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).TryGetComponent(out SpriteRenderer spriteRenderer))
                    spriteRenderer.enabled = true;
            }
        }

        protected virtual void LaunchTheRocket()
        {
            foreach (Transform child in transform)
            {
                if (child.TryGetComponent(out RocketLaunch rocket)) rocket.Launch(cubeType);
            }

            StartCoroutine(LaunchRocket());
        }

        public virtual IEnumerator LaunchRocket() // Launch the rocket to destroy all cubes in the line.
        {
            yield return null;
            DestructionCheck();
            _gameManager.DecreaseRow();
        }

        protected override void OnMouseUp()
        {
            LaunchTheRocket();
        }
    }
}