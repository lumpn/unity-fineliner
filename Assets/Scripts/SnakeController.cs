using UnityEngine;

public sealed class SnakeController
{
    public bool up, down, left, right, jump;

    private readonly string keyUp, keyDown, keyLeft, keyRight, keyJump;

    public SnakeController(string up, string down, string left, string right, string jump)
    {
        keyUp = up;
        keyDown = down;
        keyLeft = left;
        keyRight = right;
        keyJump = jump;
    }

    public void Reset()
    {
        up = down = left = right = jump = false;
    }

    public void OnUpdate()
    {
        up    |= Input.GetButtonDown(keyUp);
        down  |= Input.GetButtonDown(keyDown);
        left  |= Input.GetButtonDown(keyLeft);
        right |= Input.GetButtonDown(keyRight);
        jump  |= Input.GetButtonDown(keyJump);
    }
}
