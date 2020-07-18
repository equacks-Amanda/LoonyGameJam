using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(EventManager.instance != null)
        {
            EventManager.instance.QueueEvent(new EnvioEvents.SpawnerCreated(this.transform));
        }
    }
}
