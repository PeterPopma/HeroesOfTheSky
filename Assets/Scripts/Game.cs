using AirplaneGame;
using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState_
{
    JoiningNetworkGame,
    WaitingForSecondPlayer,
    Playing,
    WaitingForSecondPlayerStats,
    GameOver
}

public class Game : MonoBehaviour
{
    private GameState_ gameState;
    public GameState_ GameState { get => gameState; set => gameState = value; }
    public Transform PfStaticAirplaneRed { get => pfStaticAirplaneRed; set => pfStaticAirplaneRed = value; }
    public Transform PfStaticAirplaneBlue { get => pfStaticAirplaneBlue; set => pfStaticAirplaneBlue = value; }
    public Transform CameraTransform { get => cameraTransform; set => cameraTransform = value; }
    public GameObject OtherPlayer { get => otherPlayer; set => otherPlayer = value; }
    public Transform StaticAirplaneSpawnPositionRed { get => staticAirplaneSpawnPositionRed; set => staticAirplaneSpawnPositionRed = value; }
    public Transform StaticAirplaneSpawnPositionBlue { get => staticAirplaneSpawnPositionBlue; set => staticAirplaneSpawnPositionBlue = value; }
    public GameObject PlayerRed { get => playerRed; set => playerRed = value; }
    public GameObject PlayerBlue { get => playerBlue; set => playerBlue = value; }
    public PowerUpSpawner PowerUpSpawner { get => powerUpSpawner; set => powerUpSpawner = value; }

    [SerializeField] private RectTransform minimapAirplaneRed;
    [SerializeField] private RectTransform minimapAirplaneBlue;
    [SerializeField] private RectTransform healthBarRed;
    [SerializeField] private RectTransform healthBarBlue;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private GameObject panelStatus;
    [SerializeField] private TextMeshProUGUI textTryOutMode;

    [SerializeField] private Transform pfStaticAirplaneRed;
    [SerializeField] private Transform pfStaticAirplaneBlue;
    [SerializeField] private Transform staticAirplaneSpawnPositionRed;
    [SerializeField] private Transform staticAirplaneSpawnPositionBlue;
    [SerializeField] private GameTimer gameTimer;
    [SerializeField] private Canvas canvasConnecting;
    [SerializeField] private PowerUpSpawner powerUpSpawner;

