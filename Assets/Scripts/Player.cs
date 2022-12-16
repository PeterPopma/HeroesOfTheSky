using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;
using Cinemachine;
using Photon.Pun;

namespace AirplaneGame
{
    public class Player : MonoBehaviour
    {
        private const float MINIMUM_FLY_SPEED = 30;

        [Header("Rotating speeds")]
        [Range(5f, 500f)]
        [SerializeField] private float yawSpeed = 50f;

        [Range(5f, 500f)]
        [SerializeField] private float pitchSpeed = 100f;

        [Range(5f, 500f)]
        [SerializeField] private float rollSpeed = 200f;

        [Header("Acceleration / Deceleration")]
        [Range(0.1f, 150f)]
        [SerializeField] private float throttleAcceleration = 0.05f;
        [Range(0.1f, 150f)]
        [SerializeField] private float speedAcceleration = 0.05f;

        [Header("Engine propellor settings")]
        [Range(10f, 10000f)]
        [SerializeField] private float propelSpeedMultiplier = 100f;

        [SerializeField] private GameObject[] propellors;

        [SerializeField] private GameObject pfMissile;
        [SerializeField] private GameObject pfBomb;
        [SerializeField] private GameObject pfBullet;
        [SerializeField] private Transform[] spawnPositionMissile = new Transform[2];
        [SerializeField] private Transform[] spawnPositionBullet = new Transform[2];
        [SerializeField] private Transform spawnPositionBomb;
        [SerializeField] private Transform spawnPositionSmoke;
        [SerializeField] private Transform crossHair;

        [SerializeField] private Transform vfxShootRocket;
        [SerializeField] private Transform vfxCrash;
        [SerializeField] private Transform vfxSmoke;

        [SerializeField] private AudioSource soundEngine;
        [SerializeField] private AudioSource soundMissile;
        [SerializeField] private AudioSource soundCrash;
        [SerializeField] private AudioSource soundBombDrop;
        [SerializeField] private AudioSource soundGun;
        [SerializeField] private AudioSource soundSwitchWeapon;
        [SerializeField] private AudioSource soundWind;
        [SerializeField] private AudioSource soundPowerUp;

        [SerializeField] private LayerMask gunfireLayerMask;

        private float currentSpeed;
        private float throttle;
        private bool isCrashed;
        private bool isRedPlane;

        private Transform airplaneSpawnPosition;
        private Transform pfStaticAirplane;

        private Collider colliderAirplaneBody;
        private Collider colliderLandingZone;

        private List<Transform> planesLeft = new List<Transform>();

        private Transform activeWeaponGun;
        private Transform activeWeaponMissile;
        private Transform activeWeaponBomb;

        private TextMeshProUGUI textSpeed;
        private TextMeshProUGUI textAltitude;
        private TextMeshProUGUI textBombs;
        private TextMeshProUGUI textMissiles;
        private TextMeshProUGUI textAirplanes;
        private TextMeshProUGUI textWind;
        private TextMeshProUGUI textResupplied;

        private PhotonView photonView;

        private RectTransform windNeedle;
        private RectTransform compassNeedle;
        private RectTransform fuelNeedle;
        private RectTransform fuelLight;
        private RectTransform throttleBar;

        private GameObject[] missile = new GameObject[2];
        int currentMissile;
        int currentBullet;
        float[] timeMissileFired = new float[2];

        bool bombDropping;
        private GameObject bomb;
        float timeGunFired;
        float timeCrashed;
        float timeLeftShowResupplied;

        float windSpeed = 2.5f;     // speed 0..5
        float windSpeedChange;      // -0.1 .. 0.1
        float windDirection;        // 0..360
        float windDirectionChange;  // -0.1 .. 0.1
        float timeWindChanged;
        private Vector3 speed;

        float minimapPositionX;
        float minimapPositionY;
        float minimapRotationY;

        private int currentWeapon;          // 0=gun, 1=missile, 2=bomb

        private Game scriptGame;

        private float altitude, previousAltitude;
        private float heightAboveGround;

        Vector2 movement;
        private float movementX;
        private float movementY;
        bool buttonAccelerate;
        bool buttonDecelerate;
        bool buttonFire;
        bool buttonSwitchWeapon;
        bool buttonYawLeft;
        bool buttonYawRight;
        bool buttonRestart;
        bool buttonHelp;
        bool isLanded;
        private float smokeAmount;
        private float smokeAmountLimit = 0;

