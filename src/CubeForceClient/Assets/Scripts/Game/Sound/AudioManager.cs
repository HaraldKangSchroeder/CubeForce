using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{

    public Sound[] sounds;

    private Game game;

    private Sound background_sound;

    private float volume_scale = 1f;


    void Awake()
    {
        game = GameObject.Find("Game").GetComponent<Game>();
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        playBackgroundSound();
        game.music_start_time = System.DateTime.Now.Second;
    }

    public void Play(string name){
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.volume = s.volume * volume_scale;
        s.source.Play();
    }

    private void playBackgroundSound(){
        background_sound = Array.Find(sounds, sound => sound.name == "BackgroundSound");
        background_sound.source.volume = background_sound.volume * volume_scale;
        background_sound.source.Play();
    }

    private void changeVolumeBackgroundSound(float scale){
        background_sound.source.volume = background_sound.volume * scale;
    }

    public void changeVolume(float scale){
        volume_scale = scale;
        changeVolumeBackgroundSound(volume_scale);
    }
}
