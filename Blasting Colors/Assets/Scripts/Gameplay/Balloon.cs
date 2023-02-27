using UnityEngine;
using static Actions;

namespace Gameplay
{
    public class Balloon : Cube
    {
        protected override void OnMouseUp()
        {
            if (_manager.isPlayable)
            {
                _manager.isPlayable = false;
                if (TryGetComponent(out Animator animator)) animator.SetTrigger("isWrong");
                _manager.moves++;
                _manager.moves--;
            }
        }

        protected override void DestroyThisCube()
        {
            Pool.Instance.SpawnObject(transform.position, PoolItemType.BalloonPopExplosion, null, 1f);
            _manager.allCubes[column, row] = null;
            BalloonDestroyed?.Invoke();
            
            Destroy(gameObject);
        }
    }
}