        int numAirplanes;
        int numMissiles;
        int numBombs;
        float amountFuel;           // fuel 0..100
        float distanceTravelled;
        int health;                 // health of the base

        public AudioSource SoundBombDrop { get => soundBombDrop; set => soundBombDrop = value; }
        public Vector3 Speed { get => speed; set => speed = value; }
        public bool BombDropping { get => bombDropping; set => bombDropping = value; }
        public bool IsRedPlane { get => isRedPlane; set => isRedPlane = value; }
        public int NumAirplanes { get => numAirplanes; set => numAirplanes = value; }
        public float CurrentSpeed { get => currentSpeed; set => currentSpeed = value; }
        public Collider ColliderLandingZone { get => colliderLandingZone; set => colliderLandingZone = value; }

        public Transform AirplaneSpawnPosition { get => airplaneSpawnPosition; set => airplaneSpawnPosition = value; }
        public Transform PfStaticAirplane { get => pfStaticAirplane; set => pfStaticAirplane = value; }
        public float DistanceTravelled { get => distanceTravelled; set => distanceTravelled = value; }
        public float MinimapPositionX { get => minimapPositionX; set => minimapPositionX = value; }
        public float MinimapPositionY { get => minimapPositionY; set => minimapPositionY = value; }
        public float MinimapRotationY { get => minimapRotationY; set => minimapRotationY = value; }
        public PhotonView PhotonView { get => photonView; set => photonView = value; }
        public int Health { get => health; set => health = value; }

        private void Awake()
        {
            photonView = GetComponent<PhotonView>();
            scriptGame = GameObject.Find("/Scripts/Game").GetComponent<Game>();
            if (photonView.IsMine)
            {
                activeWeaponGun = GameObject.Find("/Canvas/Weapon/Gun").transform;
                activeWeaponMissile = GameObject.Find("/Canvas/Weapon/Missile").transform;
                activeWeaponBomb = GameObject.Find("/Canvas/Weapon/Bomb").transform;
                textSpeed = GameObject.Find("/Canvas/PanelTop/Speed").GetComponent<TextMeshProUGUI>();
                textAltitude = GameObject.Find("/Canvas/PanelTop/Altitude").GetComponent<TextMeshProUGUI>();
                textBombs = GameObject.Find("/Canvas/PanelTop/TextBombs").GetComponent<TextMeshProUGUI>();
                textMissiles = GameObject.Find("/Canvas/PanelTop/TextMissiles").GetComponent<TextMeshProUGUI>();
                textAirplanes = GameObject.Find("/Canvas/PanelTop/TextPlanes").GetComponent<TextMeshProUGUI>();
                textWind = GameObject.Find("/Canvas/Compass/TextWind").GetComponent<TextMeshProUGUI>();
                windNeedle = GameObject.Find("/Canvas/Compass/WindNeedle").GetComponent<RectTransform>();
                compassNeedle = GameObject.Find("/Canvas/Compass/Needle").GetComponent<RectTransform>();
                fuelNeedle = GameObject.Find("/Canvas/Fuel/FuelNeedle").GetComponent<RectTransform>();
                fuelLight = GameObject.Find("/Canvas/Fuel/FuelLight").GetComponent<RectTransform>();
                throttleBar = GameObject.Find("/Canvas/PanelTop/Throttle/ThrottleBar").GetComponent<RectTransform>();
                colliderAirplaneBody = transform.Find("Colliders/Body").GetComponent<Collider>();
                textResupplied = GameObject.Find("/Canvas/TextResupplied").GetComponent<TextMeshProUGUI>();
            }
            else
            {
                GetComponent<PlayerInput>().actions.Disable();
            }
        }

        private void Start()
        {
            if (!photonView.IsMine)
            {
                return;
            }

            // take control of the user input by disabling all and then enabling mine
            /*
             var players = FindObjectsOfType<Player>();
            foreach (Player player in players)
            {
                player.GetComponent<PlayerInput>().enabled = false;
            }
            GetComponent<PlayerInput>().enabled = true;
            */
            if (Settings.WindStrength==0)
            {
                textWind.enabled = false;
                windNeedle.gameObject.SetActive(false);
            }

            textResupplied.enabled = false;

            CreateMissile(0);
            CreateMissile(1);
            CreateBomb();
        }

