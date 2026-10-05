using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class BasicPlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5.0f;
    public float mouseSensitivity = 0.1f;
    public CharacterController controller;
    public float jumpForce = 5.0f;
    
    private Vector3 _moveDirection = Vector3.zero;
    private Vector3 moveDirection;
    private float _verticalRotation = 0f;

    void Start()
    {
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        if (Keyboard.current == null)
        {
            Debug.LogError("DIAGNOSTIC: No keyboard detected by Unity's Input System!");
        }
    }
    void Update()
    {
        
        if (Mouse.current != null)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue() * mouseSensitivity;
            
            transform.Rotate(Vector3.up * mouseDelta.x);

            _verticalRotation -= mouseDelta.y;
            _verticalRotation = Mathf.Clamp(_verticalRotation, -90f, 90f);
            Camera.main.transform.localRotation = Quaternion.Euler(_verticalRotation, 0f, 0f);
        }
        
        if (controller.isGrounded)
        {
            float moveX = 0f;
            float moveZ = 0f;

            if (Keyboard.current != null)
            {
                // Read WASD / Arrow Keys
                if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) moveZ = 1f;
                if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) moveZ = -1f;
                if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveX = -1f;
                if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveX = 1f;
            }

            _moveDirection = (transform.forward * moveZ) + (transform.right * moveX);
            _moveDirection = _moveDirection.normalized * moveSpeed; // Normalized to prevent fast diagonal movement

            // 3. Jumping
            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                _moveDirection.y = jumpForce;
            }
        }

        //_moveDirection.y -= Physics.gravity.y * Time.deltaTime;
        controller.Move(_moveDirection * Time.deltaTime);

        // Unlock cursor with Escape
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
