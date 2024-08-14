using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SettingOptions : MonoBehaviour
{
    private int currentMenu = 0;
    public TMPro.TMP_Text[] mainSectionName = new TMP_Text[4];
    public GameObject[] sectionGameObjet = new GameObject[4];
    //[SerializeField] private textmes
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        ChangeCurrentSelection(0);
    }
    public void ChangeCurrentSelection(int newMenu)
    {
        if (newMenu == currentMenu) return;
        else
        {
            mainSectionName[currentMenu].fontStyle = FontStyles.Normal;
            mainSectionName[newMenu].fontStyle = FontStyles.Bold;
            sectionGameObjet[currentMenu].SetActive(false);
            sectionGameObjet[newMenu].SetActive(true);
            currentMenu = newMenu;

        }
    }
}
