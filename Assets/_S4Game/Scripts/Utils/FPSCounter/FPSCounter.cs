using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class FPSCounter : MonoBehaviour
{
	[SerializeField]
	float count;

	[System.Serializable]
	public class FPSUpdatedEvent : UnityEvent<float> { };

	[SerializeField]
	public FPSUpdatedEvent onFPSUpdated = new FPSUpdatedEvent();

	IEnumerator Start()
	{
		GUI.depth = 2;
		while (true)
		{
			if (Time.timeScale == 1)
			{
				yield return new WaitForSeconds(0.1f);
				count = (1 / Time.deltaTime);
				onFPSUpdated.Invoke(count);
			}
			yield return new WaitForSeconds(0.5f);
		}
	}
}