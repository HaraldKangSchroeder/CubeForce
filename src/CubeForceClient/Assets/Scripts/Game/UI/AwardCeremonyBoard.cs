using UnityEngine;
using UnityEngine.UI;

public class AwardCeremonyBoard : MonoBehaviour
{
    
    public GameObject player_stats_field;
    public GameObject player_stats_prefab;
    public GameObject to_lobby_button;
    private int start_top = -13;
    private int step_size = 6;

    public void openBoard(){
        Game game = GameObject.Find("Game").GetComponent<Game>(); 
        gameObject.SetActive(true);
        for(int i = 0; i < game.players.Length; i++){
            if(game.players[i] != null){
                GameObject player_stats_object = Instantiate(player_stats_prefab);
                PlayerAttributes player_attributes = game.players[i].transform.GetChild(0).GetComponent<PlayerAttributes>();
                player_stats_object.GetComponent<RectTransform>().localPosition  = new Vector3(0, start_top - i * step_size,0);
                player_stats_object.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = player_attributes.name;
                player_stats_object.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = player_attributes.kills.ToString();
                player_stats_object.transform.GetChild(2).GetComponent<TMPro.TextMeshProUGUI>().text = player_attributes.wins.ToString();
                player_stats_object.transform.SetParent(player_stats_field.transform,false);
            }
        }
    }

    public void closeBoard(){
        gameObject.SetActive(false);
        foreach (Transform child in player_stats_field.transform) {
            GameObject.Destroy(child.gameObject);
        }
    }


    public void toLobby(){
        Packet packet = PacketManufacturer.toLobby(Client.id);
        GameObject.Find("Client").GetComponent<Client>().sendMessageReliable(packet);
    }
}
