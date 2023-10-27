using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSTexture : MonoBehaviour
{
    public bool activeDebug = true;
    public int textureSize = 256;
    public Gradient colorGradientTest;
    public Vector2 texturePosition;

    private Rect m_rect;
    private Texture2D m_fpsTexture;
    public int[] m_fpsCount = new int[100];
    private int m_textureHeight = 100;
    private int m_textureWidth = 100;
    private int m_currentFps;

     private float m_fpsRatio = 0;
    private Color m_colorToUse;
    private Color m_colorBase;
    private Color m_transparent = new Color(0, 0, 0, 0);
    
    // TODO : 
    // 3. Optimize 

    // Start is called before the first frame update
    void Start()
    {
        m_textureWidth = m_textureHeight = textureSize;
        m_rect.x = texturePosition.x;
        m_rect.y = texturePosition.y;
        m_rect.width = m_textureWidth;
        m_rect.height = m_textureHeight;
        m_fpsCount = new int[textureSize];
        m_fpsTexture = new Texture2D(textureSize, textureSize);

    }

    // Update is called once per frame
    void Update()
    {
        if (!activeDebug) return;

       

        m_currentFps = (int)(1.0f / Time.deltaTime);
        if (m_currentFps > 90) m_fpsRatio = 90;
        // Drawing the result in a texture

        for (int width = 0; width < m_textureWidth; width++)
        {
            // Counting fps in array
            if (width != m_fpsCount.Length - 1) m_fpsCount[width] = m_fpsCount[width + 1];
            else m_fpsCount[m_fpsCount.Length - 1] = m_currentFps;

             m_fpsRatio = (m_fpsCount[width] / 90.0f);
           
            int pixelsToDraw = (int)((1.0f-m_fpsRatio) * m_textureHeight);
            m_colorBase = colorGradientTest.Evaluate(1.0f-m_fpsRatio);


            for (int height = 0; height < m_textureHeight; height++)
            {
                m_colorToUse = m_colorBase;
                if (height == (int)(m_textureHeight * .6f) || height == (int)(m_textureHeight * .3f) || height == (int)(m_textureHeight * .3f) + 1)
                {
                    m_colorToUse = Color.white;
                }
              
                if (height >= pixelsToDraw)
                {
                    m_colorToUse = m_transparent;
                 
                }

                
                m_fpsTexture.SetPixel(width, height, m_colorToUse);
            }
        }

        m_fpsTexture.Apply();
    }

    private void OnGUI()
    {
        if (Event.current.type != EventType.Repaint || !activeDebug) return;

        Graphics.DrawTexture(m_rect, m_fpsTexture);
    }

}
