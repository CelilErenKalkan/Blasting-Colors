using DG.Tweening;
using UnityEngine;

namespace Gameplay
{
    public class RocketLaunch : MonoBehaviour
    {
        [SerializeField]private bool isRight;

        public void Launch(CubeType cubeType)
        {
            var targetTime = 0f;
            var targetDistance = 0f;
            var target = transform.position;
        
            if (cubeType == CubeType.HorizontalRocket)
            {
                targetTime = GameManager.Instance.width * 0.24f;
                targetDistance = GameManager.Instance.width * GameManager.Instance.offset * 2;

                if (isRight)
                    target.x += targetDistance;
                else
                    target.x -= targetDistance;
                transform.DOMove(target, targetTime);
            }
            else
            {
                targetTime = GameManager.Instance.height * 0.24f;
                targetDistance = GameManager.Instance.height * GameManager.Instance.offset * 2;
            
                if (isRight)
                    target.y += targetDistance;
                else
                    target.y -= targetDistance;
                transform.DOMove(target, targetTime);
            }
        }
    }
}