        [PunRPC]
        public void SetPlayerStats(int viewID, float distanceOther, int healthOther, int numAirplanesOther)
        {
            if (photonView.ViewID == viewID)
            {
                scriptGame.SetGameOverInfo(distanceOther, healthOther, numAirplanesOther);
            }
        }


        [PunRPC]
        public void EndGame(int viewID)
        {
            if (photonView.ViewID == viewID)
            {
                scriptGame.SetGameState(GameState_.CollectingGameStatistics);
            }
        }

        [PunRPC]
        public void StartGameBluePlayer(int viewID)
        {
            if (photonView.ViewID == viewID)
            {
                scriptGame.SetGameState(GameState_.Playing);
            }
        }

        [PunRPC]
        public void StartGameRedPlayer(int viewID)
        {
            if (photonView.ViewID == viewID)
            {
                scriptGame.SpawnHouses();
                scriptGame.PowerUpSpawner.SpawnPowerUps();
                scriptGame.SetGameState(GameState_.Playing);
                photonView.RPC("StartGameBluePlayer", RpcTarget.Others, photonView.ViewID);
            }
        }

        [PunRPC]
        public void ChangePlaneColor(int viewID, string colorName)
        {
            if (photonView.ViewID == viewID)
            {
                GameObject planeRoot = transform.Find("Plane").gameObject;
                planeRoot.GetComponent<MeshRenderer>().material = Resources.Load("base_" + colorName, typeof(Material)) as Material;
                GameObjectsIterator.IterateChildren(planeRoot, 
                    delegate (GameObject gameObject) 
                        { 
                            MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
                            if (renderer != null && renderer.material.name.StartsWith("base"))
                            {
                                renderer.material = Resources.Load("base_" + colorName, typeof(Material)) as Material;
                            };
                        } );
            }
        }

        [PunRPC]
        public void PlayerShotDown(int viewID)
        {
            if (photonView.ViewID == viewID)
            {
                Crash();
            }
        }

        [PunRPC]
        public void SetHealth(int viewID, int health)
        {
            if (photonView.ViewID == viewID)
            {
                this.health = health;
            }
        }

        [PunRPC]
        public void SetNumAirplanes(int viewID, int numPlanes, bool isRed)
        {
            if (photonView.ViewID == viewID)
            {
                numAirplanes = numPlanes;
                foreach (Transform plane in planesLeft)
                {
                    Destroy(plane.gameObject);
                }
                planesLeft.Clear();
                for (int i = 0; i < numAirplanes - 1; i++)
                {
                    Transform plane;
                    if (isRed)
                    {
                        plane = Instantiate(scriptGame.PfStaticAirplaneRed);
                        plane.position = new Vector3(scriptGame.StaticAirplaneSpawnPositionRed.position.x + 15 * i, scriptGame.StaticAirplaneSpawnPositionRed.position.y, scriptGame.StaticAirplaneSpawnPositionRed.position.z);
                        plane.rotation = scriptGame.StaticAirplaneSpawnPositionRed.rotation;
                    }
                    else
                    {
                        plane = Instantiate(scriptGame.PfStaticAirplaneBlue);
                        plane.position = new Vector3(scriptGame.StaticAirplaneSpawnPositionBlue.position.x, scriptGame.StaticAirplaneSpawnPositionBlue.position.y, scriptGame.StaticAirplaneSpawnPositionBlue.position.z - 15 * i);
                        plane.rotation = scriptGame.StaticAirplaneSpawnPositionBlue.rotation;
                    }
                    plane.parent = GameObject.Find("ExtraPlanes").transform;
                    planesLeft.Add(plane);
                }
            }
        }

        public void DecreaseHealth()
        {
            health -= 5;
            photonView.RPC("SetHealth", RpcTarget.All, photonView.ViewID, health);
        }

        private void OnRestart(InputValue value)
        {
            buttonRestart = value.isPressed;
        }

        private void OnFire(InputValue value)
        {
            buttonFire = value.isPressed;
        }

