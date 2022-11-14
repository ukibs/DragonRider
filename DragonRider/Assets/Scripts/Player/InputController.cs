using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    //
    private static InputController instance;
    private Gamepad gamepad;
    private Keyboard keyboard;
    private Mouse mouse;
    //private float cameraDirection = -1;

    //
    public static InputController Instance { get { return instance; } }

    //
    public Vector2 MovementAxis
    {
        get
        {
            //Debug.Log(gamepad);
            if (gamepad != null)
            {
                //Debug.Log(gamepad.leftStick.ReadValue());
                return gamepad.leftStick.ReadValue();
            }
            else if (keyboard != null)
            {

                //Debug.Log("Getting keyboard");

                float horizontalAxis = 0;
                if (keyboard.aKey.isPressed) horizontalAxis = -1;
                if (keyboard.dKey.isPressed) horizontalAxis = 1;

                float verticalAxis = 0;
                if (keyboard.wKey.isPressed) verticalAxis = 1;
                if (keyboard.sKey.isPressed) verticalAxis = -1;

                //Debug.Log(horizontalAxis + " - " + verticalAxis);

                return new Vector2(horizontalAxis, verticalAxis);
            }
            else
            {
                return Vector2.zero;
            }
        }
    }

    //
    public Vector2 CameraAxis
    {
        get
        {
            if (gamepad != null)
            {
                Vector2 move = gamepad.rightStick.ReadValue();
                move = new Vector2(Mathf.Pow(move.x, 2) * Mathf.Sign(move.x), Mathf.Pow(move.y, 2) * Mathf.Sign(move.y));
                return move;
            }
            else if (mouse != null)
            {
                return mouse.delta.ReadValue() * 0.1f;
            }
            else
            {
                return Vector2.zero;
            }
        }
    }

    // A button
    public bool APressed
    {
        get
        {
            if (gamepad != null)
            {
                return gamepad.aButton.wasPressedThisFrame;
            }
            else if (keyboard != null)
            {
                return keyboard.spaceKey.wasPressedThisFrame;
            }
            else
            {
                return false;
            }
        }
    }

    public bool AReleased
    {
        get
        {
            if (gamepad != null)
            {
                return gamepad.aButton.wasReleasedThisFrame;
            }
            else if (keyboard != null)
            {
                return keyboard.spaceKey.wasReleasedThisFrame;
            }
            else
            {
                return false;
            }
        }
    }

    // B Button
    public bool BPressed
    {
        get
        {
            if (gamepad != null)
            {
                return gamepad.bButton.wasPressedThisFrame;
            }
            else if (keyboard != null)
            {
                return keyboard.leftShiftKey.wasPressedThisFrame;
            }
            else
            {
                return false;
            }
        }
    }

    public bool BReleased
    {
        get
        {
            if (gamepad != null)
            {
                return gamepad.bButton.wasReleasedThisFrame;
            }
            else if (keyboard != null)
            {
                return keyboard.leftShiftKey.wasReleasedThisFrame;
            }
            else
            {
                return false;
            }
        }
    }

    // Right Shoulder
    public bool RightShoulderPressed
    {
        get
        {
            if (gamepad != null)
            {
                return gamepad.rightShoulder.wasPressedThisFrame;
            }
            else if (mouse != null)
            {
                return mouse.rightButton.wasPressedThisFrame;
            }
            else
            {
                return false;
            }
        }
    }

    public bool RightShoulderReleased
    {
        get
        {
            if (gamepad != null)
            {
                return gamepad.rightShoulder.wasReleasedThisFrame;
            }
            else if (mouse != null)
            {
                return mouse.rightButton.wasReleasedThisFrame;
            }
            else
            {
                return false;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        gamepad = Gamepad.current;
        keyboard = Keyboard.current;
        mouse = Mouse.current;
    }
}
