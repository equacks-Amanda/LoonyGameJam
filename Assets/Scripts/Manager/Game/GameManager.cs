
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class GameManager: MonoSingleton<GameManager>
{
    #region Properties
 
    [Header("Player Information")]
    [SerializeField]
    PlayerStats playerInfo;

    [SerializeField]
    PlayerView playerPrefab;

    [Header("Camera Set Up")]
    [SerializeField]
    CameraFollowPlayer follower;

    [SerializeField]
    Vector2 cameraFollowOffset;

    [Header("Sound Set Up")]
    [Range(0,1)]
    [SerializeField]
    float musicVolume = .5f;

    [SerializeField]
    AudioClip musicClip;

    #endregion

    #region StartUp
    public void Initialize()
    {
        //Subscribe
        Subscribe();
        //Load Scene
        StartCoroutine(LoadAsyncFirstScene());
        follower.offSet = cameraFollowOffset;
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

    private void OnSpawnFound(EnvioEvents.SpawnerCreated @event)
    {
        PlayerController pCtrl = new PlayerController(playerPrefab, playerInfo, @event.spawnerLoc);
        AudioSource audioSrc = gameObject.AddComponent<AudioSource>();
        audioSrc.volume = musicVolume;
        audioSrc.clip = musicClip;
        audioSrc.Play();
    }

    #endregion
}