        private void OnSwitchWeapon(InputValue value)
        {
            buttonSwitchWeapon = value.isPressed;
        }

        private void UpdateWind()
        {
            windSpeed += windSpeedChange * Time.deltaTime;
            if(windSpeed < 0)
            {
                windSpeed = 0;
            }
            if (windSpeed > 9.9)
            {
                windSpeed = 9.9f;
            }
            windDirection += windDirectionChange * Time.deltaTime;
            while(windDirection<0)
            {
                windDirection += 360;
            }
            windDirection %= 360;

            if (Time.time - timeWindChanged > 4)
            {
                timeWindChanged = Time.time;
                windSpeedChange = Random.value * 1 - 0.5f;
                windDirectionChange = Random.value * 40 - 20;     // -10..10
            }

            textWind.text = "Wind: " + windSpeed.ToString("0.0");
            windNeedle.rotation = Quaternion.Euler(0, 0, windDirection);
            soundWind.volume = windSpeed / 10.0f;
        }

        private void CreateMissile(int missileNumber)
        {
            missile[missileNumber] = Instantiate(pfMissile, spawnPositionMissile[missileNumber].position, Quaternion.LookRotation(transform.forward, Vector3.up));
            missile[missileNumber].GetComponent<Missile>().OwnerIsRed = isRedPlane;
            missile[missileNumber].name = "Missile-" + (missileNumber == 0 ? "Left" : "Right");
            missile[missileNumber].transform.parent = transform;
        }

        public void CreateBomb()
        {
            bomb = Instantiate(pfBomb, spawnPositionBomb.position, Quaternion.LookRotation(transform.forward, Vector3.up));
            bomb.GetComponent<Bomb>().SetPlayerScript(this);
            bomb.transform.parent = transform;
        }

        private void DropBomb()
        {
            bombDropping = true;
            soundBombDrop.Play();
            numBombs--;
            bomb.GetComponent<Bomb>().Activate(this);
            textBombs.text = numBombs.ToString();
        }

        private void FireGun()
        {
            soundGun.Play();
            timeGunFired = Time.time;
            Vector3 aimDirection = (crossHair.position - spawnPositionBullet[currentBullet].position).normalized;
            GameObject newBullet = Instantiate(pfBullet, spawnPositionBullet[currentBullet].position, Quaternion.LookRotation(aimDirection, Vector3.up));
//            newBullet.GetComponent<Bullet>().Initialize(aimDirection);
            currentBullet++;
            if (currentBullet > 1)
            {
                currentBullet = 0;
            }

            if (Physics.Raycast(spawnPositionBullet[currentBullet].position, transform.TransformDirection(Vector3.forward), out RaycastHit raycastHit, 800f, gunfireLayerMask))
            {
                // Only player is in the mask, so this means a player was hit
                Transform hitTransForm = raycastHit.transform;
                // Don't hit own plane
                if ((hitTransForm.parent.parent.name.Equals("player_blue") && isRedPlane == false) || 
                    (hitTransForm.parent.parent.name.Equals("player_red") && isRedPlane == true))
                {
                    return;
                }
                photonView.RPC("PlayerShotDown", RpcTarget.All, scriptGame.OtherPlayer.GetComponent<Player>().PhotonView.ViewID);
            }
        }

        private void FireMissile()
        {
            if (missile[currentMissile].GetComponent<Missile>().IsMoving == false)
            {
                timeMissileFired[currentMissile] = Time.time;
                missile[currentMissile].GetComponent<Missile>().IsMoving = true;
                missile[currentMissile].transform.SetParent(null);
                soundMissile.Play();
                Transform newFX = Instantiate(vfxShootRocket, spawnPositionMissile[currentMissile].position, Quaternion.identity);
                newFX.parent = GameObject.Find("/vFX").transform;
                numMissiles--;
                textMissiles.text = numMissiles.ToString();
            }

            currentMissile++;
            if (currentMissile > 1)
            {
                currentMissile = 0;
            }
        }

