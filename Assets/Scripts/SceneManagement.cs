using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public delegate void OnAsyncSceneUpdate(float asyncProgress);
public delegate void OnAsyncSceneComplete();
public delegate bool OnRequestSceneActivation();

public class SceneManagement : MonoBehaviour
{
	public static SceneManagement sceneManagement = null;

	/// <summary>
	/// The currently active scene's build index.
	/// </summary>
	[HideInInspector] public int sceneCurrActive { get; private set; } = -1;

	private void Awake()
	{
		#region Singleton Managment
		if (!sceneManagement)
		{
			sceneManagement = this;
		}

		if (sceneManagement != this)
		{
			Destroy(this);
			return;
		}

		DontDestroyOnLoad(this);
		#endregion
	}

	/// <summary>
	/// Returns the current active scene.
	/// </summary>
	public Scene GetActiveScene() { return SceneManager.GetActiveScene(); }

	/// <summary>
	/// Returns array of all currently loaded scenes.
	/// </summary>
	public Scene[] GetAllLoadedScenes()
	{
		Scene[] tempArr = new Scene[SceneManager.sceneCount];

		for (int i = 0; i < SceneManager.sceneCount; i++)
		{
			tempArr[i] = SceneManager.GetSceneAt(i);
		}

		return tempArr;
	}

	#region Load Scene Methods

	/// <summary>
	/// Loads scene using Scene Index.
	/// </summary>
	public void LoadScene(int sceneIndex)
	{
		SceneManager.LoadScene(sceneIndex, LoadSceneMode.Single);
	}

	/// <summary>
	/// Loads scene using Scene Name.
	/// </summary>
	public void LoadScene(string sceneName)
	{
		SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
	}

	/// <summary>
	/// Loads scene using Scene Index. Declare if you want to load additively or not.
	/// </summary>
	public void LoadScene(int sceneIndex, LoadSceneMode loadAdditive)
	{
		SceneManager.LoadScene(sceneIndex, loadAdditive);
	}

	/// <summary>
	/// Loads scene using Scene Name. Declare if you want to load additively or not.
	/// </summary>
	public void LoadScene(string sceneName, LoadSceneMode loadAdditive)
	{
		SceneManager.LoadScene(sceneName, loadAdditive);
	}

	/// <summary>
	/// Loads scene using Scene Index. Declare if scene is to be loaded additively or not. Declare if the scene is to be loaded asynchronously or not.
	/// </summary>
	/// <param name="_onComplete">This function will be called when the loaded scene has reached 90% completion. (only if async)</param>
	/// <param name="_waitToActivate">If true, the scene will not be allowed to load in until you change the "allowSceneActivation" variable to true. This variable is accessible through the AsyncOperation reference returned in the _onComplete function you passed though.
	/// <para>IMPORTANT: If you use this feature, you must set "allowSceneActivation" to true otherwise, the scene will never finish loading.</para></param>
	public void LoadScene(int sceneIndex, LoadSceneMode loadAdditive, bool loadAsync, OnAsyncSceneUpdate onProgressUpdate = null, OnAsyncSceneComplete onComplete = null, OnRequestSceneActivation onActivationRequest = null, bool waitToActivate = false)
	{
		if (loadAsync)
		{
			StartCoroutine(LoadOrUnloadAsync(true, sceneIndex, onActivationRequest, onComplete, onProgressUpdate, waitToActivate, loadAdditive));
		}
		else
		{
			SceneManager.LoadScene(sceneIndex, loadAdditive);
		}
	}

	/// <summary>
	/// Loads scene using Scene Name. Declare if scene is to be loaded additively or not. Declare if the scene is to be loaded asynchronously or not.
	/// </summary>
	/// <param name="_onComplete">This function will be called when the loaded scene has reached 90% completion. (only if async)</param>
	/// <param name="_waitToActivate">If true, the scene will not be allowed to load in until you change the "allowSceneActivation" variable to true. This variable is accessible through the AsyncOperation reference returned in the _onComplete function you passed though.
	/// <para>IMPORTANT: If you use this feature, you must set "allowSceneActivation" to true otherwise, the scene will never finish loading.</para></param>
	public void LoadScene(string sceneName, LoadSceneMode loadAdditive, bool loadAsync, OnAsyncSceneUpdate onProgressUpdate = null, OnAsyncSceneComplete onComplete = null, OnRequestSceneActivation onActivationRequest = null, bool waitToActivate = false)
	{
		if (loadAsync)
		{
			int sceneIndex = GetSceneIndexByName(sceneName);
			StartCoroutine(LoadOrUnloadAsync(true, sceneIndex, onActivationRequest, onComplete, onProgressUpdate, waitToActivate, loadAdditive));
		}
		else
		{
			SceneManager.LoadScene(sceneName, loadAdditive);
		}
	}

