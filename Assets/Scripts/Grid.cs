using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public enum PieceType
    {
        EMPTY,
        NORMAL,
        BUBBLE,
        COUNT,
    };

    [System.Serializable]
    public struct PiecePrefab
    {
        public PieceType type;
        public GameObject prefab;
    }


    public int xDim;
    public int yDim;
    public float fillTime;

    public PiecePrefab[] piecePrefabs;
    public GameObject backgroundPrefab;


    private Dictionary<PieceType, GameObject> piecePrefabDict;

    private GamePiece[,] pieces;
    private bool inverse = false;

    private GamePiece pressedPiece;
    private GamePiece enteredPiece;

    // Start is called before the first frame update
    void Start()
    {
        piecePrefabDict = new Dictionary<PieceType, GameObject>();

        for(int i = 0; i < piecePrefabs.Length; i++)
        {
            if (!piecePrefabDict.ContainsKey(piecePrefabs[i].type))
            {
                piecePrefabDict.Add(piecePrefabs[i].type, piecePrefabs[i].prefab);
            }
        }

        for(int x = 0; x < xDim; x++)
        {
            for(int y = 0; y < yDim; y++)
            {
                GameObject background = (GameObject)Instantiate(backgroundPrefab, GetWorldPosition(x,y), Quaternion.identity);
                background.transform.parent = transform;
            }
        }

        pieces = new GamePiece[xDim, xDim];
        for(int x = 0; x < xDim; x++)
        {
            for(int y = 0; y < yDim; y++)
            {
                SpawnNewPiece(x, y, PieceType.EMPTY);
            }
        }

        Destroy(pieces[1, 4].gameObject);
        SpawnNewPiece(1, 4, PieceType.BUBBLE);

        Destroy(pieces[2, 4].gameObject);
        SpawnNewPiece(2, 4, PieceType.BUBBLE);

        Destroy(pieces[3, 4].gameObject);
        SpawnNewPiece(3, 4, PieceType.BUBBLE);

        Destroy(pieces[5, 4].gameObject);
        SpawnNewPiece(5, 4, PieceType.BUBBLE);

        Destroy(pieces[6, 4].gameObject);
        SpawnNewPiece(6, 4, PieceType.BUBBLE);

        Destroy(pieces[7, 4].gameObject);
        SpawnNewPiece(7, 4, PieceType.BUBBLE);

        Destroy(pieces[4, 0].gameObject);
        SpawnNewPiece(4, 0, PieceType.BUBBLE);

       StartCoroutine( Fill());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator Fill()
    {
        bool needsRefill = true;

        while(needsRefill)
        {
            yield return new WaitForSeconds(fillTime);

            while (FillStep())
            {

                inverse = !inverse;
                yield return new WaitForSeconds(fillTime);
            }

            needsRefill = ClearAllValidMatches();
        }
    }

    public bool FillStep()
    {
        bool movedPiece = false;

        for(int y = yDim - 2; y>= 0; y--)
        {
            for(int loopX = 0; loopX < xDim; loopX++)
            {
                int x = loopX;

                if(inverse)
                {
                    x = xDim - 1 - loopX;
                }
                GamePiece piece = pieces[x, y];
                if(piece.isMoveable())
                {
                    GamePiece pieceBelow = pieces[x, y + 1];

                    if(pieceBelow.Type == PieceType.EMPTY)
                    {
                        Destroy(pieceBelow.gameObject);
                        piece.MoveableComponent.Move(x, y + 1, fillTime);
                        pieces[x, y + 1] = piece;
                        SpawnNewPiece(x, y, PieceType.EMPTY);
                        movedPiece= true;
                    }
                    else
                    {
                        for(int diag = -1; diag <= 1; diag++)
                        {
                            if(diag != 0 )
                            {
                                int diagX = x + diag;

                                if(inverse)
                                {
                                    diagX = x - diag;
                                }

                                if(diagX >= 0 && diagX < xDim)
                                {
                                    GamePiece diagonalPiece = pieces[diagX, y + 1];

                                    if(diagonalPiece.Type == PieceType.EMPTY)
                                    {
                                        bool hasPieceAbove = true;

                                        for(int aboveY = y; aboveY >= 0; aboveY--)
                                        {
                                            GamePiece pieceAbove = pieces[diagX, aboveY];

                                            if(pieceAbove.isMoveable())
                                            {
                                                break;
                                            }
                                            else if(!pieceAbove.isMoveable() && pieceAbove.Type != PieceType.EMPTY)
                                            {
                                                hasPieceAbove= false;
                                                break;
                                            }
                                        }
                                        if(!hasPieceAbove)
                                        {
                                            Destroy(diagonalPiece.gameObject);
                                            piece.MoveableComponent.Move(diagX, y + 1, fillTime);
                                            pieces[diagX, y + 1] = piece;
                                            SpawnNewPiece(x, y, PieceType.EMPTY);
                                            movedPiece= true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        for(int x =0; x<xDim ; x++)
        {
            GamePiece pieceBelow = pieces[x, 0];

            if(pieceBelow.Type == PieceType.EMPTY)
            {
                Destroy(pieceBelow.gameObject);
                GameObject newPiece = (GameObject)Instantiate(piecePrefabDict[PieceType.NORMAL], GetWorldPosition(x, -1), Quaternion.identity);
                newPiece.transform.parent = transform;

                pieces[x, 0] = newPiece.GetComponent<GamePiece>();
                pieces[x, 0].Init(x, -1, this, PieceType.NORMAL);
                pieces[x, 0].MoveableComponent.Move(x, 0, fillTime);
                pieces[x, 0].ColorComponent.SetColor((ColorPiece.ColorType)Random.Range(0, pieces[x, 0].ColorComponent.NumColors));
                movedPiece= true;
            }
        }

        return movedPiece;
    }

   public Vector2 GetWorldPosition(int x, int y)
    {
        return new Vector2(transform.position.x - xDim / 2.0f + x,
            transform.position.y + yDim / 2.0f - y);
    }

    public GamePiece SpawnNewPiece(int x, int y, PieceType type)
    {
        GameObject newPiece = (GameObject)Instantiate(piecePrefabDict[type], GetWorldPosition(x, y), Quaternion.identity);
        newPiece.transform.parent = transform;

        pieces[x,y] = newPiece.GetComponent<GamePiece>();
        pieces[x, y].Init(x, y, this, type);

        return pieces[x, y];
    }

    public bool IsAdjacent(GamePiece piece1, GamePiece piece2)
    {
        return (piece1.X == piece2.X && (int)Mathf.Abs(piece1.Y - piece2.Y) == 1)
            || (piece1.Y == piece2.Y && (int)Mathf.Abs(piece1.X - piece2.X) == 1);
    }

    public void SwapPieces(GamePiece piece1, GamePiece piece2)
    {
        if(piece1.isMoveable() && piece2.isMoveable())
        {
            pieces[piece1.X, piece1.Y] = piece2;
            pieces[piece2.X, piece2.Y] = piece1;

            if(GetMatch(piece1, piece2.X, piece2.Y) != null || GetMatch(piece2, piece1.X, piece1.Y) != null)
            {
                int piece1X = piece1.X;
                int piece1Y = piece1.Y;

                piece1.MoveableComponent.Move(piece2.X, piece2.Y, fillTime);
                piece2.MoveableComponent.Move(piece1X, piece1Y, fillTime);

                ClearAllValidMatches();

                StartCoroutine(Fill());
            }
            else
            {
                pieces[piece1.X, piece1.Y] = piece1;
                pieces[piece2.X, piece2.Y] = piece2;
            }

           
        }
    }

    public void PressPiece(GamePiece piece)
    {
        pressedPiece = piece;
    }

    public void EnterPiece(GamePiece piece)
    {
        enteredPiece = piece;
    }

    public void ReleasePiece()
    {
        if(IsAdjacent (pressedPiece, enteredPiece))
        {
            SwapPieces(pressedPiece, enteredPiece);
        }
    }

    public List<GamePiece> GetMatch(GamePiece piece, int newX, int newY)
    {
        if(piece.IsColored())
        {
            ColorPiece.ColorType color = piece.ColorComponent.Color;
            List<GamePiece> horizontalPieces= new List<GamePiece>();
            List<GamePiece> verticalPieces = new List<GamePiece>();
            List<GamePiece> matchingPieces = new List<GamePiece>();

            horizontalPieces.Add(piece);

            for(int dir = 0; dir <= 1; dir++)
            {
                for(int xOffset = 1; xOffset < xDim; xOffset++)
                {
                    int x;

                    if(dir == 0)
                    {
                        x = newX - xOffset;
                    }
                    else
                    {
                        x = newX + xOffset;
                    }

                    if(x < 0 || x >= xDim)
                    {
                        break;
                    }

                    if (pieces[x, newY].IsColored() && pieces[x , newY].ColorComponent.Color == color)
                    {
                        horizontalPieces.Add(pieces[x, newY]);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if(horizontalPieces.Count >= 3)
            {
                for(int i = 0; i < horizontalPieces.Count; i++)
                {
                    matchingPieces.Add(horizontalPieces[i]);
                }
            }

            if(horizontalPieces.Count >=3)
            {
                for(int i = 0; i < horizontalPieces.Count; i++)
                {
                    for(int dir =0; dir <= 1; dir++)
                    {
                        for(int yOffset = 1; yOffset < yDim; yOffset++)
                        {
                            int y;

                            if(dir == 0)
                            {
                                y = newY- yOffset;
                            }
                            else
                            {
                                y = newY + yOffset;
                            }

                            if(y < 0 || y>= yDim)
                            {
                                break;
                            }

                            if (pieces[horizontalPieces[i].X, y].IsColored() && pieces[horizontalPieces[i].X, y].ColorComponent.Color == color)
                            {
                                verticalPieces.Add(pieces[horizontalPieces[i].X, y]);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    if(verticalPieces.Count < 2)
                    {
                        verticalPieces.Clear();
                    }
                    else
                    {
                        for(int j =0; j<verticalPieces.Count; j++)
                        {
                            matchingPieces.Add(verticalPieces[j]);
                        }
                        break;
                    }
                }
            }

            if(matchingPieces.Count >= 3)
            {
                return matchingPieces;
            }


            horizontalPieces.Clear();
            verticalPieces.Clear();
            verticalPieces.Add(piece);

            for (int dir = 0; dir <= 1; dir++)
            {
                for (int yOffset = 1; yOffset < xDim; yOffset++)
                {
                    int y;

                    if (dir == 0)
                    {
                        y = newX - yOffset;
                    }
                    else
                    {
                        y = newX + yOffset;
                    }

                    if (y < 0 || y >= xDim)
                    {
                        break;
                    }

                    if (pieces[newX, y].IsColored() && pieces[newX, y].ColorComponent.Color == color)
                    {
                        verticalPieces.Add(pieces[newX, y]);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (verticalPieces.Count >= 3)
            {
                for (int i = 0; i < verticalPieces.Count; i++)
                {
                    matchingPieces.Add(verticalPieces[i]);
                }
            }

            if (verticalPieces.Count >= 3)
            {
                for (int i = 0; i < verticalPieces.Count; i++)
                {
                    for (int dir = 0; dir <= 1; dir++)
                    {
                        for (int xOffset = 1; xOffset < yDim; xOffset++)
                        {
                            int x;

                            if (dir == 0)
                            {
                                x = newX - xOffset;
                            }
                            else
                            {
                                x = newX + xOffset;
                            }

                            if (x < 0 || x >= yDim)
                            {
                                break;
                            }

                            if (pieces[x ,verticalPieces[i].Y].IsColored() && pieces[x, verticalPieces[i].Y].ColorComponent.Color == color)
                            {
                                verticalPieces.Add(pieces[x, verticalPieces[i].Y]);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    if (horizontalPieces.Count < 2)
                    {
                        horizontalPieces.Clear();
                    }
                    else
                    {
                        for (int j = 0; j < horizontalPieces.Count; j++)
                        {
                            matchingPieces.Add(horizontalPieces[j]);
                        }
                        break;
                    }
                }
            }

            if (matchingPieces.Count >= 3)
            {
                return matchingPieces;
            }
        }

        return null;
    }

    public bool ClearAllValidMatches()
    {
        bool needsRefill = false;

        for(int y = 0; y<yDim; y++)
        {
            for(int x = 0; x< xDim; x++)
            {
                if (pieces[x,y].isClearable())
                {
                    List<GamePiece> match = GetMatch(pieces[x, y], x, y);

                    if (match != null)
                    {
                        for(int i =0; i < match.Count; i++)
                        {
                            if (ClearPiece(match[i].X, match[i].Y))
                            {
                                needsRefill= true;
                            }
                        }
                    }
                }
            }
        }

        return needsRefill;
    }

    public bool ClearPiece(int x, int y)
    {
        if (pieces[x,y].isClearable() && !pieces[x,y].ClearableComponent.IsBeingCleared)
        {
            pieces[x, y].ClearableComponent.Clear();
            SpawnNewPiece(x, y, PieceType.EMPTY);

            ClearObstacles(x, y);

            return true;
        }

        return false;
    }

    public void ClearObstacles(int x, int y)
    {
        for(int adjacentX = x-1; adjacentX <= x+1; adjacentX++)
        {
            if(adjacentX!=x && adjacentX >= 0 && adjacentX < xDim)
            {
                if (pieces[adjacentX, y].Type == PieceType.BUBBLE && pieces[adjacentX, y].isClearable())
                {
                    pieces[adjacentX, y]. ClearableComponent.Clear();
                    SpawnNewPiece(adjacentX, y, PieceType.EMPTY);
                }
            }
        }

        for(int adjacentY = y - 1; adjacentY <= y + 1; adjacentY++)
        {
            if(adjacentY != y && adjacentY >= 0 && adjacentY < yDim)
            {
                if (pieces[x, adjacentY].Type == PieceType.BUBBLE&& pieces[x, adjacentY].isClearable())
                {
                    pieces[x, adjacentY].ClearableComponent.Clear();
                    SpawnNewPiece(x, adjacentY, PieceType.EMPTY);
                }
            }
        }
    }
}
