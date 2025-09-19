using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BackgroundMusic : MonoBehaviour
{
    void Awake(){
        DontDestroyOnLoad(gameObject);
        var src = GetComponent<AudioSource>();
        src.loop = true;
        if (!src.isPlaying) src.Play();
    }
}