using UnityEngine;
using System.Collections;

public class PlaneInput : MonoBehaviour 
{
	public int m_TextureSize = 1024;
	public Vector3 m_MousePosition = Vector3.zero; 
	public Vector2 m_MouseUVs = Vector3.zero;
	public float m_BrushRadius = 10.0f;

	public Gradient m_Colors;

	private Texture2D m_Texture = null;
	private float[] m_TexelValues;

	void Start()
	{
		m_Texture = new Texture2D(m_TextureSize, m_TextureSize);
		m_TexelValues = new float[m_TextureSize * m_TextureSize];

		Color[] colors = new Color[m_TextureSize * m_TextureSize];

		for(int i = 0; i < m_TextureSize * m_TextureSize; ++i)
			colors[i] = m_Colors.Evaluate(0.0f);

		m_Texture.SetPixels(colors);
		m_Texture.Apply();

		renderer.material.mainTexture = m_Texture;
	}

	void Update() 
	{
		m_MousePosition = Input.mousePosition;

		Ray ray = Camera.main.ScreenPointToRay(m_MousePosition);
		RaycastHit hit;

		if(Physics.Raycast(ray, out hit, float.PositiveInfinity))
		{
			m_MouseUVs = hit.textureCoord;

			UpdateTexture();
		}

		float scroll = Input.GetAxis("Mouse ScrollWheel");
		if(scroll != 0.0f)
		{
			m_BrushRadius += scroll * 100.0f;
			m_BrushRadius = Mathf.Clamp(m_BrushRadius, 5.0f, (float)m_TextureSize * 0.5f);
		}
	}

	void UpdateTexture()
	{
		Vector2 extents = Vector2.one * m_BrushRadius * 2.0f;
		Vector2 topLeft = m_MouseUVs * m_TextureSize;
		topLeft -= extents * 0.5f; 

		if((int)(topLeft.x + extents.x) > m_TextureSize || (int)(topLeft.y + extents.y) > m_TextureSize ||
		   (int)topLeft.x < 0.0f || (int)topLeft.y < 0.0f)
		{
			return;
		}

		Color[] colors = m_Texture.GetPixels((int)topLeft.x, (int)topLeft.y, (int)extents.x, (int)extents.y);

		for(int x = 0; x < (int)extents.x; ++x)
		{
			for(int y = 0; y < (int)extents.y; ++y)
			{
				Vector2 pos = new Vector2(x,y);
				pos.x = ((pos.x / extents.x) - 0.5f) * extents.x;
				pos.y = ((pos.y / extents.y) - 0.5f) * extents.y;

				float dist = Vector2.Distance(pos, Vector3.zero);

				if(dist < m_BrushRadius)
				{
					float falloff = 1.0f - (dist / m_BrushRadius);
					float value = m_TexelValues[((int)topLeft.x + x) + (((int)topLeft.y + y) * m_TextureSize)];
					value += Mathf.Clamp01(falloff * Time.deltaTime);

					colors[x + (y * (int)extents.x)] = m_Colors.Evaluate(value);
					m_TexelValues[((int)topLeft.x + x) + (((int)topLeft.y + y) * m_TextureSize)] = value;
				}
			}
		}

		m_Texture.SetPixels((int)topLeft.x, (int)topLeft.y, (int)extents.x, (int)extents.y, colors);
		m_Texture.Apply();
	}
}
