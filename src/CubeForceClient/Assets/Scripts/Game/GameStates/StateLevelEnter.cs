using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateLevelEnter : MonoBehaviour, IState
{
    public GameObject occluders;
    public StateID state_id;
    public Color light_color;

    public Texture2D crosshair;



    private Game game;

    private Coroutine coroutine;

    public void Awake(){
        game = GameObject.Find("Game").GetComponent<Game>();
    }

    public void enter(){
        Debug.Log("Enter state : " + state_id);
        game.occluders = Instantiate(occluders, new Vector3(0,35,0), Quaternion.identity);
        game.occluders.GetComponent<Animator>().Play("OccludersFadeIn");

        Cursor.SetCursor(crosshair,new Vector3(16,16),CursorMode.ForceSoftware);

        StartCoroutine("changeColor");
    }

    public void execute(){
        // start animation on arena and players and set finish true when animation is done

        //debug (simulates animation time)
        // if(!coroutine_running){
        //     coroutine_running = true;
        //     StartCoroutine(FinishWithDelay(finish_delay));
        // }
    }

    public void exit(){
        Debug.Log("Exit state : " + state_id);
    }

    


    public StateID getStateID(){
        return state_id;
    }

    private IEnumerator changeColor(){
        Light light0 = GameObject.Find("Light0").GetComponent<Light>();
        Light light1 = GameObject.Find("Light1").GetComponent<Light>();
        Light light2 = GameObject.Find("Light2").GetComponent<Light>();
        Light light3 = GameObject.Find("Light3").GetComponent<Light>();

        Color color_light0_prev = light0.color;
        Color color_light1_prev = light1.color;
        Color color_light2_prev = light2.color;
        Color color_light3_prev = light3.color;

        int interpolation_steps = 40;
        for(int i = 0; i < interpolation_steps; i++){
            light0.color = Color.Lerp(color_light0_prev,light_color * 0.95f,i/(float)interpolation_steps);
            light1.color = Color.Lerp(color_light1_prev,light_color * 1.15f,i/(float)interpolation_steps);
            light2.color = Color.Lerp(color_light2_prev,light_color * 0.95f,i/(float)interpolation_steps);
            light3.color = Color.Lerp(color_light3_prev,light_color * 1.15f,i/(float)interpolation_steps);
            yield return new WaitForSeconds(0.1f);
        }
    }


}
