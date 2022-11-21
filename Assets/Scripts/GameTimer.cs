using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    private int GAME_PLAYTIME = 30;
    private int timeLeft;
    private TextMeshProUGUI textTimeLeft;
    float startTime;
    Game scriptGame;

    public int TimeLeft { get => timeLeft; set => timeLeft = value; }

    // Start is called before the first frame update
    void Start()
    {
        textTimeLeft = GetComponent<TextMeshProUGUI>();
        scriptGame = GameObject.Find("/Scripts/Game").GetComponent<Game>();
    }

    public void InitTimer()
    {
        //        startTime = double.Parse(PhotonNetwork.CurrentRoom.CustomProperties["StartTime"].ToString());
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
//        if (!Game.Instance.gameState.Equals(GameState.Game))
//        {
//            return;
//        }

//        int timeLeft = (int)(300 - (PhotonNetwork.Time - startTime));

        timeLeft = (int)(GAME_PLAYTIME - (Time.time - startTime));
        if (timeLeft <= 0 && !scriptGame.GameState.Equals(GameState_.WaitingForSecondPLayer))
        {
            timeLeft = 0;
            scriptGame.SetGameState(GameState_.GameOver);
        }
        textTimeLeft.text = "Time left: " + timeLeft.ToString();
    }
}
