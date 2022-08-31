using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OptionsController : MonoBehaviour
{

    public Slider volumeSlider, difficultySlider;
    public LevelManager levelManager;
    private MusicManager musicManager;

    // Use this for initialization
    void Start()
    {
        musicManager = GameObject.FindObjectOfType<MusicManager>();
        volumeSlider.value = PlayerPrefsManager.GetMasterVolume();
    }

    // Update is called once per frame
    void Update()
    {
        musicManager.ChangeVolume(volumeSlider.value);
    }

    public void SaveAndExit()
    {
        PlayerPrefsManager.SetMasterVolume(volumeSlider.value);
        levelManager.LoadLevel(1);
    }

    public void SetDefault()
    {
        volumeSlider.value = 0.8f;
        difficultySlider.value = 2f;

    }
}
