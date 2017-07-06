using UnityEngine;
using System.Collections;

public class PathEventDisplayObject : PathEvent {

	public GameObject obj;
	public float time = 4.0f;
	
	// TODO: other spawn options, such as dificulty, initial state, etc
	
	protected override void OnActivate(){
		obj.SetActive(true);
		StartCoroutine(DelayedCompletion());
	}
	
	
	IEnumerator DelayedCompletion(){
		yield return new WaitForSeconds(time);
		obj.SetActive(false);
		EventComplete();
	}


}
