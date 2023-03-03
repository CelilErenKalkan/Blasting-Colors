using System.Collections;
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
            foreach (Transform child in transform)
            {
                if (child.TryGetComponent(out RocketLaunch rocket)) rocket.Launch(cubeType);
            }
        
            StartCoroutine(LaunchRocket());
        }
        
            public IEnumerator LaunchRocket() // Launch the rocket to destroy all cubes in the line.
            {
                if (cubeType == CubeType.HorizontalRocket)
                {
                    for (var i = 1; i < _gameManager.width; i++)
                    {
                        if (column + i < _gameManager.width)
                        {
                            yield return new WaitUntil(() => !_isOnce);
                            _isOnce = true;
                            _gameManager.allCubes[column + i, row].DestructionCheck();
                        }
        
                        if (column - i >= 0)
                        {
                            yield return new WaitUntil(() => !_isOnce);
                            _isOnce = true;
                            _gameManager.allCubes[column - i, row].DestructionCheck();
                        }
        
                        yield return new WaitForSeconds(0.05f);
                    }
                }
                else
                {
                    for (var i = 1; i < _gameManager.height; i++)
                    {
                        if (row + i < _gameManager.height)
                        {
                            yield return new WaitUntil(() => !_isOnce);
                            _isOnce = true;
                            _gameManager.allCubes[column, row + i].DestructionCheck();
                        }
        
                        if (row - i >= 0)
                        {
                            yield return new WaitUntil(() => !_isOnce);
                            _isOnce = true;
                            _gameManager.allCubes[column, row - i].DestructionCheck();
                        }
        
                        yield return new WaitForSeconds(0.05f);
                    }
                }
        
                DestructionCheck();
                _gameManager.DecreaseRow();
            }
    }
}