	#endregion

	#region Unload Scene Methods

	/// <summary>
	/// Unloads scene asynchronously using Scene Index. You can only unload a scene if isn't the "active" scene. Scenes loaded additively can be unloaded.
	/// </summary>
	/// <param name="_onComplete">This function will be called when the loaded scene has reached 90% completion.</param>
	/// <param name="_waitToActivate">If true, the scene will not be allowed to load in until you change the "allowSceneActivation" variable to true. This variable is accessible through the AsyncOperation reference returned in the _onComplete function you passed though.
	/// <para>IMPORTANT: If you use this feature, you must set "allowSceneActivation" to true otherwise, the scene will never finish loading.</para></param>
	public void UnloadScene(int sceneIndex, OnAsyncSceneUpdate onProgressUpdate = null, OnAsyncSceneComplete onComplete = null, OnRequestSceneActivation onActivationRequest = null, bool waitToActivate = false)
	{
		StartCoroutine(LoadOrUnloadAsync(false, sceneIndex, onActivationRequest, onComplete, onProgressUpdate, waitToActivate));
	}

	/// <summary>
	/// Unloads scene asynchronously using Scene Name. You can only unload a scene if isn't the "active" scene. Scenes loaded additively can be unloaded.
	/// </summary>
	/// <param name="_onComplete">This function will be called when the loaded scene has reached 90% completion.</param>
	/// <param name="_waitToActivate">If true, the scene will not be allowed to load in until you change the "allowSceneActivation" variable to true. This variable is accessible through the AsyncOperation reference returned in the _onComplete function you passed though.
	/// <para>IMPORTANT: If you use this feature, you must set "allowSceneActivation" to true otherwise, the scene will never finish loading.</para></param>
	public void UnloadScene(string sceneName, OnAsyncSceneUpdate onProgressUpdate = null, OnAsyncSceneComplete onComplete = null, OnRequestSceneActivation onActivationRequest = null, bool waitToActivate = false)
	{
		int sceneIndex = GetSceneIndexByName(sceneName);
		StartCoroutine(LoadOrUnloadAsync(false, sceneIndex, onActivationRequest, onComplete, onProgressUpdate, waitToActivate));
	}

	#endregion

	#region Async Operations
	private IEnumerator LoadOrUnloadAsync(bool shouldLoad, int sceneIndex, OnRequestSceneActivation onActivationRequest,
		                                  OnAsyncSceneComplete onComplete, OnAsyncSceneUpdate onProgressUpdate,
		                                  bool waitToActivate = false, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
	{
		if (onActivationRequest == null && waitToActivate)
		{
			Debug.LogError("WARNING, attempting to load a scene using waitToActivate, but there was no onComplete function given. The scene will never finish loading!");
			yield break; // If scene will never load, cancel operation.
		}

		AsyncOperation async = PerformAsyncSceneLoadOrUnload(shouldLoad, sceneIndex, loadSceneMode);

		if (async == null)
		{
			yield break; // If method fails to call, cancel operation.
		}

		if (waitToActivate)
		{
			async.allowSceneActivation = false;
		}

		while (async.progress <= 0.9f)
		{
			onProgressUpdate?.Invoke(async.progress);
		}

		while (async.allowSceneActivation == false) // If the IEnumerator closes the async variable reference will be lost. If it's lost, null references will happen when you try to change "allowSceneActivation" when using the _waitToActivate functionality
		{
			async.allowSceneActivation = onActivationRequest();
			yield return null;
		}

		onComplete?.Invoke();
	}

	private int GetSceneIndexByName(string name)
	{
		int sceneIndex = -1;
		int numberOfScenes = SceneManager.sceneCount;

		for (int iter = 0; iter < numberOfScenes; iter++)
		{
			if (SceneManager.GetSceneAt(iter).name == name)
            {
				sceneIndex = iter;
				break;
            }
		}

		return sceneIndex;
	}

	private AsyncOperation PerformAsyncSceneLoadOrUnload(bool shouldLoad, int sceneIndex, LoadSceneMode loadSceneMode)
    {
		if(shouldLoad)
        {
			return SceneManager.LoadSceneAsync(sceneIndex, loadSceneMode);
        }
		else
        {
			return SceneManager.UnloadSceneAsync(sceneIndex);
        }
    }
    #endregion
}