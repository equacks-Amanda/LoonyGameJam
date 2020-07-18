
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager: MonoSingleton<GameManager>
{
    #region Properties
    List<Obstacle> currentLevelObstacles = new List<Obstacle>();

    [SerializeField]
    PlayerStats playerInfo;

    [SerializeField]
    PlayerView playerPrefab;


    #endregion

    #region StartUp
    public void Initialize()
    {
        //Subscribe
        Subscribe();
        //Load Scene
        StartCoroutine(LoadAsyncFirstScene());
    }

    IEnumerator LoadAsyncFirstScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("LEVEL-1", LoadSceneMode.Additive);
        while(!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    void Subscribe()
    {
        if(EventManager.instance != null)
        {
            //Subscribe to player / game events
            EventManager.instance.AddListener<EnvioEvents.SpawnerCreated>(OnSpawnFound);
        }
    }

    #endregion

    #region Ready Scene Change

    private void FindObstaclesInScene()
    {
        currentLevelObstacles = new List<Obstacle>(FindObjectsOfType<Obstacle>());
        if(currentLevelObstacles.Count <= 0 )
        {
            Debug.LogWarning("FOIS - NONE FOUND - CORRECT?");
        }
    }

    private void OnSpawnFound(EnvioEvents.SpawnerCreated @event)
    {
        PlayerController pCtrl = new PlayerController(playerPrefab, playerInfo, @event.spawnerLoc);
    }

    #endregion
}
