using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public bool pause_menu_open = false;
    public bool options_menu_open = false;

    public GameObject pause_menu_panel;
    public GameObject options_menu_panel;
    public Slider slider;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)){
            if(pause_menu_open){
                Resume();
            }
            else if(options_menu_open){
                SetActivePauseMenu(true);
                SetActiveOptionsMenu(false);
            }
            else{
                showPause();
            }
        }
    }

    public void Resume(){
        SetActivePauseMenu(false);
    }

    public void showPause(){
        SetActivePauseMenu(true);
        SetActiveOptionsMenu(false);
    }


    public void LoadMenu(){
        Debug.Log("Load Menu");
        Client.close();
        Scene scene = SceneManager.GetActiveScene(); 
        SceneManager.LoadScene(scene.name);
    }

    public void QuitGame(){
        Debug.Log("Quit Game");
        Application.Quit();
    }

    public void showOptions(){
        SetActivePauseMenu(false);
        SetActiveOptionsMenu(true);
    }

    public void SetActivePauseMenu(bool active){
        pause_menu_open = active;
        pause_menu_panel.SetActive(active);
    }

    public void SetActiveOptionsMenu(bool active){
        options_menu_open = active;
        options_menu_panel.SetActive(active);
    }

    public void ChangeVolume(){
        GameObject.Find("AudioManager").transform.GetComponent<AudioManager>().changeVolume(slider.value);
    }
}
