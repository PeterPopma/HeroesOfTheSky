using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private AudioSource soundTimerAlarm;

    private int GAME_PLAYTIME = 30;
    private int timeLeft;
    private TextMeshProUGUI textTimeLeft;
    float startTime;
    Game scriptGame;
    bool alarmPlaying;

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
        soundTimerAlarm.Stop();
        alarmPlaying = false;
    }

    // Update is called once per frame
    void Update()
    {
        timeLeft = (int)(GAME_PLAYTIME - (Time.time - startTime));
        if (timeLeft <= 0 && !scriptGame.GameState.Equals(GameState_.WaitingForSecondPlayer))
        {
            timeLeft = 0;
            scriptGame.SetGameState(GameState_.WaitingForSecondPlayerStats);
        }
        textTimeLeft.text = "Time left: " + timeLeft.ToString();
        if (timeLeft<10 && !alarmPlaying)
        {
            alarmPlaying = true;
            soundTimerAlarm.Play();
        }
    }
}
