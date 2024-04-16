using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMoves : Level
{

    public int numMoves;
    public int targetScore;

    private int movesUsed = 0;
    // Start is called before the first frame update
    void Start()
    {
        type = LevelType.MOVES;

        Debug.Log("Number of moves:" + numMoves + "Target score: " + targetScore);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnMove()
    {
        movesUsed++;

        Debug.Log("Moves remaining:" + (numMoves - movesUsed));

        if(numMoves - movesUsed == 0)
        {
            if(currentScore >= targetScore)
            {
                GameWin();
            }
            else
            {
                GameLose();
            }
        }
    }
}
