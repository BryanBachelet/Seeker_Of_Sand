using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class FogOfWarScript : MonoBehaviour {
	
	public GameObject m_fogOfWarPlane;
	public Transform m_player;
	public LayerMask m_fogLayer;
	public float m_radius = 5f;
	private float m_radiusSqr { get { return m_radius*m_radius; }}
	
	private Mesh m_mesh;
	private Vector3[] m_vertices;
	private Color m_refernceMatColor;
	private Color[] m_colors;

	public int currentZoomLevel;
	public int maxZoomLevel = 3;

	public Vector3[] zoomLevelPosition = new Vector3[3];
	public Vector3[] zoomLevelIcon = new Vector3[3];
	private ObjectState state;
	public GameObject playerPositionIcon;
	// Use this for initialization
	void Start () {
		Initialize();
		state = new ObjectState();
		GameState.AddObject(state);
	}
	
	// Update is called once per frame
	void Update () {
		if(currentZoomLevel == 0)
        {
			transform.position = zoomLevelPosition[0];
			playerPositionIcon.transform.localScale = zoomLevelIcon[0];
		}
		else
        {
			transform.position = new Vector3(m_player.position.x, zoomLevelPosition[currentZoomLevel].y, m_player.position.z);
			playerPositionIcon.transform.localScale = zoomLevelIcon[currentZoomLevel];
		}

		Ray r = new Ray(transform.position, m_player.position - transform.position);
		RaycastHit hit;
		if (Physics.Raycast(r, out hit, 3000, m_fogLayer, QueryTriggerInteraction.Collide)) {
			for (int i=0; i< m_vertices.Length; i++) {
				Vector3 v = m_fogOfWarPlane.transform.TransformPoint(m_vertices[i]);
				float dist = Vector3.SqrMagnitude(v - hit.point);
				if (dist < m_radiusSqr) {
					float alpha = Mathf.Min(m_colors[i].a, dist/m_radiusSqr);
					m_colors[i].a = alpha;
				}
			}
			Debug.DrawRay(transform.position, m_player.position - transform.position);
			UpdateColor();
		}
        else
        {
			//Debug.DrawRay(transform.position, m_player.position - transform.position);
        }
	}
	
	void Initialize() {
		m_mesh = m_fogOfWarPlane.GetComponent<MeshFilter>().mesh;
		m_refernceMatColor = m_fogOfWarPlane.GetComponent<MeshRenderer>().material.GetColor("_TintColor");
		m_vertices = m_mesh.vertices;
		m_colors = new Color[m_vertices.Length];
		for (int i=0; i < m_colors.Length; i++) {
			m_colors[i] = m_refernceMatColor;
		}
		UpdateColor();
	}
	
	void UpdateColor() {
		m_mesh.colors = m_colors;
	}

	public void ZoomIn(InputAction.CallbackContext ctx)
    {
		if (ctx.performed && state.isPlaying)
		{
			if (currentZoomLevel == maxZoomLevel) return;
			else
			{
				currentZoomLevel += 1;
			}
		}

    }

	public void ZoomOut(InputAction.CallbackContext ctx)
	{
		if (ctx.performed && state.isPlaying)
		{
			if (currentZoomLevel == 0) return;
			else
			{
				currentZoomLevel -= 1;
			}
		}

	}
}
