using UnityEngine;
using System.Collections;

public class SetStartVolume : MonoBehaviour {

    private MusicManager musicManager;
    // Use this for initialization
    void Start () {
        musicManager = GameObject.FindObjectOfType<MusicManager>();
        if (musicManager)
        {
            musicManager.ChangeVolume(PlayerPrefsManager.GetMasterVolume());
        }
        else
        {
            Debug.LogError("Can't find music Manager!");
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
