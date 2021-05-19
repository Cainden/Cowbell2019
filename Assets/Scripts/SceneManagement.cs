using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
	public Scene GetCurrScene() { return SceneManager.GetActiveScene(); }

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
	public void LoadScene(int _sceneIndex)
	{
		SceneManager.LoadScene(_sceneIndex, LoadSceneMode.Single);
	}

	/// <summary>
	/// Loads scene using Scene Name.
	/// </summary>
	public void LoadScene(string _sceneName)
	{
		SceneManager.LoadScene(_sceneName, LoadSceneMode.Single);
	}

	/// <summary>
	/// Loads scene using Scene Index. Declare if you want to load additively or not.
	/// </summary>
	public void LoadScene(int _sceneIndex, LoadSceneMode _loadAdditive)
	{
		SceneManager.LoadScene(_sceneIndex, _loadAdditive);
	}

	/// <summary>
	/// Loads scene using Scene Name. Declare if you want to load additively or not.
	/// </summary>
	public void LoadScene(string _sceneName, LoadSceneMode _loadAdditive)
	{
		SceneManager.LoadScene(_sceneName, _loadAdditive);
	}

	/// <summary>
	/// Loads scene using Scene Index. Declare if scene is to be loaded additively or not. Declare if the scene is to be loaded asynchronously or not.
	/// </summary>
	/// <param name="_onComplete">This function will be called when the loaded scene has reached 90% completion. (only if async)</param>
	/// <param name="_waitToActivate">If true, the scene will not be allowed to load in until you change the "allowSceneActivation" variable to true. This variable is accessible through the AsyncOperation reference returned in the _onComplete function you passed though.
	/// <para>IMPORTANT: If you use this feature, you must set "allowSceneActivation" to true otherwise, the scene will never finish loading.</para></param>
	public void LoadScene(int _sceneIndex, LoadSceneMode _loadAdditive, bool _loadAsync, System.Action<AsyncOperation> _onComplete = null, bool _waitToActivate = false)
	{
		if (_loadAsync)
		{
			StartCoroutine(LoadAsync(_sceneIndex, _loadAdditive, _onComplete, _waitToActivate));
		}
		else
		{
			SceneManager.LoadScene(_sceneIndex, _loadAdditive);
		}
	}

	/// <summary>
	/// Loads scene using Scene Name. Declare if scene is to be loaded additively or not. Declare if the scene is to be loaded asynchronously or not.
	/// </summary>
	/// <param name="_onComplete">This function will be called when the loaded scene has reached 90% completion. (only if async)</param>
	/// <param name="_waitToActivate">If true, the scene will not be allowed to load in until you change the "allowSceneActivation" variable to true. This variable is accessible through the AsyncOperation reference returned in the _onComplete function you passed though.
	/// <para>IMPORTANT: If you use this feature, you must set "allowSceneActivation" to true otherwise, the scene will never finish loading.</para></param>
	public void LoadScene(string _sceneName, LoadSceneMode _loadAdditive, bool _loadAsync, System.Action<AsyncOperation> _onComplete = null, bool _waitToActivate = false)
	{
		if (_loadAsync)
		{
			StartCoroutine(LoadAsync(_sceneName, _loadAdditive, _onComplete, _waitToActivate));
		}
		else
		{
			SceneManager.LoadScene(_sceneName, _loadAdditive);
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
	public void UnloadScene(int _sceneIndex, System.Action<AsyncOperation> _onComplete = null, bool _waitToActivate = false)
	{
		StartCoroutine(UnloadAsync(_sceneIndex, _onComplete, _waitToActivate));
	}

	/// <summary>
	/// Unloads scene asynchronously using Scene Name. You can only unload a scene if isn't the "active" scene. Scenes loaded additively can be unloaded.
	/// </summary>
	/// <param name="_onComplete">This function will be called when the loaded scene has reached 90% completion.</param>
	/// <param name="_waitToActivate">If true, the scene will not be allowed to load in until you change the "allowSceneActivation" variable to true. This variable is accessible through the AsyncOperation reference returned in the _onComplete function you passed though.
	/// <para>IMPORTANT: If you use this feature, you must set "allowSceneActivation" to true otherwise, the scene will never finish loading.</para></param>
	public void UnloadScene(string _sceneName, System.Action<AsyncOperation> _onComplete = null, bool _waitToActivate = false)
	{
		StartCoroutine(UnloadAsync(_sceneName, _onComplete, _waitToActivate));
	}

	#endregion

	#region Async Operations

	/// <summary>
	/// Using Scene Index.
	/// </summary>
	private IEnumerator LoadAsync(int _sceneIndex, LoadSceneMode _loadAdditive, System.Action<AsyncOperation> _onComplete, bool _waitToActivate)
	{
		if(_onComplete == null && _waitToActivate)
		{
			Debug.LogError("WARNING, attempting to load a scene using waitToActivate, but there was no onComplete function given. The scene will never finish loading!");
			yield break; // If scene will never load, cancel operation.
		}

		AsyncOperation async;
		async = SceneManager.LoadSceneAsync(_sceneIndex, _loadAdditive);
		if (async == null)
		{
			yield break; // If method fails to call, cancel operation.
		} 

		float asyncProgress; // Not being used yet. For sending into methods later if we need.

		if (_waitToActivate)
		{
			async.allowSceneActivation = false;
		}

		while (async.progress < 0.9f)
		{
			asyncProgress = async.progress;
			yield return null;
		}

		if (async.progress == 0.9f)
		{
			asyncProgress = 1;
			_onComplete?.Invoke(async);
		}

		while (async.allowSceneActivation == false) // If the IEnumerator closes the async variable reference will be lost. If it's lost, null references will happen when you try to change "allowSceneActivation" when using the _waitToActivate functionality
		{
			yield return null;
		}
	}

	/// <summary>
	/// Using Scene Name.
	/// </summary>
	private IEnumerator LoadAsync(string _sceneName, LoadSceneMode _loadAdditive, System.Action<AsyncOperation> _onComplete, bool _waitToActivate)
	{
		if (_onComplete == null && _waitToActivate)
		{
			Debug.LogError("WARNING, attempting to load a scene using waitToActivate, but there was no onComplete function given. The scene will never finish loading!");
			yield break; // If scene will never load, cancel operation.
		}

		AsyncOperation async;
		async = SceneManager.LoadSceneAsync(_sceneName, _loadAdditive);
		if (async == null)
		{
			yield break; // If method fails to call, cancel operation.
		} 

		float asyncProgress; // Not being used yet. For sending into methods later if we need.

		if (_waitToActivate)
		{
			async.allowSceneActivation = false;
		}

		while (async.progress < 0.9f)
		{
			asyncProgress = async.progress;
			yield return null;
		}

		if (async.progress == 0.9f)
		{
			asyncProgress = 1;
			_onComplete?.Invoke(async);
		}

		while (async.allowSceneActivation == false) // If the IEnumerator closes the async variable reference will be lost. If it's lost, null references will happen when you try to change "allowSceneActivation" when using the _waitToActivate functionality
		{
			yield return null;
		}
	}

	/// <summary>
	/// Using Scene Index.
	/// </summary>
	private IEnumerator UnloadAsync(int _sceneIndex, System.Action<AsyncOperation> _onComplete, bool _waitToActivate)
	{
		if (_onComplete == null && _waitToActivate)
		{
			Debug.LogError("WARNING, attempting to load a scene using waitToActivate, but there was no onComplete function given. The scene will never finish loading!");
			yield break; // If scene will never load, cancel operation.
		}

		AsyncOperation async;
		async = SceneManager.UnloadSceneAsync(_sceneIndex);
		if (async == null)
		{
			yield break; // If method fails to call, cancel operation.
		}

		float asyncProgress; // Not being used yet. For sending into functions later if we need.

		if (_waitToActivate)
		{
			async.allowSceneActivation = false;
		}

		while (async.progress < 0.9f)
		{
			asyncProgress = async.progress;
			yield return null;
		}

		if (async.progress == 0.9f)
		{
			asyncProgress = 1;
			_onComplete?.Invoke(async);
		}

		while (async.allowSceneActivation == false) // If the IEnumerator closes the async variable reference will be lost. If it's lost, null references will happen when you try to change "allowSceneActivation" when using the _waitToActivate functionality
		{
			yield return null;
		}
	}

	/// <summary>
	/// Using Scene Name.
	/// </summary>
	private IEnumerator UnloadAsync(string _sceneName, System.Action<AsyncOperation> _onComplete, bool _waitToActivate)
	{
		if (_onComplete == null && _waitToActivate)
		{
			Debug.LogError("WARNING, attempting to load a scene using waitToActivate, but there was no onComplete function given. The scene will never finish loading!");
			yield break; // If scene will never load, cancel operation.
		}

		AsyncOperation async;
		async = SceneManager.UnloadSceneAsync(_sceneName);
		if (async == null)
		{
			yield break; // If method fails to call, cancel operation.
		}

		float asyncProgress; // Not being used yet. For sending into functions later if we need.

		if (_waitToActivate)
		{
			async.allowSceneActivation = false;
		}

		while (async.progress < 0.9f)
		{
			asyncProgress = async.progress;
			yield return null;
		}

		if (async.progress == 0.9f)
		{
			asyncProgress = 1;
			_onComplete?.Invoke(async);
		}

		while (async.allowSceneActivation == false) // If the IEnumerator closes the async variable reference will be lost. If it's lost, null references will happen when you try to change "allowSceneActivation" when using the _waitToActivate functionality
		{
			yield return null;
		}
	}

	#endregion
}