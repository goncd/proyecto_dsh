using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int width = 20;
    public int height = 10;

    public GameObject blockPrefab;

    private GameObject[,] board;

    private bool isMoving = false;

    private static readonly Color[] colors = new Color[] {
        Color.cyan,
        Color.red,
        Color.green,
        Color.yellow
    };

    private Vector2 boardOffset;

    public TimeCounter timeCounter;
    public PointsCounter pointsCounter;

    public AudioClip destroySound;

    AudioSource audioSource;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = destroySound;
        board = new GameObject[width, height];

        boardOffset = new(-(width - 1) / 2f, -height / 2f);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject block = Instantiate(blockPrefab, GetWorldPosition(x, y), Quaternion.identity, transform);
                block.GetComponent<SpriteRenderer>().color = colors[Random.Range(0, colors.Length)];

                Block b = block.GetComponent<Block>();

                b.x = x;
                b.y = y;

                board[x, y] = block;
            }
        }
    }

    void FindMatchingBlocks(int x, int y, Color color, List<GameObject> matches)
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
            return;

        GameObject block = board[x, y];

        if (block == null || matches.Contains(block))
            return;

        Color blockColor = block.GetComponent<SpriteRenderer>().color;

        if (blockColor != color)
            return;

        matches.Add(block);

        // Recurse in 4 directions
        FindMatchingBlocks(x + 1, y, color, matches);
        FindMatchingBlocks(x - 1, y, color, matches);
        FindMatchingBlocks(x, y + 1, color, matches);
        FindMatchingBlocks(x, y - 1, color, matches);
    }

    IEnumerator WaitThenCollapseColumn(float time, int x)
    {
        isMoving = true;

        yield return new WaitForSeconds(time); // espera después de caída vertical

        for (int y = 0; y < height; y++)
        {
            if (board[x, y] == null)
            {
                for (int y2 = y + 1; y2 < height; y2++)
                {
                    if (board[x, y2] != null)
                    {
                        // Mueve el bloque hacia abajo en la matriz
                        board[x, y] = board[x, y2];
                        board[x, y2] = null;

                        // Actualiza la posición lógica y visual
                        StartCoroutine(MoveBlockToPosition(board[x, y], GetWorldPosition(x, y)));

                        Block b = board[x, y].GetComponent<Block>();
                        b.y = y;

                        break;
                    }
                }
            }
        }
        isMoving = false;
    }

    public void DestroyBlock(int x, int y, Color color)
    {
        if (!timeCounter.HasCounterStarted())
            timeCounter.StartCounter();

        List<GameObject> toDestroy = new();
        FindMatchingBlocks(x, y, color, toDestroy);

        // Only delete if we found a group of two or more
        // and we are not moving blocks.
        if (toDestroy.Count < 2 || isMoving)
            return;

        audioSource.Play();

        pointsCounter.Points += (int)System.Math.Pow(toDestroy.Count - 1, 2);

        foreach (GameObject block in toDestroy)
        {
            Block b = block.GetComponent<Block>();

            board[b.x, b.y] = null;
            StartCoroutine(FadeAndDestroy(block));
        }

        for (int i = 0; i < width; i++)
            StartCoroutine(WaitThenCollapseColumn(0.3f, i));

        StartCoroutine(ShiftColumnsLeftAnimated(1f));
    }

    IEnumerator MoveBlockToPosition(GameObject block, Vector2 targetPosition)
    {
        float elapsed = 0f;
        float duration = 0.2f;
        Vector2 startPos = block.transform.position;

        while (elapsed < duration)
        {
            block.transform.position = Vector2.Lerp(startPos, targetPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        block.transform.position = targetPosition;
    }

    public IEnumerator ShiftColumnsLeftAnimated(float time)
    {
        isMoving = true;

        yield return new WaitForSeconds(time); // espera después de caída vertical

        for (int x = 0; x < width; x++)
        {
            if (IsColumnEmpty(x))
            {
                int nextColumn = x + 1;
                while (nextColumn < width && IsColumnEmpty(nextColumn))
                    nextColumn++;

                if (nextColumn >= width)
                    break;

                // Mueve bloques horizontalmente
                for (int y = 0; y < height; y++)
                {
                    board[x, y] = board[nextColumn, y];
                    if (board[x, y] != null)
                    {
                        Block b = board[x, y].GetComponent<Block>();
                        b.x = x;

                        // Mueve el bloque animadamente a su nueva posición
                        StartCoroutine(MoveBlockToPosition(board[x, y], GetWorldPosition(x, y)));
                    }

                    board[nextColumn, y] = null;
                }
            }
        }
        isMoving = false;

    }

    private Vector2 GetWorldPosition(int x, int y)
    {
        return new Vector2(x, y) + boardOffset;
    }

    private bool IsColumnEmpty(int x)
    {
        for (int y = 0; y < height; y++)
        {
            if (board[x, y] != null)
                return false;
        }
        return true;
    }

    public IEnumerator FadeAndDestroy(GameObject block)
    {
        isMoving = true;
        SpriteRenderer sr = block.GetComponent<SpriteRenderer>();
        Color originalColor = sr.color;

        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            elapsed += Time.deltaTime;
            yield return null;
        }

        sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        Destroy(block);
        isMoving = false;
    }
}
