using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject blockPrefab;
    public int columns = 8;
    public int rows = 5;
    public float spacing = 1.5f;
    public Vector3 startPosition = new Vector3(-5.25f, 12f, 0.15f);

    void Start()
    {
        Color[] rowColors = new Color[]
        {
            Color.green,
            new Color(1f, 0.4f, 0.7f),
            Color.blue,
            Color.yellow,
            Color.red
        };

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector3 position = startPosition + new Vector3(col * 1.6f, row * 1.1f, -0.4f);
                GameObject block = Instantiate(blockPrefab, position, Quaternion.identity);

                Renderer rend = block.GetComponent<Renderer>();
                if (rend != null && row < rowColors.Length)
                {
                    rend.material.color = rowColors[row];
                }
            }
        }
    }

}