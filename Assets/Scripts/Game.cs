using AirplaneGame;
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState_
{
    WaitingForSecondPLayer,
    Playing,
    GameOver
}

public class Game : MonoBehaviour
{
    private GameState_ gameState;

    public GameState_ GameState { get => gameState; set => gameState = value; }
    public Transform PfStaticAirplaneRed { get => pfStaticAirplaneRed; set => pfStaticAirplaneRed = value; }
    public Transform PfStaticAirplaneBlue { get => pfStaticAirplaneBlue; set => pfStaticAirplaneBlue = value; }

    [SerializeField] private RectTransform minimapAirplaneRed;
    [SerializeField] private RectTransform minimapAirplaneBlue;
    [SerializeField] private RectTransform healthBarRed;
    [SerializeField] private RectTransform healthBarBlue;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private GameObject panelStatus;
    [SerializeField] private TextMeshProUGUI textTryOutMode;

    [SerializeField] private Transform pfPlayerRed;
    [SerializeField] private Transform pfPlayerBlue;
    [SerializeField] private Transform pfStaticAirplaneRed;
    [SerializeField] private Transform pfStaticAirplaneBlue;
    [SerializeField] private GameTimer gameTimer;

    Transform redPlayer;
    Transform bluePlayer;

    // Start is called before the first frame update
    void Start()
    {
        bluePlayer = Instantiate(pfPlayerBlue);
        bluePlayer.name = "player_blue";
        bluePlayer.GetComponent<Player>().ColliderLandingZone = GameObject.Find("/AirportBlue/LandingZone").GetComponent<Collider>();
        bluePlayer.GetComponent<Player>().AirplaneSpawnPosition = GameObject.Find("/AirPlaneSpawnPositionBlue").transform;
        bluePlayer.GetComponent<Player>().StaticAirplaneSpawnPosition = GameObject.Find("/ExtraPlanesBlue").transform;
        bluePlayer.GetComponent<Player>().PfStaticAirplane = pfStaticAirplaneBlue;
        bluePlayer.GetComponent<Player>().IsRedPlane = false;
        cameraTransform.GetComponent<CinemachineFreeLook>().Follow = bluePlayer;
        cameraTransform.GetComponent<CinemachineFreeLook>().LookAt = bluePlayer;

        redPlayer = Instantiate(pfPlayerRed);
        redPlayer.name = "player_red";
        redPlayer.GetComponent<Player>().ColliderLandingZone = GameObject.Find("/AirportRed/LandingZone").GetComponent<Collider>();
        redPlayer.GetComponent<Player>().AirplaneSpawnPosition = GameObject.Find("/AirPlaneSpawnPositionRed").transform;
        redPlayer.GetComponent<Player>().StaticAirplaneSpawnPosition = GameObject.Find("/ExtraPlanesRed").transform;
        redPlayer.GetComponent<Player>().PfStaticAirplane = pfStaticAirplaneRed;
        redPlayer.GetComponent<Player>().IsRedPlane = true;
        cameraTransform.GetComponent<CinemachineFreeLook>().Follow = redPlayer;
        cameraTransform.GetComponent<CinemachineFreeLook>().LookAt = redPlayer;

        GameObject.Find("CanvasHelp").GetComponent<Canvas>().enabled = false;

        SetGameState(GameState_.WaitingForSecondPLayer);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameState.Equals(GameState_.WaitingForSecondPLayer))
        {
            if (Time.time*2%2 < 1)
            {
                textTryOutMode.enabled = true;
            }
            else
            {
                textTryOutMode.enabled = false;
            }
        }    

//        healthRed = 100 - (Time.time * 100f) % 100;
        healthBarRed.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, GlobalParams.HealthRed * 1.57f);
        healthBarBlue.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, GlobalParams.HealthBlue * 1.57f);

        // 0,-14..221,-236 <-> -5000,5000..5000,-5000
        // 0,0..130,-120 <-> -5000,5000..5000,-5000
        float minimapXPos = (float)(130 * (redPlayer.position.x + 5000) / 10000.0);
        float minimapYPos = (float)(-120 + 120 * (redPlayer.position.z + 5000) / 10000.0);
        minimapAirplaneRed.anchoredPosition = new Vector3(minimapXPos, minimapYPos, 0); 
        minimapAirplaneRed.rotation = Quaternion.Euler(0, 0, -redPlayer.rotation.eulerAngles.y);

        minimapXPos = (float)(130 * (bluePlayer.position.x + 5000) / 10000.0);
        minimapYPos = (float)(-120 + 120 * (bluePlayer.position.z + 5000) / 10000.0);
        minimapAirplaneBlue.anchoredPosition = new Vector3(minimapXPos, minimapYPos, 0);
        minimapAirplaneBlue.rotation = Quaternion.Euler(0, 0, -bluePlayer.rotation.eulerAngles.y);
    }

    private void SetGameOverInfo()
    {
        if (gameTimer.TimeLeft == 0)
        {
            GlobalParams.GameOverReason = "Because the base is in better condition than the enemy";
        }
        else if (bluePlayer.GetComponent<Player>().NumAirplanes == 0 || redPlayer.GetComponent<Player>().NumAirplanes == 0)
        {
            GlobalParams.GameOverReason = "Because the enemy has no airplanes left";
        }
        else
        {
            GlobalParams.GameOverReason = "Because he destroyed the enemy base";
        }
        if (bluePlayer.GetComponent<Player>().NumAirplanes == 0 || GlobalParams.HealthBlue == 0 || GlobalParams.HealthRed > GlobalParams.HealthBlue)
        {
            GlobalParams.PlayerWon = 0;
        }
        else if (redPlayer.GetComponent<Player>().NumAirplanes == 0 || GlobalParams.HealthRed == 0 || GlobalParams.HealthBlue > GlobalParams.HealthRed)
        {
            GlobalParams.PlayerWon = 1;
        }
        else
        {
            GlobalParams.PlayerWon = 2; 
            GlobalParams.GameOverReason = "";
        }
        GlobalParams.DistanceBlue = bluePlayer.GetComponent<Player>().DistanceTravelled / 1000.0f;
        GlobalParams.DistanceRed = redPlayer.GetComponent<Player>().DistanceTravelled / 1000.0f;
    }

    public Player GetOtherPlayer(Player player)
    {
        if (player.IsRedPlane)
        {
            return bluePlayer.GetComponent<Player>();
        }
        else 
        {
            return redPlayer.GetComponent<Player>();
        }
    }

    public void SetGameState(GameState_ newGameState)
    {
        gameState = newGameState;

        switch (newGameState)
        {
            case GameState_.GameOver:
                SetGameOverInfo();
                SceneManager.LoadSceneAsync("GameOver");
                break;
            case GameState_.WaitingForSecondPLayer:
                panelStatus.SetActive(false);
                GlobalParams.HealthRed = GlobalParams.HealthBlue = 100;
                redPlayer.GetComponent<Player>().ResetGame();
                bluePlayer.GetComponent<Player>().ResetGame();
                break;
            case GameState_.Playing:
                textTryOutMode.enabled = false;
                panelStatus.SetActive(true);
                GlobalParams.HealthRed = GlobalParams.HealthBlue = 100;
                redPlayer.GetComponent<Player>().ResetGame();
                bluePlayer.GetComponent<Player>().ResetGame();
                gameTimer.InitTimer();
                break;
        }
    }

    public void OnExitGameClick()
    {
        Application.Quit();
    }
}

