using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
	[Range(0.0f, 10f)]
	public float strtTntesity;
	[Range(0.0f, 10f)]
	public float maxIntensity;
	public int smoothing = 32;

	[SerializeField]
	List<Light> candles = new List<Light>();

	Queue<float> smoothQueue;
	float lastSum = 0;



	// Start is called before the first frame update

	// Start is called before the first frame update
	void Start()
	{
		

		   smoothQueue = new Queue<float>(smoothing);

		for (int i = 0; i < candles.Count; i++)
		{
			candles[i].intensity = strtTntesity;
			
		}
	}

    // Update is called once per frame
    void Update()
	{
		while (smoothQueue.Count >= smoothing)
		{
			lastSum -= smoothQueue.Dequeue();
		}

		for (int i = 0; i < candles.Count; i++)
		{
			float newVal = Random.Range(strtTntesity, maxIntensity);
			smoothQueue.Enqueue(newVal);
			lastSum += newVal;
			candles[i].intensity = lastSum / (float)smoothQueue.Count; ;
			//Debug.Log("candle:" + candles[i].intensity);
		}

	}

	/// <summary>
	/// Reset the randomness and start again. You usually don't need to call
	/// this, deactivating/reactivating is usually fine but if you want a strict
	/// restart you can do.
	/// </summary>
	public void Reset()
	{
		smoothQueue.Clear();
		lastSum = 0;
	}







}
