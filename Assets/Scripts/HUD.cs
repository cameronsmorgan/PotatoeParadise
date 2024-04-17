using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{

    public Level level;
    public UnityEngine.UI.Text remainingText;
    public UnityEngine.UI.Text remainingSubText;
    public UnityEngine.UI.Text targetText;
    public UnityEngine.UI.Text targetSubtext;
    public UnityEngine.UI.Text scoreText;
    public UnityEngine.UI.Image[] stars;

    private int starIdx = 0;
    private bool isGameOver = false;



    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < stars.Length; i++)
        {
            if(i == starIdx)
            {
                stars[i].enabled= true;
            }
            else
            {
                stars[i].enabled= false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetScore(int score)
    {
        scoreText.text = score.ToString();

        int visibleStar = 0;

        if(score >= level.score1Star && score < level.score2Star)
        {
            visibleStar= 1;
        }
        else if(score >= level.score2Star && score < level.score3Star)
        {
            visibleStar= 2;
        }
        else if(score >= level.score3Star)
        {
            visibleStar= 3;
        }

        for(int i = 0; i < stars.Length; i++)
        {
            if(i == visibleStar)
            {
                stars[i].enabled = true;
            }
            else
            {
                stars[i].enabled= false;
            }
        }

        starIdx = visibleStar;
    }

    public void SetTarget(int target)
    {
        targetText.text = target.ToString();
    }

    public void SetRemaining(int remaining)
    {
        remainingText.text = remaining.ToString();
    }

    public void SetRemaining(string remaining)
    {
        remainingText.text = remaining;
    }

    public void SetLevelType(Level.LevelType type)
    {
        remainingSubText.text = "moves remaining";
        targetSubtext.text = "target score";
    }

    public void OnGameWin(int score)
    {
        isGameOver= true;
    }

    public void OnGameLose()
    {
        isGameOver= true;
    }
        

}
