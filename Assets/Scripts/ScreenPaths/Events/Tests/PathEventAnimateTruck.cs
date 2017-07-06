using UnityEngine;
using System.Collections;

public class PathEventAnimateTruck : PathEvent {

	public GameObject truck;

	protected override void OnActivate(){
		if (truck != null) {
			Animator animator = truck.GetComponent<Animator>();
			if (animator != null) animator.Play("Car_Moving");
		}
		EventComplete();
	}

}
