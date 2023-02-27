using UnityEngine;

namespace Gameplay
{
    public class Duck : Cube
    {
        protected override void SetCube()
        {
            base.SetCube();
            if (row != 0) return;
            
            isDestroyed = true;
            _manager.isPlayable = false;
        }
        
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
    }
}
