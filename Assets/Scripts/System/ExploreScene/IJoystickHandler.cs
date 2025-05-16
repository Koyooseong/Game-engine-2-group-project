using UnityEngine;
public enum JoystickType { Left, Right }

public interface IJoystickHandler
{
    Vector2 Direction { get; }
    void Initialize(JoystickType joystickType);
}