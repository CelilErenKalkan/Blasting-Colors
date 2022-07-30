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
        GameManager.Instance.offset = distance;
        GameManager.Instance.matrixTransforms = new Vector2[width, height];
        
        var amountX = width / -2;
        var amountY = height / -2;
        var x = amountX * distance;
        var y = amountY * distance;
        
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameManager.Instance.matrixTransforms[i, j] = new Vector2(x, y);
                y += distance;
            }

            y = amountY * distance;
            x += distance;
        }

        x = amountX * distance;
    }
}
