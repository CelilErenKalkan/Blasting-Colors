using UnityEngine;

namespace Gameplay
{
    public class Rocket : Cube
    {
        protected override void OnMouseUp()
        {
            LaunchTheRocket();
        }
        
        protected virtual void LaunchTheRocket()
        {
            foreach (Transform child in transform.GetChild(0))
            {
                if (child.TryGetComponent(out RocketLaunch rocket)) rocket.Launch(cubeType);
            }
        
            StartCoroutine(_gameManager.LaunchRocket(column, row, cubeType));
        }
    }
}
