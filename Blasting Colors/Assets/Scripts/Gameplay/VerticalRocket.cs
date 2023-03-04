using System.Collections;
using UnityEngine;

namespace Gameplay
{
    public class VerticalRocket : Rocket
    {
        public override IEnumerator LaunchRocket() // Launch the rocket to destroy all cubes in the line.
        {
            for (var i = 1; i < _gameManager.height; i++)
            {
                if (row + i < _gameManager.height)
                {
                    if (_gameManager.allCubes[column, row + i] != null)
                        _gameManager.allCubes[column, row + i].DestructionCheck();
                }

                if (row - i >= 0)
                {
                    if (_gameManager.allCubes[column, row - i] != null)
                        _gameManager.allCubes[column, row - i].DestructionCheck();
                }

                yield return new WaitForSeconds(0.05f);
            }

            DestructionCheck();
            _gameManager.DecreaseRow();
        }
    }
}