using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class Airplane : MonoBehaviour
    {
        private float currentSpeed;
        private float throttle;
        private bool planeIsDead;
        private Rigidbody rigidbody;
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

        [SerializeField] private GameObject pfRocket;
        [SerializeField] private Transform[] spawnPositionRocket = new Transform[2];

        [SerializeField] private Transform vfxShootRocket;
        [SerializeField] private AudioSource soundRocketFire;

        [SerializeField] private AudioSource soundCrash;
        [SerializeField] private Transform vfxCrash;

        [SerializeField] private TextMeshProUGUI textThrottle;
        [SerializeField] private TextMeshProUGUI textSpeed;
        [SerializeField] private TextMeshProUGUI textAltitude;
        [SerializeField] private TextMeshProUGUI textHeight;
        [SerializeField] private Transform needle;

        private GameObject[] rocket = new GameObject[2];
        int currentRocket;
        float[] timeRocketFired = new float[2];

        private Game scriptGame;
        [SerializeField] private Transform airplaneSpawnPosition;

        private float altitude, previousAltitude;

        Vector2 movement;
        bool buttonAccelerate;
        bool buttonDecelerate;
        bool buttonFire;
        bool buttonYawLeft;
        bool buttonYawRight;
        bool buttonRestart;

        private void Awake()
        {
            scriptGame = GameObject.Find("/Scripts/Game").GetComponent<Game>();
        }

        private void Start()
        {
            rigidbody = GetComponent<Rigidbody>();

            CreateRocket(0);
            CreateRocket(1);
        }

        private void OnRestart(InputValue value)
        {
            buttonRestart = value.isPressed;
        }

        private void OnFire(InputValue value)
        {
            buttonFire = value.isPressed;
        }

        private void CreateRocket(int rocketNumber)
        {
            rocket[rocketNumber] = Instantiate(pfRocket, spawnPositionRocket[rocketNumber].position, Quaternion.LookRotation(transform.forward, Vector3.up));
            rocket[rocketNumber].name = "Rocket-" + (rocketNumber == 0 ? "Left" : "Right");
            rocket[rocketNumber].transform.parent = transform;
        }

        private void UpdateRockets()
        {
            if (buttonFire)
            {
                buttonFire = false;

                if (rocket[currentRocket].GetComponent<Rocket>().IsMoving == false)
                {
                    timeRocketFired[currentRocket] = Time.time;
                    rocket[currentRocket].GetComponent<Rocket>().IsMoving = true;
                    rocket[currentRocket].transform.SetParent(null);
                    soundRocketFire.Play();
                    Instantiate(vfxShootRocket, spawnPositionRocket[currentRocket].position, Quaternion.identity);
                }

                currentRocket++;
                if (currentRocket > 1)
                {
                    currentRocket = 0;
                }
            }

            // Spawn new rocket
            for (int rocket_number = 0; rocket_number < 2; rocket_number++)
            {
                if (rocket[rocket_number] == null || rocket[rocket_number].GetComponent<Rocket>().IsMoving == true)
                {
                    if (Time.time - timeRocketFired[rocket_number] > 1)
                    {
                        CreateRocket(rocket_number);
                    }
                }
            }
        }
        
        private void Update()
        {
            previousAltitude = altitude;
            altitude = transform.position.y;
            textAltitude.text = "altitude: " + altitude.ToString("0");
            textHeight.text = "above ground: " + (transform.position.y-Terrain.activeTerrain.SampleHeight(transform.position)).ToString("0");
            needle.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.y);

            // Airplane move only if not dead
            if (!planeIsDead)
            {
                Movement();
                UpdateRockets();
                if (currentSpeed < MINIMUM_FLY_SPEED)
                {
                    rigidbody.useGravity = true;
                    rigidbody.isKinematic = false;
                }
                else
                {
                    rigidbody.useGravity = false;
                    rigidbody.isKinematic = true;
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
            scriptGame.SetGameState(GameState_.Playing);
            transform.position = airplaneSpawnPosition.position;
            transform.rotation = airplaneSpawnPosition.rotation;
            engineSoundSource.volume = 0.4f;

            planeIsDead = false;
            rigidbody.isKinematic = true;
            rigidbody.useGravity = false;
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
                    textThrottle.text = "Throttle: " + throttle.ToString("0") + "%";
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
                    textThrottle.text = "Throttle: " + throttle.ToString("0") + "%";
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
            // Set rigidbody to non cinematic
            rigidbody.isKinematic = false;
            rigidbody.useGravity = true;

            // Kill player
            planeIsDead = true;
            engineSoundSource.volume = 0f;

            Instantiate(vfxCrash, transform.position, Quaternion.identity);
            soundCrash.Play();
            scriptGame.SetGameState(GameState_.GameOver);
        }

        #region Variables

        public bool PlaneIsDead()
        {
            return planeIsDead;
        }

        public float CurrentSpeed()
        {
            return currentSpeed;
        }

        #endregion
    }
}