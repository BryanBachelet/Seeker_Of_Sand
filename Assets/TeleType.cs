using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TeleType : MonoBehaviour
{
    private TextMeshPro m_textMeshPro;
    public bool activeTeleType = false;
    // Start is called before the first frame update
    void Start()
    {
        m_textMeshPro = this.GetComponent<TextMeshPro>();
        StartCoroutine(StartTeleType());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator StartTeleType()
    {
        int totalVisibleCharacters = m_textMeshPro.textInfo.characterCount;
        int counter = 0;

        while (activeTeleType)
        {
            int visibleCount = counter % (totalVisibleCharacters + 1);
            m_textMeshPro.maxVisibleCharacters = visibleCount;

            if (visibleCount >= totalVisibleCharacters)
                yield return new WaitForSeconds(1.0f);

            counter += 1;

            yield return new WaitForSeconds(0.05f);
        }
    }
}