        private void UpdateFiring()
        {
            if (buttonSwitchWeapon)
            {
                soundSwitchWeapon.Play();
                buttonSwitchWeapon = false;
                currentWeapon++;
                if (currentWeapon == 1)
                {
                    activeWeaponGun.gameObject.SetActive(false);
                    activeWeaponMissile.gameObject.SetActive(true);
                    activeWeaponBomb.gameObject.SetActive(false);
                }
                else if (currentWeapon == 2)
                {
                    activeWeaponGun.gameObject.SetActive(false);
                    activeWeaponMissile.gameObject.SetActive(false);
                    activeWeaponBomb.gameObject.SetActive(true);
                }
                else
                {
                    currentWeapon = 0;
                    activeWeaponGun.gameObject.SetActive(true);
                    activeWeaponMissile.gameObject.SetActive(false);
                    activeWeaponBomb.gameObject.SetActive(false);
                }
            }

            if (buttonFire && scriptGame.TimeLeftShowStarted<=0)
            {
                if (currentWeapon == 0)
                {
                    if (Time.time - timeGunFired > 0.1)
                    {
                        FireGun();
                    }
                }
                else if (currentWeapon == 1)
                {
                    buttonFire = false;
                    FireMissile();
                }
                if (currentWeapon == 2 && numBombs>0)
                {
                    buttonFire = false;
                    if (!bombDropping)
                    {
                        DropBomb();
                    }
                }
            }

            // Spawn new missile
            for (int missile_number = 0; missile_number < 2; missile_number++)
            {
                if (numMissiles>1 && (missile[missile_number] == null || missile[missile_number].GetComponent<Missile>().IsMoving == true))
                {
                    if (Time.time - timeMissileFired[missile_number] > 1)
                    {
                        CreateMissile(missile_number);
                    }
                }
            }
        }

        public void AddMissiles()
        {
            soundPowerUp.Play();
            numMissiles += 3;
            textMissiles.text = numMissiles.ToString();
        }

        public void AddBombs()
        {
            soundPowerUp.Play();
            numBombs++;
            textBombs.text = numBombs.ToString();
        }
        
        public void AddFuel()
        {
            soundPowerUp.Play();
            amountFuel += 10;
            if(amountFuel > 100)
            {
                amountFuel = 100;
            }
        }

        private void Update()
        {
            if (!photonView.IsMine)
            {
                return;
            }
            if (!scriptGame.GameState.Equals(GameState_.Playing) && !scriptGame.GameState.Equals(GameState_.WaitingForSecondPlayer))
            {
                return;
            }

            if (buttonHelp)
            {
                buttonHelp = false;
                Canvas canvasHelp = GameObject.Find("CanvasHelp").GetComponent<Canvas>();
                canvasHelp.enabled = !canvasHelp.enabled;
            }

            if (timeLeftShowResupplied>0)
            {
                timeLeftShowResupplied -= Time.deltaTime;
                if (timeLeftShowResupplied<=0)
                {
                    textResupplied.enabled = false;
                }
            }

            if (Settings.WindStrength > 0)
            {
                UpdateWind();
            }

            previousAltitude = altitude;
            altitude = spawnPositionBomb.position.y;
            textAltitude.text = "Alt: " + altitude.ToString("0");
            heightAboveGround = transform.position.y-Terrain.activeTerrain.SampleHeight(transform.position);
            compassNeedle.rotation = Quaternion.Euler(0, 0, -transform.rotation.eulerAngles.y);
            amountFuel -= throttle * 0.001f * Time.deltaTime;
            fuelNeedle.rotation = Quaternion.Euler(0, 0, 132 - 2.64f * amountFuel);
            if (amountFuel < 10 && Time.time%2<1)
            {
                fuelLight.gameObject.SetActive(true);
            }
            else
            {
                fuelLight.gameObject.SetActive(false);
            }

            // Airplane move only if not dead
            if (!isCrashed)
            {
                Movement();
                UpdateFiring();
                if (currentSpeed < MINIMUM_FLY_SPEED)
                {
                    GetComponent<Rigidbody>().useGravity = true;
                    GetComponent<Rigidbody>().isKinematic = false;
                }
                else
                {
                    GetComponent<Rigidbody>().useGravity = false;
                    GetComponent<Rigidbody>().isKinematic = true;
                }

                smokeAmount += throttle * Time.deltaTime * Random.value;
                if (smokeAmount > smokeAmountLimit)
                {
                    smokeAmountLimit = Random.value * 25;
                    smokeAmount = 0;
                    Instantiate(vfxSmoke, spawnPositionSmoke.position, Quaternion.identity); 
                }

                // Rotate propellers if any
                if (propellors.Length > 0)
                {
                    RotatePropellors(propellors);
                }

                if (amountFuel<=0)
                {
                    throttle = 0;
                    throttleBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, throttle);

                    if (speed.magnitude < 2)
                    {
                        Crash();
                    }
                }

                bool nowLanded = heightAboveGround < 3 && speed.magnitude < 5 && colliderAirplaneBody.bounds.Intersects(colliderLandingZone.bounds);
                if (!isLanded && nowLanded)
                {
                    ReplenishPlane();
                }
                isLanded = nowLanded;
            }
            else
            {
                if (!scriptGame.GameState.Equals(GameState_.GameOver) && Time.time - timeCrashed > 4)
                {
                    ResetAfterCrash();
                }
            }

