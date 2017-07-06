using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GarciaAiInputProvider: InputProvider {

	public float velocityReference = 10.0f;
	public float maxVelocity	   = 20.0f;
	public Vector3 maxDistanceToAttack = new Vector3(6.0f, 0.0f, 2.0f);
	public int maxComboBeforeStandBack = 3;
	public float chaseMoveProbability = 0.3f;
	public float attackProbability = 0.3f;

	private BasicPhysicsComponent physics;
	private Animator animator;
	private ComboCounter comboCounter;
	private int standingAnimHash;
	private int walkAnimHash;

	private GameObject target;
	private Vector3 targetPosition;

	private bool attack = false;
	private float velocity = 0;
	private Vector3 inputVelocity = Vector3.zero;
	private Vector3 targetPoint;

	private bool chasing = false;

	private float targetReachabilityTime = 0.0f;
	private float idleTime = 0.0f;



	void Awake() {
		GameObject parent = Utils.FindParentWithComponent<Animator>(gameObject);
		physics = parent.GetComponent<BasicPhysicsComponent>();
		animator = parent.GetComponent<Animator>();
		comboCounter = parent.GetComponent<ComboCounter>();
		standingAnimHash = Animator.StringToHash(animator.GetLayerName(0) + ".standing");
		walkAnimHash = Animator.StringToHash(animator.GetLayerName(0) + ".walk");
	}





	void ChooseTarget() {
		// For demo purpose, choose a target randomly
		target = null;
		List<GameObject> targets = EntitiesManager.Instance().GetEntityObjects(0);
		if (targets != null && targets.Count > 0) {
			target = EntitiesManager.Instance().GetEntityObjects(0)[targets.Count - 1].gameObject;
		}
		if (target != null) targetPosition = target.transform.position;
	}


	float GetPassiveVelocity() {
		return Random.Range(0.4f, 0.7f) * maxVelocity;
	}

	float GetActiveVelocity() {
		return Random.Range(0.6f, 1.0f) * maxVelocity;
	}

	void SetIdle(float time = -1.0f) {
		chasing = false;
		targetReachabilityTime = 0.0f;
		velocity = 0.0f;
		if (time == -1.0f) idleTime = Random.Range(0.25f, 1.2f);
		else idleTime = time;
	}

	void AttackMove() {
		ChooseTarget();
		Hittable targetHittable = target == null ? null : target.GetComponentInChildren<Hittable>();
		if (targetHittable == null || !targetHittable.enabled) {
			// Don't attack who can't be hitten, stand back
			StandBack(false);
			return;
		}
		idleTime = 0.0f;
		chasing = true;
		float targetX = (transform.position.x < targetPosition.x ? -0.5f : 0.5f)*maxDistanceToAttack.x;
		targetPoint = new Vector3(targetX, 0, Random.Range(-maxDistanceToAttack.z, maxDistanceToAttack.z));
		targetPoint += targetPosition;
		velocity = GetActiveVelocity();
		targetReachabilityTime = Vector3.Distance(transform.position, targetPoint) / velocity;
	}


	void updateChasing() {
		if (target != null) targetPosition = target.transform.position;
		float targetX = (transform.position.x < targetPosition.x ? -0.5f : 0.5f)*maxDistanceToAttack.x;
		targetPoint = new Vector3(targetX, 0, Random.Range(-maxDistanceToAttack.z, maxDistanceToAttack.z));
		targetPoint += targetPosition;
	}



	void StandBack(bool forceBack) {
		ChooseTarget();
		idleTime = 0.0f;
		float targetX, targetZ;
		if (forceBack) {
			targetX = transform.position.x < targetPosition.x ? -1 : 1;
			targetPoint = new Vector3(targetX, 0, Random.Range(-1,1+1));
		}else {
			do {
				if (Mathf.Abs(transform.position.z - targetPosition.z) > maxDistanceToAttack.z){
					targetX = transform.position.x < targetPosition.x ? 1 : -1;
				}else {
					targetX = -1 + 2*Random.Range(0,1+1);
				}
				targetZ = transform.position.z < targetPosition.z ? -1 : 1;
				targetPoint = new Vector3(targetX, 0, targetZ);
			} while(targetPoint == Vector3.zero);
		}
		targetX = forceBack ? Random.Range(16f, 24f) : Random.Range(8.0f, 20f);
		targetZ = forceBack ? Random.Range(3.0f, 10f) : Random.Range(8.0f, 12f);
		targetPoint = new Vector3(targetPoint.x * targetX, 0, targetPoint.z * targetZ);
		targetPoint += targetPosition;
		velocity = GetPassiveVelocity();
		targetReachabilityTime = Vector3.Distance(transform.position, targetPoint) / velocity;
		//Debug.Log("Stand back: TargetPoint: " + (targetPoint - targetPosition) + ", time to reach: " + targetReachabilityTime);
	}


	void CheckAttackAction(){
		attack = false;

		// if moving away, do not attack
		if (IsWalkingBackward()) return;

		List<GameObject> targets = EntitiesManager.Instance().GetEntityObjects(0);

		foreach (GameObject obj in targets) {

			if (Mathf.Abs(transform.position.x - obj.transform.position.x) < maxDistanceToAttack.x
			    && Mathf.Abs(transform.position.z - obj.transform.position.z) < maxDistanceToAttack.z
			    )
			{
				Hittable targetHittable = obj.GetComponentInChildren<Hittable>();
				if (comboCounter.value >= maxComboBeforeStandBack || targetHittable == null || !targetHittable.enabled){
					// stand back imediately!
					StandBack(true);
				}else {
					ExtraColliders targetColliders = obj.GetComponent<ExtraColliders>();
					attack = (targetColliders == null || targetColliders.IsGrounded()) ? Random.value < attackProbability : true;
					SetIdle(0.1f);
				}
				break;
			}
		}
	}


	void ChooseNextAction(bool includeIdle){
		if (attack) return;

		chasing = false;

		if (Random.value < chaseMoveProbability) {
			AttackMove();
		}else{
			if (includeIdle && Random.value < 0.5f) {
				SetIdle();
				return;
			}
			StandBack(Random.value < 0.35f);
		}
	}


	void Update() {

		if (chasing) {
			updateChasing();
		}

		if (targetReachabilityTime > 0) targetReachabilityTime -= Time.deltaTime;
		if (velocity > 0.0f && (targetReachabilityTime <= 0 || Vector3.Distance(transform.position, targetPoint) <= velocity * Time.deltaTime + 0.1f) ) {
			ChooseNextAction(true);
		}else {
			if (idleTime > 0) idleTime -= Time.deltaTime;
			if (idleTime <= 0 && velocity == 0) {
				ChooseNextAction(false);
			}
		}

		// check proximity with player, and attack if close enough
		CheckAttackAction();


		// update orientation and walking animation speed
		if (target != null) targetPosition = target.transform.position;
		if (animator != null) {
			float animSpeed = 1.0f;
			if (IsStanding() || IsWalking()) {
				// force flip if not facing correctly
				if (physics.isFacingRight != transform.position.x <= targetPosition.x) {
					physics.Flip();
				}
				if (IsWalking() && velocity != 0.0f){
					animSpeed = velocity / velocityReference;
					if (targetPoint.x - transform.position.x > 0 != physics.isFacingRight){
						animSpeed *= -1; // moving backwards
					}
				}
			}
			//animator.speed = animSpeed; // no longer works on latest version, so the way to fix it is as follows:
			animator.SetFloat("speed", animSpeed);
			// manual loop if anim speed is reversed.. due to Unity not doing it itself...
			AnimatorStateInfo currAnimState = animator.GetCurrentAnimatorStateInfo(0);
			if (animator.speed < 0 && currAnimState.loop && currAnimState.normalizedTime <= 0.0f) {
				animator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, -1, 1.0f);
			}
		}
	}

	bool IsStanding() {
		return animator.GetCurrentAnimatorStateInfo(0).fullPathHash == standingAnimHash;
	}
	
	bool IsWalking() {
		return animator.GetCurrentAnimatorStateInfo(0).fullPathHash == walkAnimHash;
	}


	bool IsWalkingBackward(){
		return IsWalking() && animator.speed < 0;
	}


	public override Vector3 GetInputMovement(){
		inputVelocity = Vector3.Normalize(targetPoint - transform.position) * velocity;
		inputVelocity = Vector3.Normalize(inputVelocity) * (inputVelocity.magnitude / velocityReference);
		//Debug.Log("Input velocity: " + inputVelocity);
		return inputVelocity;
	}
	
	public override bool HasInputAttackA(){
		ExtraColliders targetColliders = target == null ? null : target.GetComponent<ExtraColliders>();
		return attack && (targetColliders == null || targetColliders.IsGrounded());
	}

	public override bool HasInputAttackB(){
		ExtraColliders targetColliders =  target == null ? null : target.GetComponent<ExtraColliders>();
		return attack && targetColliders != null && !targetColliders.IsGrounded();
	}

}
