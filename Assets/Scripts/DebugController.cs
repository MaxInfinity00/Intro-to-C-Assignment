using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace IntroAssignment {
    public class DebugController : MonoBehaviour, Controls.IDebugActions {
        private bool _showConsole;
        private bool _showHelp;

        private Controls _controls;
        private string _input;
        // public static DebugCommand x;
        // public static DebugCommand<int> x2;
        public static DebugCommand fiveMoreMinutes;
        public static DebugCommand<int> addTime;
        public static DebugCommand superPotion;
        public static DebugCommand help;

        public List<object> commandList;

        private Vector2 scroll;
        private void Awake() {
            _controls = new Controls();
            _controls.Debug.Enable();
            _controls.Debug.SetCallbacks(this);

            // x = new DebugCommand("test", "Test Cheat", "test", () => Debug.Log("Test"));
            // x2 = new DebugCommand<int>("test2", "Test Cheat 2", "test2 <int>", (value) => Debug.Log("Test "+ value));
            fiveMoreMinutes = new DebugCommand("fivemoreminutes", "resets your timer", "fivemoreminutes",
                () => GameManager.instance.AddToTimer(300));
            addTime = new DebugCommand<int>("addtime", "adds specified amount of time to your timer", "addtime <seconds>",
                (value) => GameManager.instance.AddToTimer(value));
            superPotion = new DebugCommand("superpotion", "fully heals active player", "superpotion",()=> {
                if (GameManager.instance.controlState == ControlState.On) {
                    GameManager.instance.players[GameManager.instance._currentPlayer].Heal(Player._maxHealth);
                }
            });
            help = new DebugCommand("help", "Show a list of commands", "help", () => {
                _showHelp = true;
            });
            
            commandList = new List<object> {
                // x,
                // x2,
                addTime,
                fiveMoreMinutes,
                superPotion,
                help
            };
        }

        public void OnToggleConsole(InputAction.CallbackContext _) {
            _showConsole = !_showConsole;
        }

        public void OnReturn(InputAction.CallbackContext _) {
            if (_showConsole) {
                HandleInput();
                _input = "";
            }
        }
        
        private void OnGUI() {
            if (!_showConsole) return;

            float y = 0;

            if (_showHelp) {
                GUI.Box(new Rect(0,y,Screen.width,100),"");

                Rect viewport = new Rect(0, 0, Screen.width - 30, 20 * commandList.Count);

                scroll = GUI.BeginScrollView(new Rect(0, y + 5f, Screen.width, 90), scroll, viewport);

                for (int i = 0; i < commandList.Count; i++) {
                   DebugCommandBase command = commandList[i] as DebugCommandBase;
                   string label = $"{command.commandFormat} - {command.commandDescription}";
                   Rect labelRect = new Rect(5, 20 * i, viewport.width - 100, 20);
                   GUI.Label(labelRect,label);
                }
                
                GUI.EndScrollView();
                
                y += 100;
            }
            
            GUI.Box(new Rect(0,y,Screen.width,30),"");
            GUI.backgroundColor = new Color(0,0,0,0);
            _input = GUI.TextField(new Rect(10f,y+5f, Screen.width-20f,20f),_input);
            if (string.IsNullOrEmpty(_input))
                GUI.TextArea(new Rect(10f, y + 5f, Screen.width - 20f, 20f), "use 'help' to see a list of all commands");
        }

        private void HandleInput() {
            string[] properties = _input.Split(' ');
            
            for (int i = 0; i < commandList.Count; i++) {
                DebugCommandBase commandBase = commandList[i] as DebugCommandBase;
                if (_input.Contains(commandBase.commandId)) {
                    if (commandList[i] is DebugCommand) {
                        (commandList[i] as DebugCommand).Invoke();
                    }
                    else if (commandList[i] is DebugCommand<int>) {
                        (commandList[i] as DebugCommand<int>).Invoke(int.Parse(properties[1]));
                    }
                }
            }
        }
    }
}