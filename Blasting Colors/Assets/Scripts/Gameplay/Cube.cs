using DG.Tweening;
using UnityEngine;
using static Actions;
using Random = UnityEngine.Random;

namespace Gameplay
{
    public enum CubeType
    {
        Blue,
        Green,
        Purple,
        Red,
        Yellow,
        Balloon,
        Duck,
        HorizontalRocket,
        VerticalRocket
    }
    
    public class Cube : MonoBehaviour
    {
        [HideInInspector]public int column;
        [HideInInspector]public int row;
        [HideInInspector]public GameObject group;
        protected Vector2 tempPos;
        protected SpriteRenderer spriteRenderer;
        protected int goalNo;
        protected GameManager _manager;

        [HideInInspector]public bool isDestroyed;
        public CubeType cubeType;

        private void Start()
        {
            _manager = GameManager.Instance;
            OnTurnEnded();
        }

        protected virtual void OnEnable()
        {
            TurnEnded += OnTurnEnded;
        }
    
        protected virtual void OnDisable()
        {
            TurnEnded -= OnTurnEnded;
        }

        protected virtual void OnTurnEnded()
        {
            if (isDestroyed) return;
            tempPos = _manager.matrixTransforms[column, row];
            transform.DOMove(tempPos, 0.5f).SetEase(Ease.OutBounce).OnComplete(SetCube);
            if (TryGetComponent(out SpriteRenderer renderer)) renderer.sortingOrder = row;
        }

        protected virtual void SetCube()
        {
            transform.position = tempPos;
            _manager.allCubes[column, row] = gameObject;
        }

        protected virtual void OnMouseUp()
        {
            if (_manager.isPlayable)
            {
                _manager.isPlayable = false; 
                if (transform.parent.childCount <= 1)
                {
                    if (TryGetComponent(out Animator animator)) animator.SetTrigger("isWrong");
                    _manager.moves++;
                }
                else
                {
                    var isGoal = false;
                    foreach (var goal in _manager.goalList)
                    {
                        if (transform.CompareTag(goal.tag))
                        {
                            isGoal = true;
                            break;
                        }
                    }

                    if (group.transform.childCount >= 5 && !isGoal)
                    {
                        _manager.rocketCenter = transform;
                        var targetScale = new Vector3(0.1f, 0.1f, 0.1f);
                        transform.DOScale(targetScale, 0.4f).SetEase(Ease.InBack).OnComplete(SetRocket);
                    }
                
                    StartCoroutine(_manager.DestroyCubes(column, row, group));
                }
            
                _manager.moves--;
            }
        }

        public void CreateGroup()
        {
            group = Pool.Instance.SpawnObject(Vector3.zero, PoolItemType.Group, null);
            if (group != null)
            {
                group.SetActive(true);
                transform.SetParent(group.transform);
            }
        }

        public void JumpToGoal(int goalNo)
        {
            this.goalNo = goalNo;
            if (TryGetComponent(out SpriteRenderer spriteRenderer))
                spriteRenderer.sortingOrder = 21;
            transform.SetParent(null);
            isDestroyed = true;

            var goalPosition = _manager.goalList[goalNo].transform.position;
            transform.DOJump(goalPosition, -5, 1, 1).OnComplete(DestroyThisCube);
        }

        public void JoinToTheRocket()
        {
            if (TryGetComponent(out SpriteRenderer spriteRenderer))
                spriteRenderer.sortingOrder = 21;
            transform.SetParent(null);
            isDestroyed = true;

            var targetPosition = _manager.rocketCenter.position;
            transform.DOMove(targetPosition, 0.5f).SetEase(Ease.InBack).OnComplete(DestroyThisCube);
        }

        protected virtual void DestroyThisCube()
        {
            if (gameObject.CompareTag(_manager.goalList[goalNo].tag))
            {
                Pool.Instance.SpawnObject(transform.position, PoolItemType.SmokeExplosionWhite, null, 1f);
                _manager.goalAmounts[goalNo]--;
                GoalAmountChanged?.Invoke();
            }

            Destroy(gameObject);
        }

        public void DestroyDuck()
        {
            if (_manager.allCubes[column, row] != null)
            {
                Pool.Instance.SpawnObject(transform.position, PoolItemType.BalloonPopExplosion, null, 1f);
                _manager.allCubes[column, row] = null;
                DuckDestroyed?.Invoke();
                if (TryGetComponent(out SpriteRenderer renderer)) renderer.enabled = false;
                Destroy(gameObject);
            }
        }
    
        protected virtual void SetRocket()
        {
            var randomRocket = Random.Range(1, 3);
            if (randomRocket > 1)
                isHorizontal = true;
            Instantiate(_manager.cubes[_manager.cubes.Length - randomRocket], transform, false);
            isRocket = true;
            gameObject.tag = "Rocket";
            if (TryGetComponent(out SpriteRenderer spriteRenderer))
            {
                var order = spriteRenderer.sortingOrder;
                spriteRenderer.enabled = false;
                foreach (Transform child in transform.GetChild(0))
                {
                    if (child.TryGetComponent(out SpriteRenderer renderer)) renderer.sortingOrder = order;
                }
            }
        }

        public void SetIsPlayable()
        {
            _manager.isPlayable = true;
        }

    }
}