    GameObject ownPlayer;
    GameObject otherPlayer;
    GameObject playerRed;
    GameObject playerBlue;
    Player scriptOwnPlayer;


    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("CanvasHelp").GetComponent<Canvas>().enabled = false;
        SetGameState(GameState_.JoiningNetworkGame);
    }

    private void CreatePlayerRed()
    {
        ownPlayer = PhotonNetwork.Instantiate("pfPlayer", GameObject.Find("/AirPlaneSpawnPositionRed").transform.position, Quaternion.identity);
        ownPlayer.name = "player_red";
        scriptOwnPlayer = ownPlayer.GetComponent<Player>();
        scriptOwnPlayer.ColliderLandingZone = GameObject.Find("/AirportRed/LandingZone").GetComponent<Collider>();
        scriptOwnPlayer.AirplaneSpawnPosition = GameObject.Find("/AirPlaneSpawnPositionRed").transform;
        scriptOwnPlayer.PfStaticAirplane = pfStaticAirplaneRed;
        scriptOwnPlayer.IsRedPlane = true;
        scriptOwnPlayer.PhotonView.RPC("ChangePlaneColor", RpcTarget.All, scriptOwnPlayer.PhotonView.ViewID, "red");
        playerRed = ownPlayer;
    }

    public void SpawnHouses()
    {
        // Delete any existing houses
        GameObject[] existingObjects = GameObject.FindGameObjectsWithTag("house");
        foreach (GameObject existingObject in existingObjects)
        {
            PhotonNetwork.Destroy(existingObject);
        }
        GameObject newHouse = PhotonNetwork.Instantiate("pfHouseRed", new Vector3(-3494.97314f, 136.899994f, -4602.46777f), Quaternion.Euler(new Vector3(270, 20.2657928f, 0)));
        newHouse = PhotonNetwork.Instantiate("pfHouseRed", new Vector3(-1462.17f, 166.5f, -3999.1f), Quaternion.Euler(new Vector3(270, 20.2657928f, 0)));
        newHouse = PhotonNetwork.Instantiate("pfHouseRed", new Vector3(-1972.1f, 120.5f, -2595.1f), Quaternion.Euler(new Vector3(270, 20.2657928f, 0)));
        newHouse = PhotonNetwork.Instantiate("pfHouseRed", new Vector3(-3932, 50.5999985f, 4119.8999f), Quaternion.Euler(new Vector3(270, 103.319969f, 0)));
        newHouse = PhotonNetwork.Instantiate("pfHouseRed", new Vector3(-2579.80005f, 137.400009f, -1840.3999f), Quaternion.Euler(new Vector3(270, 20.2657928f, 0)));
        newHouse = PhotonNetwork.Instantiate("pfHouseRed", new Vector3(-2220, 143.800003f, 3131.5f), Quaternion.Euler(new Vector3(270, 172.424957f, 0)));
        newHouse = PhotonNetwork.Instantiate("pfHouseRed", new Vector3(-837, 32.2000008f, -430), Quaternion.Euler(new Vector3(270, 353.540924f, 0)));
        newHouse = PhotonNetwork.Instantiate("pfHouseRed", new Vector3(-2479.19995f, 120.389999f, 1402), Quaternion.Euler(new Vector3(270, 20.2657928f, 0)));
        newHouse = PhotonNetwork.Instantiate("pfHouseRed", new Vector3(-2400.1001f, 120.599998f, 1409.09998f), Quaternion.Euler(new Vector3(270, 20.2657928f, 0)));
        newHouse = PhotonNetwork.Instantiate("pfHouseRed", new Vector3(-3068, 148.700012f, -1203.5f), Quaternion.Euler(new Vector3(270, 262.865021f, 0)));

        newHouse = PhotonNetwork.Instantiate("pfHouseRed", new Vector3(-2407.57471f, 270.76004f, -484.870117f), Quaternion.Euler(new Vector3(270, 292.92981f, 0)));
        newHouse = PhotonNetwork.Instantiate("pfHouseRed", new Vector3(-4640.2998f, 16.7000732f, 2085.99951f), Quaternion.Euler(new Vector3(270, 262.865021f, 0)));
        newHouse = PhotonNetwork.Instantiate("pfHouseRed", new Vector3(-3976.1001f, 153, -3545.6001f), Quaternion.Euler(new Vector3(270, 20.2657928f, 0)));
        newHouse = PhotonNetwork.Instantiate("pfHouseRed", new Vector3(-4635.2998f, 68.0999985f, -312.100006f), Quaternion.Euler(new Vector3(270, 20.2657928f, 0)));
        newHouse = PhotonNetwork.Instantiate("pfHouseRed", new Vector3(-217.400665f, 382.5f, -3186.16187f), Quaternion.Euler(new Vector3(270, 154.555038f, 0)));
        newHouse = PhotonNetwork.Instantiate("pfHouseRed", new Vector3(290, 11.3000002f, -4260.8999f), Quaternion.Euler(new Vector3(270, 226.723221f, 0)));
        newHouse = PhotonNetwork.Instantiate("pfHouseRed", new Vector3(-3489.53882f, 124.5f, 2830.57788f), Quaternion.Euler(new Vector3(270, 229.148788f, 0)));
        newHouse = PhotonNetwork.Instantiate("pfHouseRed", new Vector3(-4604.48242f, 11.6999998f, -4501.58887f), Quaternion.Euler(new Vector3(270, 224.676346f, 0)));
        newHouse = PhotonNetwork.Instantiate("pfHouseRed", new Vector3(-3455, 148.700012f, 301.100006f), Quaternion.Euler(new Vector3(270, 262.865021f, 0)));
        newHouse = PhotonNetwork.Instantiate("pfHouseRed", new Vector3(-1126, 79, 1466.80005f), Quaternion.Euler(new Vector3(270, 20.2657928f, 0)));

        newHouse = PhotonNetwork.Instantiate("pfHouseRed", new Vector3(-1177.47241f, 90.999939f, 2434.22681f), Quaternion.Euler(new Vector3(270, 143.650299f, 0)));
        newHouse = PhotonNetwork.Instantiate("pfHouseRed", new Vector3(-3653.04956f, 141.699997f, 1247.26892f), Quaternion.Euler(new Vector3(270, 320.830048f, 0)));

        newHouse = PhotonNetwork.Instantiate("pfHouseBlue", new Vector3(4076.75f, 108.400002f, -3337.69995f), Quaternion.Euler(new Vector3(270, 194.062973f, 0)));
        newHouse = PhotonNetwork.Instantiate("pfHouseBlue", new Vector3(3796.20044f, 141.98999f, -816.100098f), Quaternion.Euler(new Vector3(270, 0, 0)));
        newHouse = PhotonNetwork.Instantiate("pfHouseBlue", new Vector3(4739, 15.6999998f, 3737.8999f), Quaternion.Euler(new Vector3(274.586731f, 90.0000687f, 87.3395462f)));
        newHouse = PhotonNetwork.Instantiate("pfHouseBlue", new Vector3(2008.69995f, 160, 2276.69995f), Quaternion.Euler(new Vector3(274.586731f, 205.586578f, 87.3396606f)));
        newHouse = PhotonNetwork.Instantiate("pfHouseBlue", new Vector3(1899.59998f, 160, 2292.8999f), Quaternion.Euler(new Vector3(274.586731f, 176.426041f, 87.3395004f)));
        newHouse = PhotonNetwork.Instantiate("pfHouseBlue", new Vector3(2213, 203.800003f, 4125), Quaternion.Euler(new Vector3(270, 49.0859413f, 0)));
        newHouse = PhotonNetwork.Instantiate("pfHouseBlue", new Vector3(2991.21997f, 135.899963f, 337.950012f), Quaternion.Euler(new Vector3(270, 25.7166252f, 0)));
        newHouse = PhotonNetwork.Instantiate("pfHouseBlue", new Vector3(3273.80908f, 136.400024f, -838.261536f), Quaternion.Euler(new Vector3(270, 347.504272f, 0)));
        newHouse = PhotonNetwork.Instantiate("pfHouseBlue", new Vector3(1423.00146f, 150.800003f, 4431.50586f), Quaternion.Euler(new Vector3(270, 317.837189f, 0)));
        newHouse = PhotonNetwork.Instantiate("pfHouseBlue", new Vector3(2351.64526f, 250.039993f, 3314.51221f), Quaternion.Euler(new Vector3(271.947937f, 101.02549f, 278.814087f)));

        newHouse = PhotonNetwork.Instantiate("pfHouseBlue", new Vector3(4400.11133f, 143.699997f, 543.797729f), Quaternion.Euler(new Vector3(271.947815f, 84.8925095f, 278.815002f)));
        newHouse = PhotonNetwork.Instantiate("pfHouseBlue", new Vector3(2062.6001f, 218.100006f, 1038.90002f), Quaternion.Euler(new Vector3(274.586731f, 205.586578f, 87.3396606f)));
        newHouse = PhotonNetwork.Instantiate("pfHouseBlue", new Vector3(1534.37f, 77, -300), Quaternion.Euler(new Vector3(274.586761f, 230.477814f, 87.3396301f)));
        newHouse = PhotonNetwork.Instantiate("pfHouseBlue", new Vector3(2093.5f, 18.3999996f, -1879.69995f), Quaternion.Euler(new Vector3(270, 160.448608f, 0)));
        newHouse = PhotonNetwork.Instantiate("pfHouseBlue", new Vector3(3051.34814f, 154.600006f, 3591.63574f), Quaternion.Euler(new Vector3(274.58667f, 118.938622f, 87.3390198f)));
        newHouse = PhotonNetwork.Instantiate("pfHouseBlue", new Vector3(-51.9671097f, 30, 4139.47314f), Quaternion.Euler(new Vector3(270, 98.0973587f, 0)));
        newHouse = PhotonNetwork.Instantiate("pfHouseBlue", new Vector3(2416.37256f, 16.3999996f, -1067.55005f), Quaternion.Euler(new Vector3(270, 215.599991f, 0)));
        newHouse = PhotonNetwork.Instantiate("pfHouseBlue", new Vector3(1514.40002f, 77.5f, -1383.5f), Quaternion.Euler(new Vector3(274.586731f, 230.477905f, 337.505005f)));
        newHouse = PhotonNetwork.Instantiate("pfHouseBlue", new Vector3(1780.30005f, 137.100006f, -738.900024f), Quaternion.Euler(new Vector3(271.856049f, 278.882538f, 289.155945f)));
        newHouse = PhotonNetwork.Instantiate("pfHouseBlue", new Vector3(4086.74731f, 143.732773f, 2087.88062f), Quaternion.Euler(new Vector3(271.947937f, 84.8928833f, 52.1278915f)));

        newHouse = PhotonNetwork.Instantiate("pfHouseBlue", new Vector3(1198.89478f, 132.235504f, 2707.38403f), Quaternion.Euler(new Vector3(274.58667f, 176.426086f, 8.44570065f)));
        newHouse = PhotonNetwork.Instantiate("pfHouseBlue", new Vector3(3343.5f, 68.5f, 4862.7002f), Quaternion.Euler(new Vector3(270, 233.804535f, 0)));
    }

    private void CreatePlayerBlue()
    {
        ownPlayer = PhotonNetwork.Instantiate("pfPlayer", GameObject.Find("/AirPlaneSpawnPositionBlue").transform.position, Quaternion.identity);
        ownPlayer.name = "player_blue";
        scriptOwnPlayer = ownPlayer.GetComponent<Player>();
        scriptOwnPlayer.ColliderLandingZone = GameObject.Find("/AirportBlue/LandingZone").GetComponent<Collider>();
        scriptOwnPlayer.AirplaneSpawnPosition = GameObject.Find("/AirPlaneSpawnPositionBlue").transform;
        scriptOwnPlayer.PfStaticAirplane = pfStaticAirplaneBlue;
        scriptOwnPlayer.IsRedPlane = false;
        scriptOwnPlayer.PhotonView.RPC("ChangePlaneColor", RpcTarget.All, scriptOwnPlayer.PhotonView.ViewID, "blue");
        playerBlue = ownPlayer;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameState.Equals(GameState_.JoiningNetworkGame))
        {
            return;
        }
        if (GameState.Equals(GameState_.WaitingForSecondPlayer))
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
        if (GameState.Equals(GameState_.WaitingForSecondPlayerStats))
        {
            if (otherPlayer.GetComponent<Player>().PlayerStatsReceived)
            {
                SetGameState(GameState_.GameOver);
            }
        }

        if (otherPlayer == null)
        {
            FindOtherPlayer();
        }

        if (playerRed != null)
        {
            healthBarRed.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, playerRed.GetComponent<Player>().Health * 1.57f);

            // 0,-14..221,-236 <-> -5000,5000..5000,-5000
            // 0,0..130,-120 <-> -5000,5000..5000,-5000
            float minimapXPos = (float)(130 * (playerRed.transform.position.x + 5000) / 10000.0);
            float minimapYPos = (float)(-120 + 120 * (playerRed.transform.position.z + 5000) / 10000.0);
            minimapAirplaneRed.anchoredPosition = new Vector3(minimapXPos, minimapYPos, 0);
            minimapAirplaneRed.rotation = Quaternion.Euler(0, 0, -playerRed.transform.rotation.eulerAngles.y);
        }
        if (playerBlue != null)
        {
            healthBarBlue.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, playerBlue.GetComponent<Player>().Health * 1.57f);

            float minimapXPos = (float)(130 * (playerBlue.transform.position.x + 5000) / 10000.0);
            float minimapYPos = (float)(-120 + 120 * (playerBlue.transform.position.z + 5000) / 10000.0);
            minimapAirplaneBlue.anchoredPosition = new Vector3(minimapXPos, minimapYPos, 0);
            minimapAirplaneBlue.rotation = Quaternion.Euler(0, 0, -playerBlue.transform.rotation.eulerAngles.y);
        }
    }

    public void JoinNetworkGame()
    {
        if( PhotonNetwork.CurrentRoom.PlayerCount==1 )
        {
            minimapAirplaneBlue.gameObject.SetActive(false);
            SpawnHouses();
            CreatePlayerRed();
            cameraTransform.GetComponent<CinemachineFreeLook>().Follow = ownPlayer.transform;
            cameraTransform.GetComponent<CinemachineFreeLook>().LookAt = ownPlayer.transform;
            SetGameState(GameState_.WaitingForSecondPlayer);
        }
        else
        {
            minimapAirplaneBlue.gameObject.SetActive(true);
            CreatePlayerBlue();
            cameraTransform.GetComponent<CinemachineFreeLook>().Follow = ownPlayer.transform;
            cameraTransform.GetComponent<CinemachineFreeLook>().LookAt = ownPlayer.transform;
            // This is the second player, so the first should start the game (and remove existing items)
            scriptOwnPlayer.PhotonView.RPC("StartGame", RpcTarget.Others, scriptOwnPlayer.PhotonView.ViewID);
        }

    }

    private void SetGameOverInfo()
    {
        GameStats.HealthRed = playerRed.GetComponent<Player>().Health;
        GameStats.HealthBlue = playerBlue.GetComponent<Player>().Health;

        if (GameStats.HealthRed != GameStats.HealthBlue)
        {
            GameStats.GameOverReason = "Because the base is in better condition than the enemy";
        }
        else if (playerBlue.GetComponent<Player>().NumAirplanes == 0 || playerRed.GetComponent<Player>().NumAirplanes == 0)
        {
            GameStats.GameOverReason = "Because the enemy has no airplanes left";
        }
        else
        {
            GameStats.GameOverReason = "Because he destroyed the enemy base";
        }

        if (playerBlue.GetComponent<Player>().NumAirplanes == 0 || GameStats.HealthBlue == 0 || GameStats.HealthRed > GameStats.HealthBlue)
        {
            GameStats.PlayerWon = 0;
        }
        else if (playerRed.GetComponent<Player>().NumAirplanes == 0 || GameStats.HealthRed == 0 || GameStats.HealthBlue > GameStats.HealthRed)
        {
            GameStats.PlayerWon = 1;
        }
        else
        {
            GameStats.PlayerWon = 2;
            GameStats.GameOverReason = "";
        }
        GameStats.DistanceBlue = playerBlue.GetComponent<Player>().DistanceTravelled / 1000.0f;
        GameStats.DistanceRed = playerRed.GetComponent<Player>().DistanceTravelled / 1000.0f;
    }

    private void FindOtherPlayer()
    {
        var players = FindObjectsOfType<Player>();

        foreach (Player player in players)
        {
            if (!player.PhotonView.IsMine)
            {
                otherPlayer = player.gameObject;
                if (scriptOwnPlayer.IsRedPlane)
                {
                    playerBlue = otherPlayer;
                }
                else
                {
                    playerRed = otherPlayer;
                }
            }
        }
    }

    public void SetGameState(GameState_ newGameState)
    {
        gameState = newGameState;

        switch (newGameState)
        {
            case GameState_.JoiningNetworkGame:
                canvasConnecting.enabled = true;
                break;
            case GameState_.WaitingForSecondPlayer:
                scriptOwnPlayer.ResetGame();
                canvasConnecting.enabled = false;
                panelStatus.SetActive(false);
                break;
            case GameState_.Playing:
                scriptOwnPlayer.ResetGame();
                otherPlayer = null;
                canvasConnecting.enabled = false;
                textTryOutMode.enabled = false;
                panelStatus.SetActive(true);
                gameTimer.InitTimer();
                break;
            case GameState_.WaitingForSecondPlayerStats:
                scriptOwnPlayer.PhotonView.RPC("SetPlayerStats", RpcTarget.Others, scriptOwnPlayer.PhotonView.ViewID, scriptOwnPlayer.DistanceTravelled);
                break;
            case GameState_.GameOver:
                SetGameOverInfo();
                SceneManager.LoadSceneAsync("GameOver");
                break;

        }
    }

    public void OnExitGameClick()
    {
        Application.Quit();
    }
}

