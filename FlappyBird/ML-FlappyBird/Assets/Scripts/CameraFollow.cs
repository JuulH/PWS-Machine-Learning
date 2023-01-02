using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    private Vector3 velocity = Vector3.zero;
    [SerializeField] private float smoothSpeed;
    public Vector2 target;
    private Vector2 desiredPos;
    public bool followTarget;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(followTarget)
        {
            desiredPos = new Vector3(target.x, target.y, -10);
        } else
        {
            desiredPos = new Vector3(0, 0, -10);
        }

        transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref velocity, smoothSpeed);
    }
}
