using UnityEngine;

public class BrickSpawner : MonoBehaviour
{
    public GameObject brickPrefab;
    public int rows = 5;
    public int cols = 8;
    public float spacing = 1.2f;

    void Start()
    {
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                Vector2 pos = new Vector2(
                    x * spacing - (cols / 2f),
                    y * spacing + 2
                );

                Instantiate(brickPrefab, pos, Quaternion.identity);
            }
        }
    }
}