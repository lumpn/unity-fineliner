using UnityEngine;

public sealed class Snake
{
    public enum Direction
    {
        Up,
        Right,
        Down,
        Left,
    }

    public Color color;
    public Direction direction;
    public int posX, posY;
    public bool alive = true;

    public int nextPosX
    {
        get
        {
            int dx = 0;
            switch (direction)
            {
                case Direction.Right: dx =  1; break;
                case Direction.Left:  dx = -1; break;
            }
            return posX + dx;
        }
    }

    public int nextPosY
    {
        get
        {
            int dy = 0;
            switch (direction)
            {
                case Direction.Up:   dy =  1; break;
                case Direction.Down: dy = -1; break;
            }
            return posY + dy;
        }
    }

    private readonly SnakeController controller;

    public Snake(int px, int py, Direction dir, Color col, SnakeController controller)
    {
        posX = px;
        posY = py;
        direction = dir;
        color = col;
        this.controller = controller;
    }

    public void DoUpdate(IPlayground playground)
    {
        HandleInput();
        bool jump = controller.jump;
        controller.Reset();

        if (!alive)
            return;

        int step = (jump ? 20 : 1);

        int dx = 0;
        int dy = 0;
        switch (direction)
        {
            case Direction.Up:    dy =  step; break;
            case Direction.Right: dx =  step; break;
            case Direction.Down:  dy = -step; break;
            case Direction.Left:  dx = -step; break;
        }

        posX += dx;
        posY += dy;

        if (posX < 0 || posX > 255 || posY < 0 || posY > 255)
        {
            alive = false;
            return;
        }

        if (!playground.IsFree(posX, posY))
        {
            alive = false;
            return;
        }
    }

    private void HandleInput()
    {
        var desiredDirection = direction;
        if (controller.up) Steer(Direction.Up, ref desiredDirection);
        if (controller.down) Steer(Direction.Down, ref desiredDirection);
        if (controller.left) Steer(Direction.Left, ref desiredDirection);
        if (controller.right) Steer(Direction.Right, ref desiredDirection);
//        if (Input.GetButtonDown("TurnLeft")) desiredDirection = (Direction)(((int)direction + 3) % 4);
//        if (Input.GetButtonDown("TurnRight")) desiredDirection = (Direction)(((int)direction + 1) % 4);
        direction = desiredDirection;
    }

    private void Steer(Direction target, ref Direction desiredDirection)
    {
        var diff = Mathf.Abs(target - direction);
        if (diff == 1 || diff == 3)
        {
            desiredDirection = target;
        }
    }
}
