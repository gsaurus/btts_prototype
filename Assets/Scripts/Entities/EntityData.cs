using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EntityData : MonoBehaviour {

	public delegate void OnTeamChanged(EntityData entity, int oldTeam);
	public delegate void OnEntityDestroyed(EntityData entity);

	public delegate void OnDestroyed();

	private static OnTeamChanged onTeamChangedDelegate;
	private static OnEntityDestroyed onEntityDestroyedDelegate;

	private OnDestroyed onDestroyPersonalDelegate;

	[HideInInspector] [SerializeField] int _team;
	[HideInInspector] [SerializeField] float _energy;

	[ExposeProperty]
	public int team {
		get{ return _team; }
		set{
			// Automatically update team on EntitiesManager
			if (_team == value) return;
			int oldValue = _team;
			_team = value;
			if (onTeamChangedDelegate != null){
				onTeamChangedDelegate(this, oldValue);
			}
		}
	}

	[ExposeProperty]
	public float energy {
		get{ return _energy; }
		set{
			// Whenever energy changes, update parameter on animator
			_energy = value;
			if (animator != null){
				animator.SetFloat("energy", _energy);
				if (_energy <= 0) {
					animator.SetTrigger("die");
				}
			}
		}
	}

	private Animator animator;
	// Reference to the animator
	


	public static void SetDelegates(OnTeamChanged onTeamChanged, OnEntityDestroyed onEntityDestroyed){
		onTeamChangedDelegate = onTeamChanged;
		onEntityDestroyedDelegate = onEntityDestroyed;
	}


	void Start() {
		animator = GetComponent<Animator>();

		// setup initial team value
		if (onTeamChangedDelegate != null){
			onTeamChangedDelegate(this, 0);
		}
	}


	void OnDestroy(){
		if (onEntityDestroyedDelegate != null) {
			onEntityDestroyedDelegate(this);
		}
		if (onDestroyPersonalDelegate != null) {
			onDestroyPersonalDelegate();
		}
	}



	public void Destroy(){
		Destroy(gameObject);
	}

	public void AddOnDestroyDelegate(OnDestroyed del){
		onDestroyPersonalDelegate += del;
	}

}
