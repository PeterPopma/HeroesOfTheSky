using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

namespace AirplaneGame
{
    public class Player : MonoBehaviour
    {
        private float currentSpeed;
        private float throttle;
        private bool isCrashed;
        private const float MINIMUM_FLY_SPEED = 30;
        private const float MINIMUM_DAMAGE_SPEED = 30;
        private const int WIND_STRENGTH = 15;

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

        [Header("Engine sound settings")]
        [SerializeField] private AudioSource engineSoundSource;

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
        [SerializeField] private Transform crossHair;

        [SerializeField] private Transform vfxShootRocket;
        [SerializeField] private AudioSource soundRocketFire;

        [SerializeField] private AudioSource soundCrash;
        [SerializeField] private Transform vfxCrash;

        [SerializeField] private AudioSource soundBombDrop;
        [SerializeField] private AudioSource soundGun;
        private AudioSource soundClick;

        [SerializeField] private Transform activeWeaponGun;
        [SerializeField] private Transform activeWeaponMissile;
        [SerializeField] private Transform activeWeaponBomb;

        [SerializeField] private TextMeshProUGUI textSpeed;
        [SerializeField] private TextMeshProUGUI textAltitude;
        [SerializeField] private TextMeshProUGUI textBombs;
        [SerializeField] private TextMeshProUGUI textMissiles;
        [SerializeField] private TextMeshProUGUI textAirplanes;
        [SerializeField] private TextMeshProUGUI textWind;
        [SerializeField] private RectTransform windNeedle;
        [SerializeField] private RectTransform compassNeedle;
        [SerializeField] private RectTransform fuelNeedle;
        [SerializeField] private RectTransform fuelLight;
        [SerializeField] private RectTransform minimapAirplane;
        [SerializeField] private RectTransform throttleBar;
        [SerializeField] private RectTransform healthBarOwn;
        [SerializeField] private RectTransform healthBarEnemy;

        private GameObject[] missile = new GameObject[2];
        int currentMissile;
        int currentBullet;
        float[] timeMissileFired = new float[2];
        float health;

        float timeBombDropped;
        private GameObject bomb;
        float timeGunFired;
        float timeCrashed;

        float windSpeed = 2.5f;            // speed 0..5
        float windSpeedChange;      // -0.1 .. 0.1
        float windDirection;        // 0..360
        float windDirectionChange;  // -0.1 .. 0.1
        float timeWindChanged;

        private int currentWeapon;          // 0=gun, 1=missile, 2=bomb

        private Game scriptGame;
        [SerializeField] private Transform airplaneSpawnPosition;

        private float altitude, previousAltitude;
        private float heightAboveGround;

        Vector2 movement;
        bool buttonAccelerate;
        bool buttonDecelerate;
        bool buttonFire;
        bool buttonSwitchWeapon;
        bool buttonYawLeft;
        bool buttonYawRight;
        bool buttonRestart;

        int numAirplanes;
        int numMissiles;
        int numBombs;
        float amountFuel;           // fuel 0..100

        public AudioSource SoundBombDrop { get => soundBombDrop; set => soundBombDrop = value; }

        private void Awake()
        {
            scriptGame = GameObject.Find("/Scripts/Game").GetComponent<Game>();
            soundClick = GameObject.Find("/Sound/Click").GetComponent<AudioSource>();
        }

        private void Start()
        {
            CreateMissile(0);
            CreateMissile(1);
            CreateBomb();

            ResetGame();
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
        }

        private void CreateMissile(int missileNumber)
        {
            missile[missileNumber] = Instantiate(pfMissile, spawnPositionMissile[missileNumber].position, Quaternion.LookRotation(transform.forward, Vector3.up));
            missile[missileNumber].name = "Missile-" + (missileNumber == 0 ? "Left" : "Right");
            missile[missileNumber].transform.parent = transform;
        }

        private void CreateBomb()
        {
            bomb = Instantiate(pfBomb, spawnPositionBomb.position, Quaternion.LookRotation(transform.forward, Vector3.up));
            bomb.transform.parent = transform;
        }

        private void DropBomb()
        {
            soundBombDrop.Play();
            timeBombDropped = Time.time;
            numBombs--;
            bomb.transform.SetParent(null);
            bomb.GetComponent<Rigidbody>().isKinematic = false;
            bomb.GetComponent<Rigidbody>().useGravity = true;
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
        }

