using Management;
using UnityEngine;
using static Actions;

namespace Gameplay
{
    public class Balloon : Cube
    {
        protected override void OnMouseUp()
        {
            if (!_gameManager.isPlayable) return;
            
            _gameManager.isPlayable = false;
            if (TryGetComponent(out Animator animator)) animator.SetTrigger("isWrong");
            _gameManager.moves++;
            _gameManager.moves--;
        }

        public override void DestroyThis()
        {
            Pool.Instance.SpawnObject(transform.position, PoolItemType.BalloonPop, null, 1f);
            _gameManager.allCubes[column, row] = null;
            BalloonDestroyed?.Invoke();
            
            Destroy(gameObject);
        }
    }
}