using UnityEngine;

public class Dot : MonoBehaviour
{
    public int column;
    public int row;
    private int speed = 5;
    [HideInInspector] public int targetX;
    [HideInInspector] public int targetY;
    private GameManager _gameManager;
    private Vector2 tempPos;
    public GameObject group;
    private SpriteRenderer spriteRenderer;


    // Start is called before the first frame update
    void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        targetX = (int)transform.position.x;
        targetY = (int)transform.position.y;
        row = targetY - _gameManager.GetComponent<GameManager>().offset;
        column = targetX;
    }

    // Update is called once per frame
    void Update()
    {
        targetX = column;
        targetY = row;
        if (Mathf.Abs(targetX - transform.position.x) > .1)
        {
            //Move Towards Target.
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPos, Time.deltaTime * speed);
            if (_gameManager.allDots[column, row] != this.gameObject)
            {
                _gameManager.allDots[column, row] = this.gameObject;
            }
        }
        else
        {
            //Directly Set the Position.
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position = tempPos;
            _gameManager.allDots[column, row] = this.gameObject;
        }

        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            //Move Towards Target.
            tempPos = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPos, Time.deltaTime * speed);
            if (_gameManager.allDots[column, row] != this.gameObject)
            {
                _gameManager.allDots[column, row] = this.gameObject;
            }
        }
        else
        {
            //Directly Set the Position.
            tempPos = new Vector2(transform.position.x, targetY);
            transform.position = tempPos;
            _gameManager.allDots[column, row] = this.gameObject;
        }
    }

    private void OnMouseUp()
    {
        if (GameManager.Instance.isPlayable)
        {
            GameManager.Instance.isPlayable = false;
            if (transform.parent.childCount > 1)
                _gameManager.GetComponent<GameManager>().DestroyDots(group);
            else
            {
                Debug.Log("You cannot destroy only one dot.");
                GameManager.Instance.isPlayable = true;
            }
        }
    }

    public void CreateGroup()
    {
        group = Pool.Instance.SpawnObject(transform.position,"Group", null);
        if (group != null)
        {
            group.SetActive(true);
            transform.SetParent(group.transform);
        }
    }

}
