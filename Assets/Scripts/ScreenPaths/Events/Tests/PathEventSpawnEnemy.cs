using UnityEngine;
using System.Collections;

public class PathEventSpawnEnemy : PathEvent {

	public string enemyType = "Garcia";
	public float delay = 0.0f;
	public int hitPoints = 100;

	// TODO: other spawn options, such as dificulty, initial state, etc

	protected override void OnActivate(){
		if (delay != 0.0f) {
			StartCoroutine(DelayedSpawn());
		}else {
			Spawn();
		}
	}


	IEnumerator DelayedSpawn(){
		yield return new WaitForSeconds(delay);
		Spawn();
	}


	void Spawn(){
		GameObject enemy = (GameObject)Utils.SafeInstantiate(Resources.Load(enemyType), transform.position, Quaternion.identity);
		EntityData data = enemy.GetComponent<EntityData>();
		data.energy = hitPoints;
		if (data != null) {
			data.AddOnDestroyDelegate(EventComplete);
		}
	}


	public void OnDrawGizmos() {
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(transform.position, 2);
	}

}
