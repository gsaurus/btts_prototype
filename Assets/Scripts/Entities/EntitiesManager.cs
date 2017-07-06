using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EntitiesManager : MonoBehaviour {
	// Keep track of active game entities

	public float[] teamsFriendlyFire;

	private List<EntityData>[] teams;
	// Array of teams, each team a list of EntityData

	// Singleton
	private static EntitiesManager __instance;
	public static EntitiesManager Instance() {
		return __instance;
	}

	void Awake() {

		// initialize singleton
		__instance = this;

		teams = new List<EntityData>[teamsFriendlyFire.Length];
		for (int i = 0 ; i < teams.Length ; ++i) {
			teams[i] = new List<EntityData>();
		}

		EntityData.SetDelegates(TeamChanged, EntityDestroied);
	}


	
	// Called by EntityData team setter
	public void TeamChanged(EntityData entity, int oldTeam){
		teams[oldTeam].Remove(entity);
		teams[entity.team].Add(entity);
		Debug.Log("entity on team: " + entity.team + ", kind: " + entity.name);
	}
	
	// Called by EntityData OnDestroy
	void EntityDestroied(EntityData entity){
		teams[entity.team].Remove(entity);
	}

	
	public int CountEntities(params int[] teamIds){
		// Number of active entities in the given team ids
		int total = 0;

		for (int i = 0 ; i < teamIds.Length ; ++i){
			total += teams[teamIds[i]].Count;
		}

		return total;
	}

	public List<GameObject> GetEntityObjects(params int[] teamIds){
		// return all active entities in the given team ids
		List<GameObject> result = new List<GameObject>();

		for (int i = 0 ; i < teamIds.Length ; ++i){
			foreach (EntityData entity in teams[teamIds[i]]){
				result.Add(entity.gameObject);
			}
		}
		
		return result;
	}


}
