using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public int x;
    public int y;

    private Board board;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        board = GetComponentInParent<Board>();
    }

    void OnMouseDown()
    {
        if (board.IsGameWorking())
            board.DestroyBlock(x, y, GetComponent<SpriteRenderer>().color);
    }
}
