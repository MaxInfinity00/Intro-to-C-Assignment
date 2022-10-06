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
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private List<Color> playerColors;
        [SerializeField] private Vector2 mapSpawnSize;
        public float turnTime;
        public float turnWaitTime;
        public float transitionTime;
        private float _timeLeft;
        [SerializeField] private int _currentPlayer = 0;

        [SerializeField] private Image timerImage;
        [SerializeField] private TextMeshProUGUI timerText;

        [SerializeField] private Color timerStartColor;
        [SerializeField] private Color timerEndColor;
        // private bool isOngoingTurn;

        [Range(0, 11)] public float pickUpSpawnChances;

        public ControlState controlState = ControlState.Off;
        private Controls _controls;
        public List<Player> players = new List<Player>();
        
        [Header("Level Mesh")]
        public Renderer textureRenderer;
        public MeshFilter meshFilter;
        public MeshRenderer meshRenderer;
        public MeshCollider meshCollider;
    
        public MeshSettings meshSettings;
        public HeightMapSettings heightMapSettings;

        public static GameManager instance;

        private void Awake() {
            instance = this;
            
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
            }
            
            _controls = new Controls();
            _controls.Manager.Enable();
            _controls.Manager.SetCallbacks(this);
        }

        public void AddPlayer(Player player) {
            players.Add(player);
            if (players.Count == 1) {
                player.SetControls(true);
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
            else if (controlState == ControlState.Off) {
                _timeLeft -= Time.deltaTime;
                if (_timeLeft <= 0) {
                    controlState = ControlState.Transition;
                    _timeLeft = turnTime;
                }
            }
        }

        public void OnNextTurn(InputAction.CallbackContext context) {
            if(context.performed) NextTurn();
        }
        
        void NextTurn() {
            int lastPlayer = _currentPlayer;
            _timeLeft = turnWaitTime;
            controlState = ControlState.Off;
            players[_currentPlayer].SetControls(false);
            
            while (true) {
                _currentPlayer = (_currentPlayer + 1) % players.Count;
                if (players[_currentPlayer].isAlive) break;
            }
            
            if (lastPlayer == _currentPlayer) {
                //GameOver Bitches
            }
            else
            {
                players[_currentPlayer].SetControls(true);
                _cameraFollow.SetTarget(players[_currentPlayer].playerTransform);
                
                float pickUpSpawn = Random.Range(0, 100f);
                if (pickUpSpawn <= pickUpSpawnChances) {
                    //spawn pickup
                }
            }
        }
    }

    public enum ControlState {
        On,
        Off,
        Transition
    }
}