using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShield : MonoBehaviour
{
    private PlayerAttributes player_attributes;
    public GameObject shield;
    private Server server;
    private GameAttributes game_attributes;

    // Start is called before the first frame update
    void Start()
    {
        player_attributes = transform.GetComponent<PlayerAttributes>();
        server = GameObject.Find("Server").GetComponent<Server>();
        game_attributes = GameObject.Find("Game").GetComponent<GameAttributes>();
    }

    // Update is called once per frame
    void Update()
    {
        if(player_attributes.key_shield && player_attributes.shield_ready){
            StartCoroutine(shieldReloadingDuration(game_attributes.shield_cooldown_duration));
            StartCoroutine(shieldActivateOverDuration(game_attributes.shield_duration));
        }
        player_attributes.key_shield = false;
        
    }

    IEnumerator shieldActivateOverDuration(float duration)
    {
        player_attributes.shield_active = true;
        shield.SetActive(true);
        // inform player that his shield became active
        Packet packet = PacketManufacturer.playerShieldActive(player_attributes.id);
        server.sendMessageBroadcastReliable(packet);

        yield return new WaitForSeconds(duration);

        // it was possibly already set to false after blocking a projectile
        if(player_attributes.shield_active){
            player_attributes.shield_active = false;
            shield.SetActive(false);
            // inform player that his shield became inactive
            packet = PacketManufacturer.playerShieldActive(player_attributes.id);
            server.sendMessageBroadcastReliable(packet);
        }
        
        yield return null;
    }

    IEnumerator shieldReloadingDuration(float duration)
    {
        player_attributes.shield_ready = false;
        // inform respective player that his shield is on cd now
        Packet packet = PacketManufacturer.playerShieldReady(player_attributes.id);
        server.sendMessageReliable(player_attributes.id,packet);

        yield return new WaitForSeconds(duration);

        player_attributes.shield_ready = true;
        // inform respective player that his shield is ready again
        packet = PacketManufacturer.playerShieldReady(player_attributes.id);
        server.sendMessageReliable(player_attributes.id,packet);

        yield return null;
    }
}
