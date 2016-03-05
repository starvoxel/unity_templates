using UnityEngine;
using System.Collections;

public class ObstacleResizer : MonoBehaviour {

    private PolyNavObstacle m_Obstacle = null;
    private Collider2D m_Collider;

    [SerializeField]
    private float m_TimeScale;
    [SerializeField]
    private float m_Amplitude;
    [SerializeField]
    private float m_Offset;
    [SerializeField]
    private Vector2 m_Direction;
    private float m_ElapsedTime = 0.0f;
    private Vector2 m_StartPos = new Vector2(float.NaN, float.NaN);
    private Vector2 m_StartSize = new Vector2(float.NaN, float.NaN);

	// Use this for initialization
	void Start () {
        m_Obstacle = GetComponent<PolyNavObstacle>();
        m_Collider = GetComponent<Collider2D>();

        Debug.Assert(m_Obstacle != null && m_Collider != null);
	}
	
	// Update is called once per frame
	void Update () {
        m_ElapsedTime += Time.deltaTime;

	    if (m_Collider is BoxCollider2D)
        {
            BoxCollider2D boxCollider = m_Collider as BoxCollider2D;

            if (!IsValidVector2(m_StartSize))
            {
                m_StartSize = boxCollider.size;
            }

            boxCollider.size = m_StartSize + ((m_Amplitude * Mathf.Sin(m_ElapsedTime * m_TimeScale) + m_Offset) * m_Direction.normalized);
        }
        else if (m_Collider is PolygonCollider2D)
        {
            PolygonCollider2D polygonCollider = m_Collider as PolygonCollider2D;

            if (!IsValidVector2(m_StartSize))
            {
                m_StartPos = polygonCollider.GetPath(0)[0];
            }

            Vector2[] points = polygonCollider.GetPath(0);
            Vector2 sinResult = (m_Amplitude * Mathf.Sin(m_ElapsedTime * m_TimeScale) + m_Offset) * m_Direction.normalized;
            points[0] = m_StartPos + sinResult;
            polygonCollider.SetPath(0, points);
        }

        m_Obstacle.ForceRegenerate();
	}

    private bool IsValidVector2(Vector2 vector)
    {
        return !float.IsNaN(vector.x) && !float.IsNaN(vector.y);
    }
}
