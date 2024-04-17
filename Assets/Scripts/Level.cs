using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{

    public enum LevelType
    {
        TIMER,
        OBSTACLE,
        MOVES,
    };

    public Grid grid;

    public HUD hud;

    public int score1Star;
    public int score2Star;
    public int score3Star;


    protected LevelType type;

    public LevelType Type
    {
        get { return type; }
    }

    protected int currentScore;
    // Start is called before the first frame update
    void Start()
    {

        hud.SetScore(currentScore);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void GameWin()
    {
        Debug.Log("You Win!");
        hud.OnGameWin(currentScore);
        grid.GameOver();
    }

    public virtual void GameLose()
    {
        Debug.Log("You lose.");
        hud.OnGameLose();
        grid.GameOver();
    }

    public virtual void OnMove()
    {

    }

    public virtual void OnPieceCleared(GamePiece piece)
    {
        currentScore += piece.score;
        hud.SetScore(currentScore);
    }
}
