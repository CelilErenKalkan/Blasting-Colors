using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Management;
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
        [HideInInspector] public int column;
        [HideInInspector] public int row;
        protected Vector2 tempPos;
        protected SpriteRenderer spriteRenderer;
        protected int goalNo;
        protected GameManager _gameManager;
        public List<Cube> groupList = new List<Cube>();

        [HideInInspector] public bool isDestroyed;
        protected bool _isOnce, _isClicked;
        public CubeType cubeType;

        #region GROUP

        public bool HasGroup() => groupList.Count > 0;

        public bool CanTurnIntoRocket() => groupList.Count >= 5;

        public bool IsGoal()
        {
            if (cubeType == _gameManager.goalList[0].cubeType && _gameManager.goalAmounts[0] > 0)
            {
                goalNo = 0;
                return true;
            }

            if (cubeType == _gameManager.goalList[1].cubeType && _gameManager.goalAmounts[1] > 0)
            {
                goalNo = 1;
                return true;
            }

            return false;
        }

        public void ChangeGroup(List<Cube> newList, Cube invoter)
        {
            for (var i = 0; i < newList.Count; i++)
            {
                if (!groupList.Contains(newList[i]))
                    groupList.Add(newList[i]);
            }

            groupList.Add(invoter);
            groupList.Remove(this);
        }

        public void CheckGroup()
        {
            //Checking Left.
            if (column > 0)
            {
                if (_gameManager.allCubes[column - 1, row] != null)
                {
                    if (cubeType == _gameManager.allCubes[column - 1, row].cubeType)
                    {
                        ChangeGroup(_gameManager.allCubes[column - 1, row].groupList, this);
                        groupList.Add(_gameManager.allCubes[column - 1, row]);
                    }
                }
            }

            //Checking Down.
            if (row > 0)
            {
                if (_gameManager.allCubes[column, row - 1] != null)
                {
                    if (cubeType == _gameManager.allCubes[column, row - 1].cubeType)
                    {
                        ChangeGroup(_gameManager.allCubes[column, row - 1].groupList, this);
                        groupList.Add(_gameManager.allCubes[column, row - 1]);
                    }
                }
            }

            for (var i = 0; i < groupList.Count; i++)
            {
                groupList[i].ChangeGroup(groupList, this);
            }

            groupList.Remove(this);
        }

        public void ChangeSortingOrder(int order)
        {
            spriteRenderer.sortingOrder = order;
        }

        #endregion

        #region MOVE

        public virtual void DestroyThis()
        {
            if (_gameManager.goalList[goalNo].cubeType == cubeType)
            {
                Pool.Instance.SpawnObject(transform.position, PoolItemType.Star, null, 1f);
                _gameManager.goalAmounts[goalNo]--;
                GoalAmountChanged?.Invoke();
            }
            isDestroyed = true;

            Destroy(gameObject);
        }

        public virtual void JumpToGoal()
        {
            ChangeSortingOrder(21);
            isDestroyed = true;

            var goalPosition = _gameManager.goalList[goalNo].transform.position;
            transform.DOJump(goalPosition, -5, 1, 1.0f).OnComplete(DestroyThis);
        }

        # region ROCKET

        protected virtual Rocket TurnIntoARocket()
        {
            var randomRocket = Random.Range(1, 3);
            var rocket = Instantiate(_gameManager.cubes[_gameManager.cubes.Length - randomRocket]);
            rocket.transform.position = transform.position;
            if (rocket.TryGetComponent(out Rocket rocketScript))
            {
                _gameManager.allCubes[column, row] = rocketScript;
                rocketScript.column = column;
                rocketScript.row = row;
            }

            var order = spriteRenderer.sortingOrder;
            foreach (Transform child in rocket.transform)
            {
                if (child.TryGetComponent(out SpriteRenderer renderer)) renderer.sortingOrder = order;
            }

            return rocketScript;
        }

        public virtual void JoinToTheRocket(Vector3 targetPosition)
        {
            CubeDestroyed?.Invoke();
            _gameManager.allCubes[column, row] = null;
            ChangeSortingOrder(21);
            isDestroyed = true;
            transform.DOMove(targetPosition, 0.5f).SetEase(Ease.InBack).OnComplete(DestroyThis);
        }

        protected virtual IEnumerator SetRocket()
        {
            var rocket = TurnIntoARocket();

            for (var i = 0; i < groupList.Count; i++)
            {
                if (groupList[i] != this)
                {
                    groupList[i].JoinToTheRocket(rocket.transform.position);
                }
            }
            
            yield return new WaitForSeconds(0.05f);
            var targetScale = new Vector3(0.1f, 0.1f, 0.1f);
            transform.DOScale(targetScale, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
            {
                if (_isClicked)
                    _gameManager.DecreaseRow();
                
                rocket.ActivateRocket();
                DestroyThis();
            });
        }

        #endregion

        public void CheckForBalloon()
        {
            if (column + 1 < _gameManager.width && _gameManager.allCubes[column + 1, row] != null &&
                _gameManager.allCubes[column + 1, row].TryGetComponent(out Balloon rightBalloon))
            {
                rightBalloon.DestroyThis();
            }

            if (column - 1 >= 0 && _gameManager.allCubes[column - 1, row] != null &&
                _gameManager.allCubes[column - 1, row].TryGetComponent(out Balloon leftBalloon))
            {
                leftBalloon.DestroyThis();
            }

            if (row + 1 < _gameManager.height && _gameManager.allCubes[column, row + 1] != null &&
                _gameManager.allCubes[column, row + 1].TryGetComponent(out Balloon upperBalloon))
            {
                upperBalloon.DestroyThis();
            }

            if (row - 1 >= 0 && _gameManager.allCubes[column, row - 1] != null &&
                _gameManager.allCubes[column, row - 1].TryGetComponent(out Balloon downBalloon))
            {
                downBalloon.DestroyThis();
            }
        }

        public void DestructionCheck()
        {
            if (isDestroyed) return;
            
            _gameManager.allCubes[column, row] = null;
            CheckForBalloon();
            
            if (IsGoal())
            {
                JumpToGoal();
                
                if (_isClicked)
                    _gameManager.DecreaseRow();
            }
            else
            {
                if (cubeType != CubeType.HorizontalRocket && cubeType != CubeType.VerticalRocket &&
                    cubeType != CubeType.Balloon)
                {
                    //var particleName = tag + "Rocks";
                    //Pool.Instance.SpawnObject(cube.transform.position, particleName, null, 1f);
                }

                CubeDestroyed?.Invoke();
                DestroyThis();
                
                if (_isClicked)
                    _gameManager.DecreaseRow();
            }
        }

        protected virtual IEnumerator DestroyGroup() // Destroy all the cubes in the selected cube.
        {
            for (var i = 0; i < groupList.Count; i++)
            {
                if (groupList[i] != this)
                {
                    groupList[i].DestructionCheck();
                }
            }


            yield return new WaitForSeconds(0.05f);
            DestructionCheck();
        }

        #endregion

        public void SetIsPlayable()
        {
            _gameManager.isPlayable = true;
        }

        protected virtual void OnMouseUp()
        {
            if (!_gameManager.isPlayable) return;

            _gameManager.isPlayable = false;
            _isClicked = true;

            if (groupList.Count <= 0)
            {
                if (TryGetComponent(out Animator animator)) animator.SetTrigger("isWrong");
                _gameManager.moves++;
                _isClicked = false;
            }
            else
            {
                if (CanTurnIntoRocket() && !IsGoal())
                {
                    StartCoroutine(SetRocket());
                }
                else
                {
                    StartCoroutine(DestroyGroup());
                }
            }

            _gameManager.moves--;
        }

        protected virtual void OnTurnEnded()
        {
            if (isDestroyed) return;
            groupList.Clear();
            tempPos = _gameManager.matrixTransforms[column, row];
            transform.DOMove(tempPos, 0.5f).SetEase(Ease.OutBounce).OnComplete(SetCube);
            ChangeSortingOrder(row);
        }

        protected virtual void SetCube()
        {
            _gameManager.allCubes[column, row] = this;
        }

        private void Start()
        {
            _gameManager = GameManager.Instance;
            if (TryGetComponent(out SpriteRenderer renderer)) spriteRenderer = renderer;
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
    }
}