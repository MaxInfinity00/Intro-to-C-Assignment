using System.Collections.Generic;
using Freya;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace IntroAssignment {
    public class MainMenuManager : MonoBehaviour {
        [Header("Main Menu Components")]
        public GameObject MainMenu;
        public GameObject PlayMenu;
        public GameObject HowToPlayMenu;

        [Header("Game Settings")]
        public GameSettings gameSettings;

        [Header("Play Menu Stuff")]
        public TMP_InputField seedInput;
        public Vector2 minMaxPlayers;
        public TextMeshProUGUI playerText;
        public List<int> turnDurations;
        public TextMeshProUGUI turnDurationText;
        private int _turnIndex;
        private int _playerIndex;
    
        [Header("Spinning Mesh")]
        public Renderer textureRenderer;
        public MeshFilter meshFilter;
        public MeshRenderer meshRenderer;
    
        public MeshSettings meshSettings;
        public HeightMapSettings heightMapSettings;
        public TextureData textureData;

        public Transform meshTransform;
        

        private void Awake() {
            heightMapSettings.noiseSettings.seed = Random.Range(int.MinValue, int.MaxValue);
            LevelMeshGenerator.GenerateLevel(meshSettings, heightMapSettings, meshFilter);
        }

        private void Update() {
            meshTransform.Rotate(Vector3.up,Time.deltaTime);
        }
    
        public void BackToMainMenu() {
            PlayMenu.SetActive(false);
            HowToPlayMenu.SetActive(false);
        
            MainMenu.SetActive(true);
        }
    
        public void OnPlay() {
            MainMenu.SetActive(false);
            PlayMenu.SetActive(true);
            seedInput.text = "";
            _playerIndex = (int)minMaxPlayers.x;
            playerText.text = _playerIndex.ToString();
            _turnIndex = 0;
            turnDurationText.text = turnDurations[_turnIndex].ToString();
        }
    
        public void PressHowToPlay() {
            MainMenu.SetActive(false);
            HowToPlayMenu.SetActive(true);
        }
    
        public void OnQuit() {
            Application.Quit();
        }

        public void PlayerIncrementer(bool increment) {
            _playerIndex += increment ? 1 : -1;
            _playerIndex = _playerIndex.Clamp((int)minMaxPlayers.x, (int)minMaxPlayers.y);
            playerText.text = _playerIndex.ToString();
        }
        public void TurnDurationIncrementer(bool increment) {
            _turnIndex += increment ? 1 : -1;
            _turnIndex = _turnIndex.Clamp(0, turnDurations.Count - 1);
            turnDurationText.text = turnDurations[_turnIndex].ToString();
        }

        public void OnStartGame() {
            gameSettings.seed = seedInput.text.GetHashCode();
            gameSettings.playerCount = _playerIndex;
            gameSettings.turnTime = turnDurations[_turnIndex];

            SceneManager.LoadScene("Play Scene");
        }
    }
}