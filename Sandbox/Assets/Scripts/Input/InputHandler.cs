using UnityEngine;
using NaughtyAttributes;


public class InputHandler : MonoBehaviour {


    /*--- Variables ---*/

    [SerializeField] private FirstPersonViewConfig firstPersonViewConfig = null;
    [SerializeField] private LookInputState lookInputState = null;
    [SerializeField] private MoveInputState moveInputState = null;

    private InputDriver inputDriver;


    /*--- Lifecycle Methods---*/

    void Awake() {
        inputDriver = new InputDriver();
    }

    void Start() {
        lookInputState.resetInput();
        moveInputState.resetInput();

        setupLookCallbacks();
        setupMoveCallbacks();

        setupGameStateCallbacks();
    }

    void OnEnable() {
        inputDriver.Enable();
    }

    void OnDisable() {
        inputDriver.Disable();
    }

    void Update() {
        updateLookInputState();
        updateMoveInputState();
    }


    /*--- Private Methods ---*/

    private void setupLookCallbacks() {

        // Zoom Stick
        inputDriver.FirstPersonCharacter.ZoomStick.started += _ => {
            if (lookInputState.isZooming) {
                lookInputState.isZoomClicked = false;
                lookInputState.isZoomReleased = true;
            } else {
                lookInputState.isZoomClicked = true;
                lookInputState.isZoomReleased = false;
            }
        };

        // Zoom Mouse
        inputDriver.FirstPersonCharacter.ZoomMouse.started += _ => {
            lookInputState.isZoomClicked = true;
            lookInputState.isZoomReleased = false;
        };
        inputDriver.FirstPersonCharacter.ZoomMouse.canceled += _ => {
            lookInputState.isZoomClicked = false;
            lookInputState.isZoomReleased = true;
        };
    }

    private void setupMoveCallbacks() {
        inputDriver.FirstPersonCharacter.Run.started += _ => {
            moveInputState.isRunClicked = true;
            moveInputState.isRunReleased = false;
        };
        inputDriver.FirstPersonCharacter.Run.canceled += _ => {
            moveInputState.isRunClicked = false;
            moveInputState.isRunReleased = true;
        };

        inputDriver.FirstPersonCharacter.Crouch.started += _ => {
            moveInputState.isCrouchClicked = true;
            moveInputState.isCrouchReleased = false;
        };
        inputDriver.FirstPersonCharacter.Crouch.canceled += _ => {
            moveInputState.isCrouchClicked = false;
            moveInputState.isCrouchReleased = true;
        };
    }

    private void setupGameStateCallbacks() {
        inputDriver.FirstPersonCharacter.Exit.started += _ => { 
            #if UNITY_STANDALONE
                Application.Quit();
            #endif
 
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #endif
        };
    }

    private void updateLookInputState() {
        Vector2 stickInput = inputDriver.FirstPersonCharacter.LookStick.ReadValue<Vector2>();
        Vector2 mouseInput = inputDriver.FirstPersonCharacter.LookMouse.ReadValue<Vector2>();

        // Update Input Device
        if (stickInput.magnitude < firstPersonViewConfig.stickLookDriftThreshold
            && mouseInput != Vector2.zero) {
            lookInputState.isStickAiming = false;
        } else if (stickInput != Vector2.zero) {
            lookInputState.isStickAiming = true;
        } else if (mouseInput != Vector2.zero) {
            lookInputState.isStickAiming = false;
        }

        // Record Look Input
        if (lookInputState.isStickAiming) {
            lookInputState.inputVector.x = stickInput.x;
            lookInputState.inputVector.y = stickInput.y;
        } else {
            lookInputState.inputVector.x = mouseInput.x;
            lookInputState.inputVector.y = mouseInput.y;
        }
    }

    private void updateMoveInputState() {
        Vector2 movementInput = inputDriver.FirstPersonCharacter.Move.ReadValue<Vector2>();
        moveInputState.inputVector.x = movementInput.x;
        moveInputState.inputVector.y = movementInput.y;

        moveInputState.isJumpClicked = inputDriver.FirstPersonCharacter.Jump.triggered;
    }
}