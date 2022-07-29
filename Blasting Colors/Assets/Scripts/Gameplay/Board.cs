using UnityEngine;

public class Board : MonoBehaviour
{
    public int width;
    public int height;
    public float distance;
    private void Awake()
    {
        GameManager.Instance.width = width;
        GameManager.Instance.height = height;
        GameManager.Instance.matrixTransforms = new Vector2[width, height];

        var currentPos = transform.position;
        var x = currentPos.x;
        var z = currentPos.z;
        
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameManager.Instance.matrixTransforms[i, j] = new Vector2(x, z);
                x += distance;
            }

            x = currentPos.x;
            z += distance;
        }

        z = currentPos.x;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
