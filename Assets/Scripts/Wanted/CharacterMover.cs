using UnityEngine;

public class CharacterMover : MonoBehaviour
{
    public float speed = 100f;
    private RectTransform rectTransform;
    private Vector2 direction;
    private RectTransform bounds;

    private bool isMoving = false;

    public void StartMoving(RectTransform parentBounds)
    {
        bounds = parentBounds;
        rectTransform = GetComponent<RectTransform>();
        direction = Random.insideUnitCircle.normalized;
        isMoving = true;
    }

    void Update()
    {
        if (!isMoving) return;

        rectTransform.anchoredPosition += direction * speed * Time.deltaTime;

        Vector2 pos = rectTransform.anchoredPosition;
        Vector2 size = rectTransform.rect.size;
        float halfW = size.x / 2;
        float halfH = size.y / 2;

        float maxX = bounds.rect.width / 2 - halfW;
        float maxY = bounds.rect.height / 2 - halfH;

        if (pos.x < -maxX || pos.x > maxX)
            direction.x *= -1;
        if (pos.y < -maxY || pos.y > maxY)
            direction.y *= -1;
    }
}
