using UnityEngine;
using UnityEngine.InputSystem;

namespace Tanks.Input
{
    /// <summary>Adapter around the project Input Actions asset for tank controls.</summary>
    public sealed class TankInput : MonoBehaviour
    {
        [SerializeField] private InputActionAsset inputActions = null;
        [SerializeField] private string actionMapName = "Tank";

        private InputAction moveAction;
        private InputAction rotateAction;
        private InputAction fireAction;
        private InputAction aimAction;

        public float Move => moveAction.ReadValue<float>();
        public float Rotate => rotateAction.ReadValue<float>();
        public bool FirePressedThisFrame => fireAction.WasPressedThisFrame();
        public Vector2 AimPosition => aimAction.ReadValue<Vector2>();

        private void Awake()
        {
            InputActionMap map = inputActions.FindActionMap(actionMapName, true);
            moveAction = map.FindAction("Move", true);
            rotateAction = map.FindAction("Rotate", true);
            fireAction = map.FindAction("Fire", true);
            aimAction = map.FindAction("Aim", true);
        }

        private void OnEnable() => inputActions?.FindActionMap(actionMapName, true).Enable();

        private void OnDisable() => inputActions?.FindActionMap(actionMapName, true).Disable();
    }
}
