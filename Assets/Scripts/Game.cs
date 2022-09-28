using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum GameState_
{
    Playing,
    GameOver
}

public class Game : MonoBehaviour
{
    private int score;
    private int timeLeft;
    private float timeStarted;
    private GameState_ gameState;

    public int Score { get => score; set => score = value; }
    public GameState_ GameState { get => gameState; set => gameState = value; }

    [SerializeField] private TextMeshProUGUI textScore;
    [SerializeField] private TextMeshProUGUI textScoreGameOver;
    [SerializeField] private TextMeshProUGUI textTimeLeft;
    [SerializeField] private Transform panelGameOver;

    // Start is called before the first frame update
    void Start()
    {
        timeStarted = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        timeLeft = (int)(100 - (Time.time - timeStarted));
        if (timeLeft <= 0)
        {
            timeLeft = 0;
            SetGameState(GameState_.GameOver);
        }
        textTimeLeft.text = "Time left: " + timeLeft.ToString("000");
    }

    public void SetGameState(GameState_ newGameState)
    {
        gameState = newGameState;

        switch (newGameState)
        {
            case GameState_.GameOver:
                textScoreGameOver.text = score.ToString("00000");
                panelGameOver.gameObject.SetActive(true);
                break;
            case GameState_.Playing:
                timeStarted = Time.time;
                panelGameOver.gameObject.SetActive(false);
                break;
        }
    }

    public void IncreaseScore(int amount)
    {
        score += amount;
        textScore.text = "Score: " + score.ToString("00000");
    }
}
