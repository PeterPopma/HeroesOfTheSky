using AirplaneGame;
using Cinemachine;
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
    private int timeLeft;
    private float timeStarted;
    private GameState_ gameState;
    private const int GAME_PLAYTIME = 300;
    float healthRed;
    float healthBlue;

    public GameState_ GameState { get => gameState; set => gameState = value; }
    public float HealthRed { get => healthRed; set => healthRed = value; }
    public float HealthBlue { get => healthBlue; set => healthBlue = value; }
    public Transform AirplaneSpawnPositionRed { get => airplaneSpawnPositionRed; set => airplaneSpawnPositionRed = value; }
    public Transform AirplaneSpawnPositionBlue { get => airplaneSpawnPositionBlue; set => airplaneSpawnPositionBlue = value; }
    public Transform PfStaticAirplaneRed { get => pfStaticAirplaneRed; set => pfStaticAirplaneRed = value; }
    public Transform PfStaticAirplaneBlue { get => pfStaticAirplaneBlue; set => pfStaticAirplaneBlue = value; }
    public Transform StaticAirplaneSpawnPositionRed { get => staticAirplaneSpawnPositionRed; set => staticAirplaneSpawnPositionRed = value; }
    public Transform StaticAirplaneSpawnPositionBlue { get => staticAirplaneSpawnPositionBlue; set => staticAirplaneSpawnPositionBlue = value; }

    [SerializeField] private TextMeshProUGUI textTimeLeft;
    [SerializeField] private RectTransform minimapAirplaneRed;
    [SerializeField] private RectTransform minimapAirplaneBlue;
    [SerializeField] private RectTransform healthBarRed;
    [SerializeField] private RectTransform healthBarBlue;
    [SerializeField] private Transform cameraTransform;

    [SerializeField] private Transform pfPlayerRed;
    [SerializeField] private Transform pfPlayerBlue;
    [SerializeField] private Transform airplaneSpawnPositionRed;
    [SerializeField] private Transform airplaneSpawnPositionBlue;
    [SerializeField] private Transform pfStaticAirplaneRed;
    [SerializeField] private Transform pfStaticAirplaneBlue;
    [SerializeField] private Transform staticAirplaneSpawnPositionRed;
    [SerializeField] private Transform staticAirplaneSpawnPositionBlue;

    Transform redPlayer;
    Transform bluePlayer;

    // Start is called before the first frame update
    void Start()
    {
        timeStarted = Time.time;

        bluePlayer = Instantiate(pfPlayerBlue);
        bluePlayer.name = "player_blue";
        bluePlayer.GetComponent<Player>().IsRedPlane = false;
        cameraTransform.GetComponent<CinemachineFreeLook>().Follow = bluePlayer;
        cameraTransform.GetComponent<CinemachineFreeLook>().LookAt = bluePlayer;

        redPlayer = Instantiate(pfPlayerRed);
        redPlayer.name = "player_red";
        redPlayer.GetComponent<Player>().IsRedPlane = true;
        cameraTransform.GetComponent<CinemachineFreeLook>().Follow = redPlayer;
        cameraTransform.GetComponent<CinemachineFreeLook>().LookAt = redPlayer;

        GameObject.Find("CanvasHelp").GetComponent<Canvas>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
//        healthRed = 100 - (Time.time * 100f) % 100;
        healthBarRed.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, healthRed * 1.728f);
        healthBarBlue.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, healthBlue * 1.728f);

        timeLeft = (int)(GAME_PLAYTIME - (Time.time - timeStarted));
        if (timeLeft <= 0)
        {
            timeLeft = 0;
            SetGameState(GameState_.GameOver);
        }
        textTimeLeft.text = "Time left: " + timeLeft.ToString("000");
        
        // 0,-14..221,-236 <-> -5000,5000..5000,-5000
        float minimapXPos = (float)(221 * (redPlayer.position.x + 5000) / 10000.0);
        float minimapYPos = (float)(-236 + 222 * (redPlayer.position.z + 5000) / 10000.0);
        minimapAirplaneRed.anchoredPosition = new Vector3(minimapXPos, minimapYPos, 0); 
        minimapAirplaneRed.rotation = Quaternion.Euler(0, 0, -redPlayer.rotation.eulerAngles.y);

        minimapXPos = (float)(221 * (bluePlayer.position.x + 5000) / 10000.0);
        minimapYPos = (float)(-236 + 222 * (bluePlayer.position.z + 5000) / 10000.0);
        minimapAirplaneBlue.anchoredPosition = new Vector3(minimapXPos, minimapYPos, 0);
        minimapAirplaneBlue.rotation = Quaternion.Euler(0, 0, -bluePlayer.rotation.eulerAngles.y);
    }

    public void SetGameState(GameState_ newGameState)
    {
        gameState = newGameState;

        switch (newGameState)
        {
            case GameState_.GameOver:
                break;
            case GameState_.Playing:
                healthRed = healthBlue = 100;
                timeStarted = Time.time;
                break;
        }
    }

    public void OnExitGameClick()
    {
        Application.Quit();
    }
}

