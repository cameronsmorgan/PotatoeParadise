using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePiece : MonoBehaviour
{

    private int x;
    private int y;

    public int X
    {
        get { return x; }
        set
        {
            if(isMoveable())
            {
                x = value;
            }
        }
    }

    public int Y
    {
        get { return y; }
        set
        {
            if(isMoveable())
            {
                y = value;
            }
        }
    }

    private Grid.PieceType type;

    public Grid.PieceType Type
    {
        get { return type; }
    }

    private Grid grid;

    public Grid GridRef
    {
        get { return grid; }
    }

    private MoveablePiece moveableComponent;

    public MoveablePiece MoveableComponent
    {
        get { return moveableComponent; }
    }

    private ColorPiece colorComponent;
    public ColorPiece ColorComponent
    {
        get { return colorComponent; }
    }


    private void Awake()
    {
        moveableComponent = GetComponent<MoveablePiece>();
        colorComponent = GetComponent<ColorPiece>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(int _x, int _y, Grid _grid, Grid.PieceType _type)
    {
        x = _x;
        y = _y;
        grid = _grid;
        type = _type;

    }

    public bool isMoveable()
    {
        return moveableComponent != null;
    }

    public bool IsColored()
    {
        return colorComponent != null;
    }
}
