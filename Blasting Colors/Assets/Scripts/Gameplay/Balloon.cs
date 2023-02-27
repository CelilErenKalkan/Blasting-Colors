using UnityEngine;
using static Actions;

namespace Gameplay
{
    public class Balloon : Cube
    {
        protected override void OnMouseUp()
        {
            if (_gameManager.isPlayable)
            {
                _gameManager.isPlayable = false;
                if (TryGetComponent(out Animator animator)) animator.SetTrigger("isWrong");
                _gameManager.moves++;
                _gameManager.moves--;
            }
        }

        protected override void DestroyThisCube()
        {
            Pool.Instance.SpawnObject(transform.position, PoolItemType.BalloonPopExplosion, null, 1f);
            _gameManager.allCubes[column, row] = null;
            BalloonDestroyed?.Invoke();
            
            Destroy(gameObject);
        }

        public new void Destroy()
        {
            Pool.Instance.SpawnObject(transform.position, PoolItemType.BalloonPopExplosion, null, 1f);
            BalloonDestroyed?.Invoke();
        }
    }
}