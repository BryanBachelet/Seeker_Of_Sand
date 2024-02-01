using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpSignOnUI : MonoBehaviour
{
    public GameObject[] levelUpSign = new GameObject[5];
    public GameObject[] levelUpSignVfx = new GameObject[2];
    private bool vfxIsActive = false;
    private int lastLevelUpTake = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(lastLevelUpTake != CharacterUpgrade.upgradePoint)
        {
            lastLevelUpTake = CharacterUpgrade.upgradePoint;
            if(!vfxIsActive)
            {
                vfxIsActive = true;
                StartCoroutine(ActiveLevelUpSign(1));
            }
            if (lastLevelUpTake <= 0)
            {
                //ActiveVFX(lastLevelUpTake);
                DesactiveVFX(lastLevelUpTake);
                if (vfxIsActive)
                {
                    vfxIsActive = false;
                    StartCoroutine(DesactiveLevelUpSign(1));
                }
            }
            if(lastLevelUpTake < 6)
            {
                ActiveVFX(lastLevelUpTake);
                DesactiveVFX(lastLevelUpTake);
            }
            else
            {
                ActiveVFX(5);
            }


        }

    }

    public IEnumerator ActiveLevelUpSign(float time)
    {
        levelUpSignVfx[0].SetActive(true);
        yield return new WaitForSeconds(time);
        levelUpSignVfx[1].SetActive(true);
    }
    public IEnumerator DesactiveLevelUpSign(float time)
    {
        levelUpSignVfx[0].SetActive(false);
        yield return new WaitForSeconds(time);
        levelUpSignVfx[1].SetActive(false);
    }
    public void ActiveVFX(int level)
    {
        for(int i = 0; i < level; i++)
        {
            levelUpSign[i].SetActive(true);
        }
    }
    public void DesactiveVFX(int level)
    {
        for(int i = 5; i > level; i--)
        {
            levelUpSign[i - 1].SetActive(false);
        }
    }
}
