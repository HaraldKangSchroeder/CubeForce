using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateFrequencyTracker : MonoBehaviour
{
    private List<int> updates = new List<int>();
    private int update_counter = 0;

    private void Update() {
        update_counter++;
    }

    public void reset(){
        if(updates.Count >= 10){
            updates.RemoveAt(0);
        }
        updates.Add(update_counter);
        update_counter = 0;
    }

    public int getAverageUpdateSteps(){
        if(updates.Count == 0) return 1;
        int sum = 0;
        for(int i = 0; i < updates.Count; i++){
            sum += updates[i];
        }
        return Mathf.Max(sum/updates.Count,1);
    }
}
