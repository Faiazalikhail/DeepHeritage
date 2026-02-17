using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class NewMonoBehaviourScript : MonoBehaviour
{

    public Transform Target;
    [Range(0, 1)] public float smoothTime = 0.3f;
    public float deadZoneX = 2f;
    public float minX = -15f;
    public float maxX = 195f;

    private Vector3 velocity = Vector3.zero;

    
    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
        // 2. We must use Capital T here...
        if (Target != null)
        {
            // 3. ...and here...
            float xDifference = Target.position.x - transform.position.x;

            if (Mathf.Abs(xDifference) > deadZoneX)
            {
                // 4. ...and here!
                float targetX = Target.position.x - (Mathf.Sign(xDifference) * deadZoneX);

                targetX = Mathf.Clamp(targetX, minX, maxX);

                Vector3 destination = new Vector3(targetX, transform.position.y, transform.position.z);
                transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, smoothTime);
            }
        }
    }
}
