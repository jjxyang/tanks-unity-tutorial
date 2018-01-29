using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float m_DampTime = 0.2f;                 // approx. time we want to take for the camera to move to where it should be
    public float m_ScreenEdgeBuffer = 4f;           // "padding"
    public float m_MinSize = 6.5f;                  // min size of camera, not suuuper zoomed in
    /*[HideInInspector]*/ public Transform[] m_Targets; // array of transforms for the tanks


    private Camera m_Camera;                        
    private float m_ZoomSpeed;                      // how fast we're zooming; help damp camera
    private Vector3 m_MoveVelocity;                 // how fast we're moving ; help damp camera
    private Vector3 m_DesiredPosition;              // where we want camera to be (want avg of position of the two tanks);
                                                    //    we'll zoom to this position, not original position


    private void Awake()
    {
        m_Camera = GetComponentInChildren<Camera>();  // finds first camera out of children
    }


    private void FixedUpdate()
    {
        // we use fixedupdate because tanks are moving like physics objects;
        //     we want to keep things in sync
        Move();
        Zoom();
    }


    private void Move()
    {
        // avg position between tanks
        FindAveragePosition();
        // smoothly transition between current and desired position;
        //     note: `ref` means "write back to this variable"
        transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
    }


    private void FindAveragePosition()
    {
        Vector3 averagePos = new Vector3();
        int numTargets = 0;

        // counts # of active targets, sum of positions
        for (int i = 0; i < m_Targets.Length; i++)
        {
            if (!m_Targets[i].gameObject.activeSelf)
                continue;

            averagePos += m_Targets[i].position;
            numTargets++;
        }

        if (numTargets > 0)
            averagePos /= numTargets;

        // don't change y-component of the rig, don't want it to leave the ground
        averagePos.y = transform.position.y;

        m_DesiredPosition = averagePos;
    }


    private void Zoom()
    {
        float requiredSize = FindRequiredSize();
        m_Camera.orthographicSize = Mathf.SmoothDamp(m_Camera.orthographicSize, requiredSize, ref m_ZoomSpeed, m_DampTime);
    }


    private float FindRequiredSize()
    {
        Vector3 desiredLocalPos = transform.InverseTransformPoint(m_DesiredPosition);

        float size = 0f;

        // looks for the active tank that's furthest away from center
        for (int i = 0; i < m_Targets.Length; i++)
        {
            if (!m_Targets[i].gameObject.activeSelf)
                continue;

            Vector3 targetLocalPos = transform.InverseTransformPoint(m_Targets[i].position);

            // using the local space of camera rig
            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;

            // check if current tank is further from center than previous tanks along y-axis
            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.y));

            // check if current tank is further from center along x-axis
            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.x) / m_Camera.aspect);
        }
        
        size += m_ScreenEdgeBuffer;         // add "padding" to edges
        size = Mathf.Max(size, m_MinSize);  // stay at least a min size
        return size;
    }

    
    // usable by a game manager
    // reset each round, BUT don't want to smoothly move to that position, avoid damping
    public void SetStartPositionAndSize()
    {
        FindAveragePosition();

        transform.position = m_DesiredPosition;

        m_Camera.orthographicSize = FindRequiredSize();
    }
}