            if (scriptGame.GameState.Equals(GameState_.GameOver) && buttonRestart)
            {
                SceneManager.LoadSceneAsync("GameMenu");
            }
        }

        public void ResetGame()
        {
            soundWind.volume = 0.0f;
            distanceTravelled = 0;
            photonView.RPC("SetNumAirplanes", RpcTarget.All, photonView.ViewID, 5, isRedPlane);
            photonView.RPC("SetHealth", RpcTarget.All, photonView.ViewID, 100);
            ResetAfterCrash();
        }

        private void ReplenishPlane()
        {
            textResupplied.enabled = true;
            timeLeftShowResupplied = 3;

            if (numMissiles<10)
            {
                numMissiles = 10;
                textMissiles.text = numMissiles.ToString();
            }
            if (numBombs < 3)
            {
                numBombs = 3;
                textBombs.text = numBombs.ToString();
            }
            if (amountFuel < 80)
            {
                amountFuel = 80;
            }
            transform.SetPositionAndRotation(airplaneSpawnPosition.position, airplaneSpawnPosition.rotation);
        }

        private void ResetAfterCrash()
        {
            isLanded = true;
            bombDropping = false;
            timeMissileFired[0] = timeMissileFired[1] = Time.time;
            throttle = currentSpeed = 0;
            throttleBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, throttle);

            fuelLight.gameObject.SetActive(false);
            transform.SetPositionAndRotation(airplaneSpawnPosition.position, airplaneSpawnPosition.rotation);

            //            engineSoundSource.volume = 0.4f;

            isCrashed = false;
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Rigidbody>().useGravity = false;

            numMissiles = 10;
            numBombs = 3;
            amountFuel = 80;

