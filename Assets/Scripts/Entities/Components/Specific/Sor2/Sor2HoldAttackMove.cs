using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sor2HoldAttackMove : MonoBehaviour {

	public float knockoutCoolDown = 0.5f;
	private float knockoutTimer;

	private Animator animator;
	private InputProvider inputProvider;

	private int standingAnimHash;
	private int walkAnimHash;
	private int knockOutAnimHash;
	

	void Awake () {
		knockoutTimer = 0;
		inputProvider = GetComponent<InputProvider>();
		animator = GetComponent<Animator>();

		standingAnimHash = Animator.StringToHash(animator.GetLayerName(0) + ".standing");
		walkAnimHash = Animator.StringToHash(animator.GetLayerName(0) + ".walk");
		knockOutAnimHash = Animator.StringToHash("Normal Attacks.knock_out");
	}


	void Update() {
		if (inputProvider == null) return;

		// Check knockout activation
		if (knockoutTimer > knockoutCoolDown
		    && !inputProvider.IsHoldingInputAttackA()
		    && (IsStanding() || IsWalking())
		){
			knockoutTimer = 0;
			animator.Play(knockOutAnimHash);
			//animation.Play("knock_out");
			return;
		}

		// check knockout break conditions
		if (!inputProvider.IsHoldingInputAttackA() || (!IsStanding() && !IsWalking())) {
			knockoutTimer = 0;
		}else {
			knockoutTimer += Time.deltaTime;
		}

	}



	bool IsStanding() {
		return animator.GetCurrentAnimatorStateInfo(0).fullPathHash == standingAnimHash;
	}
	
	bool IsWalking() {
		return animator.GetCurrentAnimatorStateInfo(0).fullPathHash == walkAnimHash;
	}

}
