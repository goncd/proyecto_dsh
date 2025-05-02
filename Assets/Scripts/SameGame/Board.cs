using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    public int width = 20;
    public int height = 10;

    public GameObject blockPrefab;

    public TMP_Text objective;

    private int objectivePoints;

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

    public TMP_Text pointsCounter;

    private int points = 0;

    public AudioClip destroySound;

    AudioSource audioSource;

    public GameObject gameOverCanvas;
    public Button gameOverRetryButton;
    public Button gameOverExitButton;


    public GameObject pauseCanvas;
    public Button pauseContinueButton;
    public Button pauseRestartButton;
    public Button pauseExitButton;
    public Button pauseButton;

    public GameObject gameFinishedCanvas;
    public Button gameFinishedContinueButton;

    public bool IsGameWorking()
    {
        return !gameOverCanvas.activeInHierarchy && !pauseCanvas.activeInHierarchy && !gameFinishedCanvas.activeInHierarchy;
    }

    private void AddPoints(int value)
    {
        points += value;
        GameState.Instance.Set("samegame_points", points);

        pointsCounter.text = $"Puntos: {points}";
    }

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

        if (GameState.Instance.Get("samegame_objective", out int pointsGoal))
        {
            objectivePoints = pointsGoal;
            objective.text = $"Objetivo: {objectivePoints}";
            objective.gameObject.SetActive(true);
        }

        gameOverRetryButton.onClick.AddListener(() => SceneLoader.Instance.ReloadCurrentScene());
        gameOverExitButton.onClick.AddListener(() => SceneLoader.Instance.LoadPreviousScene());

        pauseContinueButton.onClick.AddListener(() => { pauseCanvas.SetActive(false); if (timeCounter.CountedTime != 0f) timeCounter.StartCounter(); });
        pauseRestartButton.onClick.AddListener(() => SceneLoader.Instance.ReloadCurrentScene());
        pauseExitButton.onClick.AddListener(() => SceneLoader.Instance.LoadPreviousScene());
        pauseButton.onClick.AddListener(() => { pauseCanvas.SetActive(true); timeCounter.StopCounter(); });

        gameFinishedContinueButton.onClick.AddListener(() => SceneLoader.Instance.LoadPreviousScene());
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

        // Recurse in 4 directions.
        FindMatchingBlocks(x + 1, y, color, matches);
        FindMatchingBlocks(x - 1, y, color, matches);
        FindMatchingBlocks(x, y + 1, color, matches);
        FindMatchingBlocks(x, y - 1, color, matches);
    }

    IEnumerator WaitThenCollapseColumn(float time, int x)
    {
        isMoving = true;

        yield return new WaitForSeconds(time);

        for (int y = 0; y < height; y++)
        {
            if (board[x, y] == null)
            {
                for (int y2 = y + 1; y2 < height; y2++)
                {
                    if (board[x, y2] != null)
                    {
                        // Moves the block downwards.
                        board[x, y] = board[x, y2];
                        board[x, y2] = null;

                        // Update the logic and visual positions.
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

        AddPoints((int)System.Math.Pow(toDestroy.Count - 1, 2));

        if (objective.gameObject.activeInHierarchy && points >= objectivePoints)
        {
            gameFinishedCanvas.SetActive(true);
            timeCounter.StopCounter();
        }

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

        // Wait for the vertical fall.
        yield return new WaitForSeconds(time);

        for (int x = 0; x < width; x++)
        {
            if (IsColumnEmpty(x))
            {
                int nextColumn = x + 1;
                while (nextColumn < width && IsColumnEmpty(nextColumn))
                    nextColumn++;

                if (nextColumn >= width)
                    break;

                // Move blocks horizontally.
                for (int y = 0; y < height; y++)
                {
                    board[x, y] = board[nextColumn, y];
                    if (board[x, y] != null)
                    {
                        Block b = board[x, y].GetComponent<Block>();
                        b.x = x;

                        // Move the block to its new position with an animation.
                        StartCoroutine(MoveBlockToPosition(board[x, y], GetWorldPosition(x, y)));
                    }

                    board[nextColumn, y] = null;
                }
            }
        }

        isMoving = false;

        yield return new WaitForSeconds(0.5f);

        if (!HasMovesLeft())
        {
            gameOverCanvas.SetActive(true);
            timeCounter.StopCounter();
        }
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

    public bool HasMovesLeft()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject current = board[x, y];

                if (current == null)
                    continue;

                Color blockColor = current.GetComponent<Block>().GetComponent<SpriteRenderer>().color;

                // Check if there are neighbours with the same color.
                if (HasSameColorNeighbor(x, y, blockColor))
                    return true;
            }
        }

        return false;
    }

    private bool HasSameColorNeighbor(int x, int y, Color color)
    {
        int[,] directions = new int[,] { { 1, 0 }, { -1, 0 }, { 0, 1 }, { 0, -1 } };

        for (int i = 0; i < 4; i++)
        {
            int nx = x + directions[i, 0];
            int ny = y + directions[i, 1];

            if (nx >= 0 && nx < width && ny >= 0 && ny < height)
            {
                GameObject neighbor = board[nx, ny];

                if (neighbor == null)
                    continue;

                Color blockColor = neighbor.GetComponent<Block>().GetComponent<SpriteRenderer>().color;

                if (blockColor == color)
                    return true;
            }
        }

        return false;
    }
}
