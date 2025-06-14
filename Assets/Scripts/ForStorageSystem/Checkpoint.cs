using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Checkpoint : MonoBehaviour
{
    private Transform player;
    public int index;

    void Awake()
    {
        player = GameObject.Find("Player").transform;
        if (DataContainer.checkpointIndex == index)
        {
            player.position = transform.position;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger entered by: " + other.name);
        if (other.CompareTag("Player"))
        {
            if (index > DataContainer.checkpointIndex)
            {
                DataContainer.checkpointIndex = index;
            }
            
        }
    }
}
