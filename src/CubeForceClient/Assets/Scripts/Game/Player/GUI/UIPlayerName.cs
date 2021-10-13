using UnityEngine;
using System.Collections;

public class UIPlayerName : MonoBehaviour
{
    public GUISkin guiSkin; // choose a guiStyle (Important!)


    public float fontSize;
    public float offsetY;

    float boxW = 150f;
    float boxH = 20f;

    public bool messagePermanent = true;

    public float messageDuration { get; set; }

    Vector2 boxPosition;
    void Start()
    {
        if(messagePermanent)
        {
            messageDuration = 1;
        }
    }
    void OnGUI()
    {
        if(messageDuration > 0)
        {
            if(!messagePermanent) // if you set this to false, you can simply use this script as a popup messenger, just set messageDuration to a value above 0
            {
                messageDuration -= Time.deltaTime;
            }

            GUI.skin = guiSkin;
            boxPosition = Camera.main.WorldToScreenPoint(transform.position);
            boxPosition.y = Screen.height - boxPosition.y;
            boxPosition.y -= boxH * 0.5f;
            guiSkin.box.fontSize = 10;
            
            if(transform.GetComponent<PlayerAttributes>().id == Client.id){
                GUI.contentColor = Color.yellow;
            }
            else{
                GUI.contentColor = Color.white;
            }
            

            Vector2 content = guiSkin.box.CalcSize(new GUIContent(transform.GetComponent<PlayerAttributes>().name));

            GUI.Box(new Rect(boxPosition.x - content.x/2f, boxPosition.y + offsetY, content.x, content.y), transform.GetComponent<PlayerAttributes>().name);
        }
    }
}