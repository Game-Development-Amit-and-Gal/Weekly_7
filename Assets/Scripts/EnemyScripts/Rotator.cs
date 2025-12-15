using UnityEngine;

/// <summary>
/// Simple rotation component that rotates the GameObject
/// back and forth between two angular bounds on the Y-axis.
/// The rotation direction is automatically reversed
/// when the limits are reached.
/// </summary>
public class Rotator : MonoBehaviour
{
    /// <summary>
    /// Minimum rotation angle (in degrees) on the Y-axis.
    /// </summary>
    [SerializeField] float minAngle = -90;

    /// <summary>
    /// Maximum rotation angle (in degrees) on the Y-axis.
    /// </summary>
    [SerializeField] float maxAngle = 90;

    /// <summary>
    /// Rotation speed in degrees per second.
    /// </summary>
    [SerializeField] float angularSpeed = 30;

    /// <summary>
    /// Current rotation direction:
    /// 1 for clockwise, -1 for counterclockwise.
    /// </summary>
    [SerializeField] private int direction = 1;
    private int clockWise = 1;
    private int counterClockWise = -1;


    int fullAngle = 360;
    int halfAngle = 180;

    /// <summary>
    /// Updates the rotation every frame.
    /// Rotates the object smoothly and reverses direction
    /// when reaching the defined angular bounds.
    /// </summary>
    void Update()
    {
        int minEntry = 0;
        // Apply rotation based on current direction and angular speed
        transform.Rotate(new Vector3(minEntry, direction * angularSpeed * Time.deltaTime, minEntry));

        // Normalize angle to range [-180, 180]
        float angle = transform.rotation.eulerAngles.y;
        if (angle > halfAngle)
            angle -= fullAngle;

        // Reverse direction when limits are reached
        if (angle <= minAngle)
            direction = clockWise;

        if (angle >= maxAngle)
            direction = counterClockWise;
    }
}
