using System;
using UnityEngine;
using static Actions;

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
        
        var amountX = (float)(width - 1) / -2;
        var amountY = (float)(height - 1) / -2;
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

    private void OnEnable()
    {
        LevelStart += OnGameStarted;
    }
    
    private void OnDisable()
    {
        LevelStart -= OnGameStarted;
    }

    private void OnGameStarted()
    {
        if (TryGetComponent(out SpriteRenderer spriteRenderer))
        {
            var borderWidth = width + 0.2f;
            var borderHeight = this.height + 0.4f;
            spriteRenderer.size = new Vector2(borderWidth, borderHeight);
            spriteRenderer.enabled = true;
        }
    }
}