            textMissiles.text = numMissiles.ToString();
            textBombs.text = numBombs.ToString();
            textAirplanes.text = numAirplanes.ToString();
        }

        private void OnYawLeft(InputValue value)
        {
            buttonYawLeft = value.isPressed;
        }

        private void OnYawRight(InputValue value)
        {
            buttonYawRight = value.isPressed;
        }
        
        private void OnAccelerate(InputValue value)
        {
            buttonAccelerate = value.isPressed;
        }

        private void OnDecelerate(InputValue value)
        {
            buttonDecelerate = value.isPressed;
        }

        private void OnMove(InputValue value)
        {
            movement = value.Get<Vector2>();
        }

        private void OnHelp(InputValue value)
        {
            buttonHelp = value.isPressed;
        }

        private void Movement()
        {
            Vector3 oldPosition = transform.position;

            // Move forward
            transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);

            if (currentSpeed > MINIMUM_FLY_SPEED)
            {
                float windChangeZ = Settings.WindStrength * Time.deltaTime * windSpeed * Mathf.Cos(windDirection * (Mathf.PI / 180));
                float windChangeX = Settings.WindStrength * Time.deltaTime * windSpeed * -Mathf.Sin(windDirection * (Mathf.PI / 180));
                transform.position += new Vector3(windChangeX, 0, windChangeZ);
            }

            if (movement.x == 1 || movement.x == -1)
            {
                movementX = movement.x;
            }
            else if (movementX!=0)
            {
                // slowly die out
                movementX *= Mathf.Pow(0.01f, Time.deltaTime);
                if (Mathf.Abs(movementX) < 0.01f)
                {
                    movementX = 0;
                }
            }

            if (movement.y == 1 || movement.y == -1)
            {
                movementY = movement.y;
            }
            else if (movementY != 0)
            {
                // slowly die out
                movementY *= Mathf.Pow(0.01f, Time.deltaTime);
                if (Mathf.Abs(movementY) < 0.01f)
                {
                    movementY = 0;
                }
            }

            // Rotate airplane by inputs
            if (currentSpeed > MINIMUM_FLY_SPEED)
            {
                transform.Rotate(Vector3.forward * -movementX * rollSpeed * Time.deltaTime);
                transform.Rotate(Vector3.right * movementY * pitchSpeed * Time.deltaTime);
            }

            // Move back to straight
            // TODO : not right yet
            /*
            if (transform.rotation.z < 0)
            {
                transform.Rotate(Vector3.forward * 125f * Time.deltaTime);
            }
            if (transform.rotation.z > 0)
            {
                transform.Rotate(Vector3.forward * -125f * Time.deltaTime);
            }
            
            Debug.Log("rotation:" + transform.rotation.x.ToString("0.00") + "," + transform.rotation.y.ToString("0.00") + "," + transform.rotation.z.ToString("0.00"));
             */


            // Rotate yaw
            if (buttonYawRight)
            {
                transform.Rotate(Vector3.up * yawSpeed * Time.deltaTime);
            }
            else if (buttonYawLeft)
            {
                transform.Rotate(-Vector3.up * yawSpeed * Time.deltaTime);
            }
            if (buttonAccelerate)
            {
                if (throttle < 100)
                {
                    throttle += throttleAcceleration * Time.deltaTime;
                    if (throttle > 100)
                    {
                        throttle = 100;
                    }
                    throttleBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, throttle);
                }
            }
            if (buttonDecelerate)
            {
                if (throttle > 0)
                {
                    throttle -= throttleAcceleration * Time.deltaTime;
                    if (throttle < 0)
                    {
                        throttle = 0;
                    }
                    throttleBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, throttle);
                }
            }

            float gainFromDescending = (previousAltitude - altitude) * Time.deltaTime * 2000;
            if (gainFromDescending > 100)
            {
                gainFromDescending = 100;
            }
            if (gainFromDescending < -100)
            {
                gainFromDescending = -100;
            }

            if (throttle + gainFromDescending > currentSpeed)
            {
                currentSpeed += speedAcceleration * Time.deltaTime;
            }

            if (throttle + gainFromDescending < currentSpeed)
            {
                currentSpeed -= speedAcceleration * Time.deltaTime;
            }
            distanceTravelled += (transform.position - oldPosition).magnitude;
            speed = (transform.position - oldPosition) / Time.deltaTime;
            textSpeed.text = "Speed: " + ((int)speed.magnitude).ToString();

            float pitch = speed.magnitude / 100;
            if (pitch < 0.8)
            {
                pitch = 0.8f;
            }
            soundEngine.pitch = pitch;
            soundEngine.volume = throttle / 100;
        }

        private void RotatePropellors(GameObject[] _rotateThese)
        {
            float _propelSpeed = throttle * propelSpeedMultiplier;

            for (int i = 0; i < _rotateThese.Length; i++)
            {
                _rotateThese[i].transform.Rotate(Vector3.forward * -_propelSpeed * Time.deltaTime);
            }
        }

        public void Crash()
        {
            if (isCrashed)
            {
                return;
            }

            // Set rigidbody to non-kinematic
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Rigidbody>().useGravity = true;

            timeCrashed = Time.time;
            isCrashed = true;
            soundEngine.volume = 0f;

            Instantiate(vfxCrash, transform.position, Quaternion.identity);
            soundCrash.Play();

            if (!scriptGame.GameState.Equals(GameState_.WaitingForSecondPlayer))
            {
                photonView.RPC("SetNumAirplanes", RpcTarget.All, photonView.ViewID, numAirplanes - 1, isRedPlane);
            }
            textAirplanes.text = numAirplanes.ToString();
            if (numAirplanes == 0)
            {
                photonView.RPC("EndGame", RpcTarget.All, photonView.ViewID);
            }
        }

        #region Variables


        #endregion
    }
}