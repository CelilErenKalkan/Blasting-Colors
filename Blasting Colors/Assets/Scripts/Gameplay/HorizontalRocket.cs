using System.Collections;
using UnityEngine;

namespace Gameplay
{
    public class HorizontalRocket : Rocket
    {
        public override IEnumerator LaunchRocket() // Launch the rocket to destroy all cubes in the line.
        {
            for (var i = 1; i < _gameManager.width; i++)
            {
                if (column + i < _gameManager.width)
                {
                    if (_gameManager.allCubes[column + i, row] != null)
                        _gameManager.allCubes[column + i, row].DestructionCheck();
                }

                if (column - i >= 0)
                {
                    if (_gameManager.allCubes[column - i, row] != null)
                        _gameManager.allCubes[column - i, row].DestructionCheck();
                }

                yield return new WaitForSeconds(0.05f);
            }

            DestructionCheck();
            _gameManager.DecreaseRow();
        }
    }
}