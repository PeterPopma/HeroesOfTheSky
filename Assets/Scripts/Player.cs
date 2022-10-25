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
        private bool isDead;
        private const float MINIMUM_FLY_SPEED = 30;
        private const float MINIMUM_DAMAGE_SPEED = 30;

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
        [SerializeField] private Transform[] spawnPositionMissile = new Transform[2];
        [SerializeField] private Transform spawnPositionBomb;

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
        [SerializeField] private Transform compassNeedle;
        [SerializeField] private Transform fuelNeedle;
        [SerializeField] private Transform fuelLight;
        [SerializeField] private RectTransform minimapAirplane;
        [SerializeField] private RectTransform throttleBar;
        [SerializeField] private RectTransform healthBarOwn;
        [SerializeField] private RectTransform healthBarEnemy;

        private GameObject[] missile = new GameObject[2];
        int currentMissile;
        float[] timeMissileFired = new float[2];
        float health;

        float timeBombDropped;
        private GameObject bomb;

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

        int numMissiles;
        int numBombs;
        float amountFuel;           // fuel 0..100

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
            bomb.GetComponent<Bomb>().Activate();
            textBombs.text = numBombs.ToString();
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
                buttonFire = false;

                if (currentWeapon == 1)
                { 
                    FireMissile();
                }
                if (currentWeapon == 2 && numBombs>0)
                {
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
            health = 100 - (Time.time*100f) % 100;
            healthBarOwn.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, health * 1.728f);

            previousAltitude = altitude;
            altitude = transform.position.y;
            textAltitude.text = "Alt: " + altitude.ToString("0");
            heightAboveGround = transform.position.y-Terrain.activeTerrain.SampleHeight(transform.position);
            compassNeedle.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.y);
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
//            minimapAirplane.localPosition = new Vector3(0, -12 + 150, 0); // -> y 0 -> -150
//            minimapAirplane.localPosition = new Vector3(0, -158 + 150, 0); // -> y 0 -> -150

            // Airplane move only if not dead
            if (!isDead)
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

            if (scriptGame.GameState.Equals(GameState_.GameOver) && buttonRestart)
            {
                ResetGame();
            }
        }
        private void ResetGame()
        {
            health = 100;
            timeBombDropped = timeMissileFired[0] = timeMissileFired[1] = Time.time;
            throttle = currentSpeed = 0;
            throttleBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, throttle * 1.25f);
            healthBarOwn.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, health * 1.25f);

            fuelLight.gameObject.SetActive(false);
            scriptGame.SetGameState(GameState_.Playing);
            transform.position = airplaneSpawnPosition.position;
            transform.rotation = airplaneSpawnPosition.rotation;
            engineSoundSource.volume = 0.4f;

            isDead = false;
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Rigidbody>().useGravity = false;

            numMissiles = 10;
            numBombs = 3;
            amountFuel = 80;

            textMissiles.text = numMissiles.ToString();
            textBombs.text = numBombs.ToString();
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
            //Move forward
            transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);

            //Rotate airplane by inputs
            if (currentSpeed > MINIMUM_FLY_SPEED)
            {
                transform.Rotate(Vector3.forward * -movement.x * rollSpeed * Time.deltaTime);
                transform.Rotate(Vector3.right * movement.y * pitchSpeed * Time.deltaTime);
            }

            //Rotate yaw
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

            float gainFromDescending = (previousAltitude - altitude) * Time.deltaTime * 500;
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
                textSpeed.text = "Speed: " + currentSpeed.ToString("0");
            }

            if (throttle + gainFromDescending < currentSpeed)
            {
                currentSpeed -= speedAcceleration * Time.deltaTime;
                textSpeed.text = "Speed: " + currentSpeed.ToString("0");
            }

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

            // Kill player
            isDead = true;
            engineSoundSource.volume = 0f;

            Instantiate(vfxCrash, transform.position, Quaternion.identity);
            soundCrash.Play();
            scriptGame.SetGameState(GameState_.GameOver);
        }

        #region Variables

        public bool PlaneIsDead()
        {
            return isDead;
        }

        public float CurrentSpeed()
        {
            return currentSpeed;
        }

        #endregion
    }
}