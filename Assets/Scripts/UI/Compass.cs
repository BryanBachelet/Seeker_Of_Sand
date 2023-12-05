using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
public class Compass : MonoBehaviour
{
    public GameObject iconPrefab;
    List<QuestMarker> questMarkers = new List<QuestMarker>();
    private Material m_boussoleMaterial;
    public RawImage m_boussole;
    public Vector2 offsetMaterial;

    public RawImage compassImage;
    public Transform player;

    public float maxDistance = 200f;

    float compassUnit;

    public QuestMarker one;
    public QuestMarker two;
    public QuestMarker three;

    private string offSetPropertyID;
    // Start is called before the first frame update
    void Start()
    {
        compassUnit = compassImage.rectTransform.rect.width / 360f;
       
        m_boussoleMaterial = m_boussole.material;
    }

    // Update is called once per frame
    void Update()
    {
        compassImage.uvRect = new Rect(0.5f + player.localEulerAngles.y / 360, 0, 1, 1);
        m_boussoleMaterial.SetVector("_Tilling", new Vector2(0.5f + -player.localEulerAngles.y / 360, 0));
        foreach (QuestMarker marker in questMarkers)
        {
            marker.image.rectTransform.anchoredPosition = GetPosOnCompass(marker) + new Vector2(0,-30);

            float dst = Vector2.Distance(new Vector2(player.transform.position.x, player.transform.position.z), marker.position);
            float scale = 0;

            if(dst < maxDistance)
            {
                scale = 1f - (dst / maxDistance);


            }
            marker.image.rectTransform.localScale = Vector3.one * scale;
            marker.image.color = new Color(1, 1, 1, scale);
        }
    }

    public void AddQuestMarker (QuestMarker marker)
    {
        GameObject newMArker = Instantiate(iconPrefab, compassImage.transform);
        marker.image = newMArker.GetComponent<Image>();
        marker.image.sprite = marker.icon;

        questMarkers.Add(marker);
    }

    Vector2 GetPosOnCompass(QuestMarker marker)
    {
        Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.z);
        Vector2 playerFwd = new Vector2(player.transform.forward.x, player.transform.forward.z);

        float angle = Vector2.SignedAngle(marker.position - playerPos, playerFwd);

        return new Vector2(compassUnit * angle, 0f);
    }
}
