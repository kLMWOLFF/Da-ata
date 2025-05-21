using UnityEngine;

public class ShakingMargin : MonoBehaviour
{

    public enum Direction
    {
        Neutral,
        Up,
        Down
    }

    public Direction direction = Direction.Neutral;

    public float margin = 10f;

    public float upAngle = 30f;

    public float downAngle = 150f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Calculate the angle between this object up axis and the world up axis
        float angle = Vector3.Angle(transform.up, Vector3.up);
        Debug.Log("Angle: " + angle);

        if (angle < upAngle - margin)
        {
            direction = Direction.Up;
        }
        else if (angle > downAngle + margin)
        {
            direction = Direction.Down;
        }
        else if (angle > upAngle + margin && angle < downAngle - margin)
        {
            direction = Direction.Neutral;
        }
        // Log the current direction

    }
}
