/*
 * Author: Wyatt Murray
 * Version: 4
 * Date: 10/20/23
 * 
 * Description:
 * Manages asynchronous loading and unloading of scenes with fade transitions.
 * 
 * Setup:
 * To set up in your project, create a scene with this script on an empty GameObject. 
 * Create a canvas under it and add a panel that covers the full screen and is colored black.
 * Remove all other scenes' EventSystems except this one.
 * Load the necessary data into the Inspector as needed and link the fade image and canvas.
 * Provide a fade duration of at least 0.25 seconds.
 * 
 * Additional Info:
 * - Ensure that referenced objects are available to call the two public methods.
 * - Remember that data must be preloaded unless you want to use GameObject.Find.
 * - This script is designed for loading single scenes with a player scene attached.
 * - There may be future improvements to support loading multiple scenes simultaneously.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.Burst;

public class AsyncLoader : MonoBehaviour
{
    #region Member Variables
    /// <summary>
    /// Struct for containing the data about all useable scenes in the game.
    /// </summary>
    [System.Serializable]
    public struct DefinedSceneData
    {
        /// <summary>
        /// The name of the Scene as a string.
        /// </summary>
        public SceneField m_scene;
        /// <summary>
        /// A toggle for labeling which scenes are persistant.
        /// </summary>
        public bool m_isPersistant;
    }

    [System.Serializable]
    public struct FadeData
    {
        /// <summary>
        /// The image used for fading transitions.
        /// </summary>
        public Image m_fadeImage;

        /// <summary>
        /// The Canvas GameObject associated with this loader.
        /// </summary>
        public GameObject m_canvas;

        /// <summary>
        /// The duration of the fade-in and fade-out transitions in seconds.
        /// </summary>
        public float m_fadeDuration;
    }


    /// <summary>
    /// A list of scene names to be used during loading and unloading operations.
    /// </summary>
    [SerializeField] private List<DefinedSceneData> m_sceneNames = new List<DefinedSceneData>();
    /// <summary>
    /// Public read access to the list of all scenes and their persistant tag.
    /// </summary>
    [SerializeField] private FadeData m_fadeData;
    public List<DefinedSceneData> SceneNames
    {
        get { return m_sceneNames; }
    }

    [SerializeField] private UnityEvent m_sceneLoaded = new UnityEvent();
    [SerializeField] private UnityEvent m_sceneUnloaded = new UnityEvent();
    [SerializeField] private UnityEvent UnloadingAllScenesExceptPersistant = new UnityEvent();


    /// <summary>
    /// Current state of the fade transition between scenes.
    /// </summary>
    private bool m_isFading = false;

    /// <summary>
    /// Public reference to the current state of the fade transition.
    /// </summary>
    public bool IsFading
    {
        get { return m_isFading; }
    }


    /// <summary>
    /// Float value of the current scene load operation.
    /// </summary>
    private float m_percentLoaded;

    /// <summary>
    /// Public reference to the current scene load operation for the automatic fade system.
    /// </summary>
    public float PercentLoaded
    {
        get { return m_percentLoaded; }
    }

    #endregion

    #region Monobehavior Methods
    private void Awake()
    {
        // Make sure the fadeImage is clear upon first load
        SetFadeAlpha(0.0f);
    }
    #endregion

    #region Overridabble Methods
    #region Load Single Scene Overrides
    /// <summary>
    /// Loads a single scene asynchronously.
    /// </summary>
    /// <param name="scene">The `SceneField` object that represents the scene to load.</param>
    /// <param name="loadSceneCoroutine">A `Coroutine` object that represents the asynchronous load operation.</param>
    [BurstCompile]
    public virtual void LoadSingleScene(SceneField scene, out Coroutine loadSceneCoroutine)
    {
        // Start the asynchronous load operation.
        Coroutine singleSceneload = StartCoroutine(LoadSceneAsyncAndWaitForCompletion(scene));
        // Return the reference to the asynchronous load operation.
        loadSceneCoroutine = singleSceneload;
    }

    /// <summary>
    /// Loads a single scene asynchronously, optionally unloading all scenes except for the persistent scene.
    /// </summary>
    /// <param name="scene">The `SceneField` object that represents the scene to load.</param>
    /// <param name="unloadAllExceptPersistant">A boolean value that indicates whether all scenes except for the persistent scene should be unloaded before the new scene is loaded.</param>
    /// <param name="loadSceneCoroutine">A `Coroutine` object that represents the asynchronous load operation.</param>
    [BurstCompile]
    public virtual void LoadSingleScene(SceneField scene, bool unloadAllExceptPersistant, out Coroutine loadSceneCoroutine)
    {
        // If unloadAllExceptPersistant is true, unload all scenes except for the persistent scene.
        if (unloadAllExceptPersistant)
        {
            UnloadAllScenesExceptPersistent();
        }
        // Start the asynchronous load operation.
        Coroutine singleSceneload = StartCoroutine(LoadSceneAsyncAndWaitForCompletion(scene));
        // Return the reference to the asynchronous load operation.
        loadSceneCoroutine = singleSceneload;
    }

    /// <summary>
    /// Loads a single scene asynchronously, optionally unloading all scenes except for the persistent scene and using a fade animation.
    /// </summary>
    /// <param name="scene">The `SceneField` object that represents the scene to load.</param>
    /// <param name="unloadAllExceptPersistant">A boolean value that indicates whether all scenes except for the persistent scene should be unloaded before the new scene is loaded.</param>
    /// <param name="loadSceneCoroutine">A `Coroutine` object that represents the asynchronous load operation.</param>
    /// <param name="useFade">A boolean value that indicates whether a fade animation should be used when loading the new scene.</param>
    [BurstCompile]
    public virtual void LoadSingleScene(SceneField scene, bool unloadAllExceptPersistant, out Coroutine loadSceneCoroutine, bool useFade = false)
    {
        // Start the asynchronous load operation.
        Coroutine singleSceneload = StartCoroutine(LoadSceneAsyncAndWaitForCompletion(scene));
        // If useFade is true, start a fade animation.
        Coroutine fade = null;
        if (useFade)
        {
            fade = StartCoroutine(FadeOutAndLoad(singleSceneload));
        }
        // Return the reference to the asynchronous load operation.
        loadSceneCoroutine = fade ?? singleSceneload;
    }
    #endregion

    #region Load Scene List Overrides
    /// <summary>
    /// Loads a list of scenes asynchronously.
    /// </summary>
    /// <param name="sceneList">A list of `SceneField` objects that represent the scenes to load.</param>
    /// <param name="loadList"> the output of the function, A `Coroutine` object that represents the asynchronous load operation.</param>
    [BurstCompile]
    public virtual void LoadSceneList(SceneField[] sceneList, out Coroutine loadList)
    {
        Coroutine listSceneLoad = StartCoroutine(LoadMultipleAsyncScenes(sceneList));
        loadList = listSceneLoad;
    }

    /// <summary>
    /// Loads a list of scenes asynchronously.
    /// </summary>
    /// <param name="sceneList">A list of `SceneField` objects that represent the scenes to load.</param>
    /// <param name="activeScene">The `SceneField` object that represents the scene that should be active after the load is complete.</param>
    /// <param name="loadList"> the output of the function, A `Coroutine` object that represents the asynchronous load operation.</param>
    [BurstCompile]
    public virtual void LoadSceneList(SceneField[] sceneList, SceneField activeScene, out Coroutine loadList)
    {
        //active scene should be set afterwards?
        IEnumerator MultiSceneLogic()
        {
            yield return StartCoroutine(LoadMultipleAsyncScenes(sceneList));
            yield return StartCoroutine(LoadActiveScene(activeScene));
        }
        Coroutine listSceneLoad = StartCoroutine(MultiSceneLogic());
        loadList = listSceneLoad;
    }

    /// <summary>
    /// Loads a list of scenes asynchronously.
    /// </summary>
    /// <param name="sceneList">A list of `SceneField` objects that represent the scenes to load.</param>
    /// <param name="activeScene">The `SceneField` object that represents the scene that should be active after the load is complete.</param>
    /// <param name="unloadAllExceptPersistant">A boolean value that indicates whether all scenes except for the persistent scene should be unloaded before the new scenes are loaded.</param>
    /// <param name="loadList"> the output of the function, A `Coroutine` object that represents the asynchronous load operation.</param>
    [BurstCompile]
    public virtual void LoadSceneList(SceneField[] sceneList, SceneField activeScene, bool unloadAllExceptPersistant, out Coroutine loadList)
    {
        if (unloadAllExceptPersistant)
        {
            // Unload all scenes except for the Persistent Scene (if needed)
            UnloadAllScenesExceptPersistent();
        }
        LoadSceneList(sceneList, activeScene, out loadList);
    }

    /// <summary>
    /// Loads a list of scenes asynchronously.
    /// </summary>
    /// <param name="sceneList">A list of `SceneField` objects that represent the scenes to load.</param>
    /// <param name="activeScene">The `SceneField` object that represents the scene that should be active after the load is complete.</param>
    /// <param name="unloadAllExceptPersistant">A boolean value that indicates whether all scenes except for the persistent scene should be unloaded before the new scenes are loaded.</param>
    /// <param name="useFade">A boolean value that indicates whether a fade animation should be used when loading the new scenes.</param>
    /// <param name="loadList"> the output of the function, A `Coroutine` object that represents the asynchronous load operation.</param>
    [BurstCompile]
    public virtual void LoadSceneList(SceneField[] sceneList, SceneField activeScene, bool unloadAllExceptPersistant, bool useFade, out Coroutine loadList)
    {
        //use the fade corotuine!
        LoadSceneList(sceneList, activeScene, unloadAllExceptPersistant, out loadList);
        StartCoroutine(FadeOutAndLoad(loadList));
    }
    #endregion

    #region Unload Scene methods
    /// <summary>
    /// Unloads a single scene asynchronously.
    /// </summary>
    /// <param name="scene">The `SceneField` object that represents the scene to unload.</param>
    /// <param name="unloadSceneCoroutine">A `Coroutine` object that represents the asynchronous unload operation.</param>
    [BurstCompile]
    public virtual void UnloadSingleScene(SceneField scene, out Coroutine unloadSceneCoroutine)
    {
        // Start the asynchronous unload operation.
        Coroutine singleSceneUnload = StartCoroutine(UnloadScene(scene));
        // Return the reference to the asynchronous unload operation.
        unloadSceneCoroutine = singleSceneUnload;
    }

    /// <summary>
    /// Unloads a list of scenes asynchronously.
    /// </summary>
    /// <param name="sceneList">A list of `SceneField` objects that represent the scenes to unload.</param>
    /// <param name="unloadSceneListCoroutine">An array of `Coroutine` objects that represent the asynchronous unload operations.</param>
    [BurstCompile]
    public virtual void UnloadSceneList(SceneField[] sceneList, out Coroutine[] unloadSceneListCoroutine)
    {
        // Create an array of `Coroutine` objects to store the asynchronous unload operations.
        Coroutine[] sceneUnloadCoroutineList = new Coroutine[sceneList.Length];
        // Iterate over the list of scenes and start an asynchronous unload operation for each scene.
        for (int i = 0; i < sceneList.Length; i++)
        {
            // Start the asynchronous unload operation and store the m_result in the temporary variable.
            Coroutine unloadCoroutine;
            UnloadSingleScene(sceneList[i], out unloadCoroutine);
            // Assign the temporary variable to the appropriate element in the `sceneUnloadCoroutineList` array.
            sceneUnloadCoroutineList[i] = unloadCoroutine;
        }
        // Return the array of `Coroutine` objects.
        unloadSceneListCoroutine = sceneUnloadCoroutineList;
    }
    #endregion
    #endregion

    #region Coroutines
    /// <summary>
    /// Basic asyncronous loading of a scene, which invokes the scene loaded event after it is complete
    /// </summary>
    /// <param name="sceneName">the name of the scene that will be added to currently loaded scenes</param>
    /// <returns>a coroutine that processes the asyncronous loading of a scene</returns>
    [BurstCompile]
    private IEnumerator LoadSceneAsyncAndWaitForCompletion(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        m_sceneLoaded.Invoke();
    }

    /// <summary>
    /// Loads a list of scenes asynchronously.
    /// </summary>
    /// <param name="sceneList">A list of `SceneField` objects that represent the scenes to load.</param>
    /// <returns>An `IEnumerator` object that represents the asynchronous load operation.</returns>
    [BurstCompile]
    private IEnumerator LoadMultipleAsyncScenes(SceneField[] sceneList)
    {
        // Load the new scene asynchronously
        foreach (SceneField scene in sceneList)
        {
            // Load the active scene asynchronously
            yield return LoadSceneAsyncAndWaitForCompletion(scene);
        }
    }

    /// <summary>
    /// Loads and activates the scene specified by the `sceneName` parameter.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load and activate.</param>
    /// <returns>An `IEnumerator` object that represents the asynchronous load operation.</returns>
    [BurstCompile]
    private IEnumerator LoadActiveScene(SceneField sceneName)
    {
        // Check if the scene that is wished to be active is loaded
        Scene currentScene = SceneManager.GetSceneByName(sceneName);
        if (!currentScene.isLoaded)
        {
            // Load the active scene asynchronously
            yield return LoadSceneAsyncAndWaitForCompletion(sceneName);
            // Set the active scene
            SceneManager.SetActiveScene(currentScene);
        }
    }

    /// <summary>
    /// Unloads the scene specified by the `scene` parameter.
    /// </summary>
    /// <param name="scene">The `SceneField` object that represents the scene to unload.</param>
    /// <returns>An `IEnumerator` object that represents the asynchronous unload operation.</returns>
    [BurstCompile]
    private IEnumerator UnloadScene(SceneField scene)
    {
        // Check if the scene is null.
        if (scene == null)
        {
            // Log a warning message.
            Debug.LogWarning("Scene is null");
            // Yield break, which will stop the coroutine.
            yield break;
        }
        // Check if the scene is loaded.
        if (SceneManager.GetSceneByName(scene.SceneName).isLoaded)
        {
            // Unload the scene asynchronously.
            SceneManager.UnloadSceneAsync(scene);
            // Wait for the scene to be unloaded.
            yield return SceneManager.UnloadSceneAsync(scene);
            // Invoke the scene unloaded event.
            m_sceneUnloaded.Invoke();
        }
        else
        {
            // Log a warning message.
            Debug.LogWarning($"Scene {scene.SceneName} is not loaded");
        }
    }

    /// <summary>
    /// Fades the screen from one alpha value to another over a specified duration.
    /// </summary>
    /// <param name="startAlpha">The starting alpha value.</param>
    /// <param name="endAlpha">The ending alpha value.</param>
    [BurstCompile]
    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0.0f;
        while (elapsedTime < m_fadeData.m_fadeDuration)
        {
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / m_fadeData.m_fadeDuration);
            SetFadeAlpha(alpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // Ensure the final alpha value is set
        SetFadeAlpha(endAlpha);
    }

    /// <summary>
    /// Fades out the screen, loads a scene, and then fades back in.
    /// </summary>
    /// <param name="sceneManagerAction">A `Coroutine` object that represents the scene manager action to perform.</param>
    /// <returns>An `IEnumerator` object that represents the asynchronous fade out and load operation.</returns>
    [BurstCompile]
    private IEnumerator FadeOutAndLoad(Coroutine sceneManagerAction)
    {
        // Set the fade canvas to active.
        m_fadeData.m_canvas.SetActive(true);
        // Set the fading flag to true.
        m_isFading = true;
        // Fade out to black.
        yield return StartCoroutine(Fade(0.0f, 1.0f));
        // Wait for the scene manager action to complete.
        yield return sceneManagerAction;
        // Fade back in.
        yield return StartCoroutine(Fade(1.0f, 0.0f));
        // Set the fading flag to false.
        m_isFading = false;
        // Set the fade canvas to inactive.
        m_fadeData.m_canvas.SetActive(false);
    }
    #endregion

    #region Internal Methods
    /// <summary>
    /// Sets the alpha value of the fade image.
    /// </summary>
    /// <param name="alpha">The alpha value to set.</param>
    [BurstCompile]
    private void SetFadeAlpha(float alpha)
    {
        if (m_fadeData.m_fadeImage != null)
        {
            Color color = m_fadeData.m_fadeImage.color;
            color.a = alpha;
            m_fadeData.m_fadeImage.color = color;
        }
    }

    /// <summary>
    /// Unloads all scenes that are not explicitly stated as persistant.
    /// </summary>
    [BurstCompile]
    private void UnloadAllScenesExceptPersistent()
    {
#if UNITY_EDITOR
        Debug.Log("Unloading all scenes except those in sceneNames list.");
#endif

        int sceneCount = SceneManager.sceneCount;
        UnloadingAllScenesExceptPersistant.Invoke();

        for (int i = 0; i < sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);

            // Check if the scene is marked as persistent in the sceneNames list
            bool isPersistent = false;
            foreach (DefinedSceneData definedScene in m_sceneNames)
            {
                if (definedScene.m_scene == scene.name && definedScene.m_isPersistant)
                {
                    isPersistent = true;
                    break;
                }
            }
            if (!isPersistent)
            {
                AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(scene);
                //since this is a total sum call, this event should occur, not an individual load

#if UNITY_EDITOR
                unloadOperation.completed += (operation) =>
                {
                    if (unloadOperation.isDone)
                    {
                        Debug.Log("Unloaded scene: " + scene.name);
                    }
                    else
                    {
                        Debug.LogError("Failed to unload scene: " + scene.name);
                    }
                };
#endif
            }

        }
    }
    #endregion
}

//TO DO LIST
/*public methods for free use
 *get list of all active scenes
 *get list of all persistant scenes
 *confirm amount instance of scene
 */
