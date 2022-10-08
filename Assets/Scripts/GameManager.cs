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
        public int playerMaxHealth;
        [SerializeField] private CameraFollow _cameraFollow;
        [SerializeField] private GameObject healthBarPrefab;
        [SerializeField] private Transform healthBarParent;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private List<Color> playerColors;
        [SerializeField] private Vector2 mapSpawnSize;
        public int _currentPlayer = 0;

        public GameObject GameOverText;
        
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

        [Range(0, 11)] public float pickUpSpawnChances;

        public ControlState controlState = ControlState.Waiting;
        private Controls _controls;
        public List<Player> players = new List<Player>();
        
        [Header("Level Mesh")]
        public MeshFilter meshFilter;
        public MeshCollider meshCollider;
    
        public MeshSettings meshSettings;
        public HeightMapSettings heightMapSettings;

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
                    players[_currentPlayer].OnStartTurn();
                }
            }
        }

        public void OnNextTurn(InputAction.CallbackContext context) {
            if(context.performed) NextTurn();
        }
        
        public void NextTurn() {
            int lastPlayer = _currentPlayer;
            _timeLeft = turnWaitTime;
            controlState = ControlState.Waiting;
            // players[_currentPlayer].SetControls(false);
            players[_currentPlayer].OnEndTurn();
            
            while (true) {
                _currentPlayer = (_currentPlayer + 1) % players.Count;
                if (players[_currentPlayer].isAlive) break;
            }
            
            if (lastPlayer == _currentPlayer) {
                //GameOver Bitches
                controlState = ControlState.GameOver;
            }
            else
            {
                // players[_currentPlayer].SetControls(true);
                _cameraFollow.SetTarget(players[_currentPlayer].playerTransform);
                
                float pickUpSpawn = Random.Range(0, 100f);
                if (pickUpSpawn <= pickUpSpawnChances) {
                    //spawn pickup
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
    }

    public enum ControlState {
        On,
        Waiting,
        Transition,
        GameOver
    }
}