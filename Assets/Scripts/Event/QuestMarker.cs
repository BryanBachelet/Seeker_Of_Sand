using UnityEngine;
using UnityEngine.UI;
public class QuestMarker : MonoBehaviour
{
    public Sprite icon;
    public Image image;

    public Sprite iconReward;
    public Sprite iconEventActive;
    public Sprite iconEventFinish;
    public Sprite iconEventFailed;

    public Sprite[] state;
    // Start is called before the first frame update
    void Start()
    {
        //state
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector2 position
    {
        get { return new Vector2(transform.position.x, transform.position.z); }
    }
}
