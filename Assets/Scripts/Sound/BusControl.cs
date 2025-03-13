using FMODUnity;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.UI;
public class BusControl : MonoBehaviour
{

    static bool vcaMain_setup = false;
    private VCA vcaMain;
    public string vcaMain_Path;
    [Range(0, 10)] public float vcaMain_Volume = 5;

    static bool vcaMusicAmbiant_setup = false;
    private static VCA vcaMusicAmbiant;
    public string vcaMusicAmbiant_Path;
    [Range(0, 10)] public float vcaMusicAmbiant_Volume = 5;


    static bool vcaSFX_setup = false;
    private VCA vcaSFX;
    public string vcaSFX_Path;
    [Range(0, 10)] public float vcaSFX_Volume = 5;

    static bool vcaEntity_setup = false;
    private VCA vcaEntity;
    public string vcaEntity_Path;
    [Range(0, 10)] public float vcaEntity_Volume = 5;
    // Start is called before the first frame update
    void Start()
    {
        if (!vcaMain_setup) vcaMain = RuntimeManager.GetVCA("VCA:/" + vcaMain_Path);
        if (!vcaMusicAmbiant_setup) vcaMusicAmbiant = RuntimeManager.GetVCA("VCA:/" + vcaMusicAmbiant_Path);
        if (!vcaSFX_setup) vcaSFX = RuntimeManager.GetVCA("VCA:/" + vcaSFX_Path);
        if (!vcaEntity_setup) vcaEntity = RuntimeManager.GetVCA("VCA:/" + vcaEntity_Path);

        updateVolume();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnValidate()
    {
        updateVolume();
    }
    public void updateVolume()
    {
        vcaMain.setVolume(vcaMain_Volume / 10);
        vcaMusicAmbiant.setVolume(vcaMusicAmbiant_Volume / 10);
        vcaSFX.setVolume(vcaSFX_Volume / 10);
        vcaEntity.setVolume(vcaEntity_Volume / 10);
    }

    public void updateMain(Slider slider)
    {
        float volume = slider.value * 10;
        if (volume > 10) volume = 10;
        else if (volume < 0) volume = 0;

        vcaMain_Volume = volume;
        updateVolume();
    }

    public void updateMusicAmbiant(Slider slider)
    {
        float volume = slider.value * 10;
        if (volume > 10) volume = 10;
        else if (volume < 0) volume = 0;

        vcaMusicAmbiant_Volume = volume;
        updateVolume();
    }

    public void updateSFX(Slider slider)
    {
        float volume = slider.value * 10;
        if (volume > 10) volume = 10;
        else if (volume < 0) volume = 0;

        vcaSFX_Volume = volume;
        updateVolume();
    }

    public void updateEntity(Slider slider)
    {
        float volume = slider.value * 10;
        if (volume > 10) volume = 10;
        else if (volume < 0) volume = 0;

        vcaEntity_Volume = volume;
        updateVolume();
    }

}