        private void FireMissile()
        {
            if (missile[currentMissile].GetComponent<Missile>().IsMoving == false)
            {
                timeMissileFired[currentMissile] = Time.time;
                missile[currentMissile].GetComponent<Missile>().IsMoving = true;
                missile[currentMissile].transform.SetParent(null);
                soundRocketFire.Play();
                Transform newFX = Instantiate(vfxShootRocket, spawnPositionMissile[currentMissile].position, Quaternion.identity);
                newFX.parent = GameObject.Find("/PlaneFX").transform;
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
                soundClick.Play();
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

            if (buttonFire)
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
                    if (Time.time - timeBombDropped > 1)
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

            // Spawn new bomb
            if (numBombs > 0)
            {
                if (bomb.transform.parent == null)
                {
                    CreateBomb();
                }
            }
        }

        public void AddMissiles()
        {
            numMissiles += 3;
            textMissiles.text = numMissiles.ToString();
        }

        public void AddBombs()
        {
            numBombs++;
            textBombs.text = numBombs.ToString();
        }
        
        public void AddFuel()
        {
            amountFuel += 10;
            if(amountFuel > 100)
            {
                amountFuel = 100;
            }
        }

        private void Update()
        {
            UpdateWind();
            health = 100 - (Time.time*100f) % 100;
            healthBarOwn.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, health * 1.728f);

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
            // { -4620, 2650, 4270, -2623 } -> { -8, -13, 228, -160  }
            float minimapXPos = (float)(-8 + 236 * (transform.position.x + 4620) / 8890.0);
            float minimapYPos = (float)(-13 + 147 * (transform.position.z + 2623) / 5273);
            minimapAirplane.localPosition = new Vector3(minimapXPos, minimapYPos, 0);

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

                // Rotate propellers if any
                if (propellors.Length > 0)
                {
                    RotatePropellors(propellors);
                }
            }
            else
            {
                if (!scriptGame.GameState.Equals(GameState_.GameOver)  && Time.time - timeCrashed > 4)
                {
                    ResetPlayer();
                }
            }

            if (scriptGame.GameState.Equals(GameState_.GameOver) && buttonRestart)
            {
                ResetGame();
            }
        }

        private void ResetGame()
        {
            numAirplanes = 5;
            healthBarOwn.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, health * 1.25f);
            ResetPlayer();
            scriptGame.SetGameState(GameState_.Playing);
        }

        private void ResetPlayer()
        {
            health = 100;
            timeBombDropped = timeMissileFired[0] = timeMissileFired[1] = Time.time;
            throttle = currentSpeed = 0;
            throttleBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, throttle * 1.25f);

            fuelLight.gameObject.SetActive(false);
            transform.position = airplaneSpawnPosition.position;
            transform.rotation = airplaneSpawnPosition.rotation;
            engineSoundSource.volume = 0.4f;

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

        private void Movement()
        {
            Vector3 oldPosition = transform.position;

            //Move forward
            transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);

            if (currentSpeed > MINIMUM_FLY_SPEED)
            {
                float windChangeZ = WIND_STRENGTH * Time.deltaTime * windSpeed * Mathf.Cos(windDirection * (Mathf.PI / 180));
                float windChangeX = WIND_STRENGTH * Time.deltaTime * windSpeed * -Mathf.Sin(windDirection * (Mathf.PI / 180));
                transform.position += new Vector3(windChangeX, 0, windChangeZ);
            }

            // Rotate airplane by inputs
            if (currentSpeed > MINIMUM_FLY_SPEED)
            {
                transform.Rotate(Vector3.forward * -movement.x * rollSpeed * Time.deltaTime);
                transform.Rotate(Vector3.right * movement.y * pitchSpeed * Time.deltaTime);
            }

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
                    throttleBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, throttle * 1.25f);
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
                    throttleBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, throttle * 1.25f);
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
            Vector3 distance = transform.position - oldPosition;
            textSpeed.text = "Speed: " + (distance.magnitude / Time.deltaTime).ToString("0");

            //Audio
            // TODO : set to speed            engineSoundSource.pitch =;
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
            if (currentSpeed < MINIMUM_DAMAGE_SPEED)
            {
                // Too slow to crash
                return;
            }
            // Set rigidbody to non-kinematic
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Rigidbody>().useGravity = true;

            timeCrashed = Time.time;
            isCrashed = true;
            engineSoundSource.volume = 0f;

            Instantiate(vfxCrash, transform.position, Quaternion.identity);
            soundCrash.Play();

            numAirplanes--;
            textAirplanes.text = numAirplanes.ToString();
            if (numAirplanes == 0)
            {
                scriptGame.SetGameState(GameState_.GameOver);
            }
        }

        #region Variables


        #endregion
    }
}