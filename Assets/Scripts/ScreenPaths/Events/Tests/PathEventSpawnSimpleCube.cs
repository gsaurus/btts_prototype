using UnityEngine;
using System.Collections;

public class PathEventSpawnSimpleCube : PathEvent {

	private GameObject cube;

	protected override void OnActivate(){
		cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		Utils.AddChild(transform, cube);
		cube.transform.localScale = new Vector3(5,5,5);
		cube.GetComponent<Renderer>().material.color = new Color(1,0,0);
		StartCoroutine(Wait());
	}


	IEnumerator Wait(){
		yield return new WaitForSeconds(Random.Range(3f, 6.0f));
		Destroy(cube);
		EventComplete();
	}
	


}
