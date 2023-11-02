using System;
using ColorRoad.UI;
using UnityEngine;
using UnityEngine.Assertions;

namespace ColorRoad.Systems
{
    public class InputControls : MonoBehaviour
    {
        [SerializeField] private InputMovementPanel inputMovementPanel;
        [SerializeField] private float screenSensitivity;
        [SerializeField] private float keyboardSensitivity;

        public event Action<float> onPlayerMove;

        private void Awake()
        {
            Assert.IsNotNull(inputMovementPanel);
            inputMovementPanel.onInputChanged += OnScreenInputChanged;
        }

        private void OnDestroy()
        {
            if (inputMovementPanel != null)
            {
                inputMovementPanel.onInputChanged -= OnScreenInputChanged;
            }
        }

        private void Update()
        {
            float input = Input.GetAxis("Horizontal");
            OnKeyboardInputChanged(input);
        }

        private void OnScreenInputChanged(Vector2 delta)
        {
            PlayerMovement(delta.x * screenSensitivity);
        }

        private void OnKeyboardInputChanged(float delta)
        {
            PlayerMovement(delta * keyboardSensitivity);
        }

        private void PlayerMovement(float delta)
        {
            onPlayerMove?.Invoke(delta);
        }
    }
}