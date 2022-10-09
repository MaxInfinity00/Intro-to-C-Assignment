using UnityEngine;
using System.Collections.Generic;
using Freya;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace IntroAssignment {
    public class GameManager : MonoBehaviour, Controls.IManagerActions {
        
        public GameSettings gameSettings;
        [SerializeField] private CameraFollow _cameraFollow;

        public GameObject GameOverText;
        public ControlState controlState = ControlState.Waiting;
        
        [Header("Players")]
        public int currentPlayer = 0;
        public List<Player> players = new List<Player>();
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private List<Color> playerColors;
        [SerializeField] private Vector2 mapSpawnSize;
        public int playerMaxHealth;
        [SerializeField] private GameObject healthBarPrefab;
        [SerializeField] private Transform healthBarParent;
        
        [Header("Timer")]
        public float turnTime;
        public float turnWaitTime;
        public float transitionTime;
        private float _timeLeft;

        [SerializeField] private Image timerImage;
        [SerializeField] private TextMeshProUGUI timerText;

        [SerializeField] private Color timerStartColor;
        [SerializeField] private Color timerEndColor;
        // private bool isOngoingTurn;

        [Header("Pickups")]
        [Range(0, 100)] public float pickUpSpawnChances;
        [SerializeField] private List<GameObject> pickups;


        [Header("Level Mesh")]
        public MeshFilter meshFilter;
        public MeshCollider meshCollider;
    
        public MeshSettings meshSettings;
        public HeightMapSettings heightMapSettings;

        private Controls _controls;
        public static GameManager instance;

        private void Awake() {
            instance = this;
            Player._maxHealth = playerMaxHealth;

            turnTime = gameSettings.turnTime;
            _timeLeft = turnTime;
            
            if (!_cameraFollow) _cameraFollow = FindObjectOfType<CameraFollow>();
            _cameraFollow.transitionTime = transitionTime;
            
            heightMapSettings.noiseSettings.seed = gameSettings.seed;
            LevelMeshGenerator.GenerateLevel(meshSettings, heightMapSettings, meshFilter, meshCollider);
            
            for (int i = 0; i < gameSettings.playerCount; i++) {
                Vector3 spawnPosition = new Vector3(mapSpawnSize.x / 2f * (i%3 == 0 ? -1 : 1),100,mapSpawnSize.y / 2f * (i%2 != 0 ? -1 : 1));
                Physics.Raycast(spawnPosition, Vector3.down, out RaycastHit hit);
                GameObject player = Instantiate(playerPrefab, hit.point + Vector3.up * 5,Quaternion.identity);
                player.GetComponent<Renderer>().material.color = playerColors[i];
                
                Transform playerTransform = player.transform;
                // playerTransform.position = hit.point + Vector3.up * 5;
                playerTransform.LookAt(Vector3.up * playerTransform.position.y);

                GameObject healthbar = Instantiate(healthBarPrefab, healthBarParent);
                player.GetComponent<Player>().SetHealthBar(healthbar.GetComponent<HealthBar>());
            }
            
            _controls = new Controls();
            _controls.Manager.Enable();
            _controls.Manager.SetCallbacks(this);
        }

        public void AddPlayer(Player player) {
            players.Add(player);
            if (players.Count == 1) {
                player.OnStartTurn();
                _cameraFollow.SetTarget(player.playerTransform);
                controlState = ControlState.On;
                _timeLeft = turnTime;
            }
        }

        private void Update() {
            if (controlState == ControlState.On) {
                
                _timeLeft -= Time.deltaTime;
                _timeLeft = _timeLeft.Clamp(0, turnTime);
                timerText.text = _timeLeft.ToString("F");
                timerImage.fillAmount = _timeLeft / turnTime;
                timerImage.color = Color.Lerp(timerEndColor, timerStartColor, _timeLeft / turnTime);
                
                if (_timeLeft <= 0) {
                    NextTurn();
                }
            }
            else if (controlState == ControlState.Waiting) {
                _timeLeft -= Time.deltaTime;
                if (_timeLeft <= 0) {
                    controlState = ControlState.Transition;
                    _timeLeft = turnTime;
                    players[currentPlayer].OnStartTurn();
                }
            }
        }

        public void OnNextTurn(InputAction.CallbackContext context) {
            if(context.performed) NextTurn();
        }
        
        public void NextTurn() {
            int lastPlayer = currentPlayer;
            _timeLeft = turnWaitTime;
            controlState = ControlState.Waiting;
            // players[_currentPlayer].SetControls(false);
            players[currentPlayer].OnEndTurn();
            
            while (true) {
                currentPlayer = (currentPlayer + 1) % players.Count;
                if (players[currentPlayer].isAlive) break;
            }
            
            if (lastPlayer == currentPlayer) {
                //GameOver Bitches
                controlState = ControlState.GameOver;
            }
            else
            {
                // players[_currentPlayer].SetControls(true);
                _cameraFollow.SetTarget(players[currentPlayer].playerTransform);
                
                float pickUpSpawn = Random.Range(0, 100f);
                if (pickUpSpawn <= pickUpSpawnChances) {
                    SpawnPickup();
                }
            }
        }

        public void GameOver() {
            // if (((players) => players.isAlive).ToList().Count <= 1) {
            if (players.FindAll((Player player) => player.isAlive).Count <= 1){
                controlState = ControlState.GameOver;
                GameOverText.SetActive(true);
            }
        }

        public void AddToTimer(int additionalTime) {
            if (controlState == ControlState.On) {
                _timeLeft += additionalTime;
            }
        }

        public void SpawnPickup() {
            Vector3 randomPosition = new Vector3(Random.Range(-mapSpawnSize.x, mapSpawnSize.x), 100,
                Random.Range(-mapSpawnSize.y, mapSpawnSize.y));
            Physics.Raycast(randomPosition, Vector3.down, out RaycastHit hit);
            GameObject randomPickup = pickups[Random.Range(0,pickups.Count-1)];
            Instantiate(randomPickup, hit.point + Vector3.up * 0.75f,Quaternion.identity);
        }
    }

    public enum ControlState {
        On,
        Waiting,
        Transition,
        GameOver
    }
}