using UnityEngine;
using System.Collections;

public class VeryDumbAiInputProvider: InputProvider {

	public int directionalRandomRange = 30;
	public float minDirectionalRandomTime = 0.5f;
	public float maxDirectionalRandomTime = 2;

	public int attackARandomRange = 20;
	public int attackBRandomRange = 120;
	public int attackCRandomRange = 120;
	public int jumpRandomRange = 30;
	public int specialRandomRange = 60;
	public int extraRandomRange = 500;
	public int doubleLeftRandomRange = 15;
	public int doubleRightRandomRange = 15;
	public int doubleUptRandomRange = 100;
	public int doubleDowntRandomRange = 100;

	private float directionCooldown;

	private Vector3 lastVelocity = Vector3.zero;

	void Update() {
		if (directionCooldown > 0) {
			directionCooldown -= Time.deltaTime;
			if (directionCooldown < 0) {
				lastVelocity = Vector3.zero;
			}
		}
	}


	public override Vector3 GetInputMovement(){
		if (directionCooldown <= 0) {
			if (Random.Range(0,directionalRandomRange) == 0) {
				directionCooldown = Random.Range(minDirectionalRandomTime, maxDirectionalRandomTime);
				lastVelocity = new Vector3(Random.Range(-1,2), 0, Random.Range(-1,2));
			}
		}
		return lastVelocity;
	}
	
	public override bool HasInputAttackA(){
		return Random.Range(0,attackARandomRange) == 0;
	}

	public override bool HasInputAttackB(){
		return Random.Range(0,attackBRandomRange) == 0;
	}

	public override bool HasInputAttackC(){
		return Random.Range(0,attackCRandomRange) == 0;
	}
	
	public override bool HasInputJump(){
		return Random.Range(0,jumpRandomRange) == 0;
	}
	
	public override bool HasInputSpecial(){
		return Random.Range(0,specialRandomRange) == 0;
	}

	public override bool HasInputExtra(){
		return Random.Range(0,extraRandomRange) == 0;
	}
	
	public override bool HasInputDoubleLeft(){
		return Random.Range(0,doubleLeftRandomRange) == 0;
	}

	public override bool HasInputDoubleRight(){
		return Random.Range(0,doubleRightRandomRange) == 0;
	}

	public override bool HasInputDoubleUp(){
		return Random.Range(0,doubleUptRandomRange) == 0;
	}

	public override bool HasInputDoubleDown(){
		return Random.Range(0,doubleDowntRandomRange) == 0;
	}

}
