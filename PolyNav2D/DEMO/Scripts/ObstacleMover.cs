using UnityEngine;
using System.Collections;

public class ObstacleMover : MonoBehaviour {

    [SerializeField]
    private float m_TimeScale;
    [SerializeField]
    private float m_Amplitude;
    [SerializeField]
    private float m_Offset;
    [SerializeField]
    private Vector3 m_Direction;
    private float m_ElapsedTime = 0.0f;
    private Vector3 m_StartPos;

	// Use this for initialization
	void Start () {
        m_StartPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        m_ElapsedTime += Time.deltaTime;

        transform.position = m_StartPos + ((m_Amplitude * Mathf.Sin(m_ElapsedTime * m_TimeScale) + m_Offset) * m_Direction.normalized);
	}
}
