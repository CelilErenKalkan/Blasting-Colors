using UnityEngine;

public class BackgroundTile : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        if (TryGetComponent(out SpriteRenderer spriteRenderer))
        {
            var width = GameManager.Instance.width + 0.2f;
            var height = GameManager.Instance.height + 0.4f;
            spriteRenderer.size = new Vector2(width, height);
        }
    }
}
