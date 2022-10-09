using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace IntroAssignment {
    public class MenuButton : MonoBehaviour, Controls.IUIActions
    {
        public Image buttonImage;
        public TextMeshProUGUI buttonText;
    
        public Color buttonIdleColor;
        public Color buttonHoverColor;
        public Color buttonClickColor;

        public Color textIdleColor;
        public Color textHoverColor;
        public Color textClickColor;

        private Controls _controls;

        public void Awake() {
            _controls = new Controls();
            _controls.UI.Enable();
            _controls.UI.SetCallbacks(this);
            IdleState();
        }

        private void IdleState() {
            buttonImage.color = buttonIdleColor;
            buttonText.color = textIdleColor;
        }
    
        private void HoverState() {
        
            buttonImage.color = buttonHoverColor;
            buttonText.color = textHoverColor;
        }
    
        private void ClickState() {
            buttonImage.color = buttonClickColor;
            buttonText.color = textClickColor;
        
        }

        public void OnNavigate(InputAction.CallbackContext _) {}
        public void OnSubmit(InputAction.CallbackContext _) { }

        public void OnCancel(InputAction.CallbackContext _) {}

        public void OnPoint(InputAction.CallbackContext context) {
            if(context.performed)
                HoverState();
        }

        public void OnClick(InputAction.CallbackContext context) {
            if(context.performed)
                ClickState();
        }

        public void OnScrollWheel(InputAction.CallbackContext _) {}

        public void OnMiddleClick(InputAction.CallbackContext _) {}

        public void OnRightClick(InputAction.CallbackContext _) {}

        public void OnTrackedDevicePosition(InputAction.CallbackContext _) {}

        public void OnTrackedDeviceOrientation(InputAction.CallbackContext _) {}
    }
}