using System.Collections;
using System.IO;
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
	/// Loads the scene using the scene index.
	/// </summary>
	/// <param name="sceneIndex">Index of the scene in build settings.</param>
	/// <param name="loadAdditive">Mode to load the scene.</param>
	/// <param name="loadAsync">True if the scene should be loaded asynchronously.</param>
	/// <param name="onProgressUpdate">Callback that will provide the calling object
	/// with updates on the progress percentage via a float value (0.0f - 1.0f).</param>
	/// <param name="onComplete">Callback that will notify the calling object when
	/// the process has completed.</param>
	/// <param name="onActivationRequest">Callback that will allow the calling object
	/// to pause completion of the process, until it returns true.</param>
	/// <param name="waitToActivate">If true, allowSceneActivation will be set to
	/// false on the AsyncOperation. This will pause the completion of the process
	/// until this value is set to true again.</param>
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
	/// Loads scene using the scene name.
	/// </summary>
	/// <param name="sceneName">Name of the scene. Can be the scene name without extension,
	/// or the relative path to the scene with extension.</param>
	/// <param name="loadAdditive">Mode to load the scene.</param>
	/// <param name="loadAsync">True if the scene should be loaded asynchronously.</param>
	/// <param name="onProgressUpdate">Callback that will provide the calling object
	/// with updates on the progress percentage via a float value (0.0f - 1.0f).</param>
	/// <param name="onComplete">Callback that will notify the calling object when
	/// the process has completed.</param>
	/// <param name="onActivationRequest">Callback that will allow the calling object
	/// to pause completion of the process, until it returns true.</param>
	/// <param name="waitToActivate">If true, allowSceneActivation will be set to
	/// false on the AsyncOperation. This will pause the completion of the process
	/// until this value is set to true again.</param>
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
	/// Unloads scene asynchronously using the scene index.
	/// </summary>
	/// <param name="sceneIndex">Index of the scene in build settings.</param>
	/// <param name="onProgressUpdate">Callback that will provide the calling object
	/// with updates on the progress percentage via a float value (0.0f - 1.0f).</param>
	/// <param name="onComplete">Callback that will notify the calling object when
	/// the process has completed.</param>
	/// <param name="onActivationRequest">Callback that will allow the calling object
	/// to pause completion of the process, until it returns true.</param>
	/// <param name="waitToActivate">If true, allowSceneActivation will be set to
	/// false on the AsyncOperation. This will pause the completion of the process
	/// until this value is set to true again.</param>
	/// <remarks>The currently active scene cannot be unloaded.</remarks>
	public void UnloadScene(int sceneIndex, OnAsyncSceneUpdate onProgressUpdate = null, OnAsyncSceneComplete onComplete = null, OnRequestSceneActivation onActivationRequest = null, bool waitToActivate = false)
	{
		StartCoroutine(LoadOrUnloadAsync(false, sceneIndex, onActivationRequest, onComplete, onProgressUpdate, waitToActivate));
	}

	/// <summary>
	/// Unloads scene asynchronously using the scene name.
	/// </summary>
	/// <param name="sceneName">Name of the scene. Can be the scene name without extension,
	/// or the relative path to the scene with extension.</param>
	/// <param name="onProgressUpdate">Callback that will provide the calling object
	/// with updates on the progress percentage via a float value (0.0f - 1.0f).</param>
	/// <param name="onComplete">Callback that will notify the calling object when
	/// the process has completed.</param>
	/// <param name="onActivationRequest">Callback that will allow the calling object
	/// to pause completion of the process, until it returns true.</param>
	/// <param name="waitToActivate">If true, allowSceneActivation will be set to
	/// false on the AsyncOperation. This will pause the completion of the process
	/// until this value is set to true again.</param>
    /// <remarks>The currently active scene cannot be unloaded.</remarks>
	public void UnloadScene(string sceneName, OnAsyncSceneUpdate onProgressUpdate = null, OnAsyncSceneComplete onComplete = null, OnRequestSceneActivation onActivationRequest = null, bool waitToActivate = false)
	{
		int sceneIndex = GetSceneIndexByName(sceneName);
		StartCoroutine(LoadOrUnloadAsync(false, sceneIndex, onActivationRequest, onComplete, onProgressUpdate, waitToActivate));
	}

	#endregion

	#region Async Operations
	/// <summary>
	/// Coroutine that manages the async loading and unloading
	/// of scenes.
	/// </summary>
	/// <param name="shouldLoad">True if the scene is to be loaded.
	/// False if the scene is to be unloaded.</param>
	/// <param name="sceneIndex">Build index of the scene to be loaded/unloaded.</param>
	/// <param name="onActivationRequest">Callback that will allow the calling object
    /// to pause completion of the process, until it returns true.</param>
	/// <param name="onComplete">Callback that will notify the calling object when
    /// the process has completed.</param>
	/// <param name="onProgressUpdate">Callback that will provide the calling object
    /// with updates on the progress percentage via a float value (0.0f - 1.0f).</param>
	/// <param name="waitToActivate">If true, allowSceneActivation will be set to
    /// false on the AsyncOperation. This will pause the completion of the process
    /// until this value is set to true again.</param>
	/// <param name="loadSceneMode">Mode to load the scene, if this is a load operation.
	/// If the scene is being unloaded, this parameter is ignored.</param>
	/// <returns></returns>
	private IEnumerator LoadOrUnloadAsync(bool shouldLoad, int sceneIndex, OnRequestSceneActivation onActivationRequest,
		                                  OnAsyncSceneComplete onComplete, OnAsyncSceneUpdate onProgressUpdate,
		                                  bool waitToActivate = false, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
	{
		if (sceneIndex == -1)
        {
			Debug.Log("Invalid scene.");
			yield break;
        }

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

	/// <summary>
    /// Finds the scene index for a given scene. The function will find the
    /// scene by name, or full path.
    /// </summary>
    /// <param name="name">Name of the scene. Can be the scene name without extension,
    /// or the relative path to the scene with extension.</param>
    /// <returns>Index of the scene. -1 if the scene was not found.</returns>
	private int GetSceneIndexByName(string name)
	{
		int sceneIndex = -1;
		int numberOfScenes = SceneManager.sceneCountInBuildSettings;

		for (int iter = 0; iter < numberOfScenes; iter++)
		{
			string scenePathNameByIndex = SceneUtility.GetScenePathByBuildIndex(iter);
			string sceneNameByIndex = Path.GetFileNameWithoutExtension(scenePathNameByIndex);
			if (sceneNameByIndex == name || scenePathNameByIndex == name)
            {
				sceneIndex = iter;
				break;
            }
		}

		return sceneIndex;
	}

	/// <summary>
    /// Initiates an async scene load/unload operation.
    /// </summary>
    /// <param name="shouldLoad">True if the scene is to be loaded.
    /// False if the scene is to be unloaded.</param>
    /// <param name="sceneIndex">Build index of the scene to be loaded/unloaded.</param>
    /// <param name="loadSceneMode">Mode to load the scene, if this is a load operation.
    /// If the scene is being unloaded, this parameter is ignored.</param>
    /// <returns>AsyncOperation object to manage the load/unload process.</returns>
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