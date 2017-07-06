using UnityEngine;
using System.Collections;


enum Direction {Left, Right, Up, Down, None};

class DirectionDownControl{
	// Used to check when up/down/left/right
	// are down (pressed in a single frame)

	private bool isLeftDown, isRightDown, isUpDown,isDownDown;
	private bool isLeftHolded, isRightHolded, isUpHolded,isDownHolded;

	public void CheckAxisDown(){

		// reset down states
		isLeftDown = isRightDown = isUpDown = isDownDown = false;

		// Right
		if (Input.GetAxisRaw("Horizontal") > 0) {
			if (!isRightHolded) {
				isRightDown = isRightHolded = true;
			}
		}else{
			isRightHolded = false;
		}

		// Left
		if (Input.GetAxisRaw("Horizontal") < 0) {
			if (!isLeftHolded) {
				isLeftDown = isLeftHolded = true;
			}
		}else{
			isLeftHolded = false;
		}

		// Up
		if (Input.GetAxisRaw("Vertical") > 0) {
			if (!isUpHolded) {
				isUpDown = isUpHolded = true;
			}
		}else{
			isUpHolded = false;
		}

		// Down
		if (Input.GetAxisRaw("Vertical") < 0) {
			if (!isDownHolded) {
				isDownDown = isDownHolded = true;
			}
		}else{
			isDownHolded = false;
		}

	}

	public bool GetDirectionDown(Direction direction){

		switch (direction) {
			case Direction.Right: return isRightDown;
			case Direction.Left: return isLeftDown;
			case Direction.Up: return isUpDown;
			case Direction.Down: return isDownDown;
			default: return false;
		}
	}

} // DirectionDownControl


public class BasicInputProvider: InputProvider {
	// provide the basic input

	public float directionKeyCooler = 0.2f;
	public float actionKeyCooler = 0.1f;
	// Coolers are used to still recognize an action a few time
	// after key being pressed. Essential for smooth controls

	private float directionCooler = 0;
	private int tapCount = 0;
	private Direction tappedDirection = Direction.None;
	// Direction control variables

	private float[] actionCooler = new float[6];
	// Cooler for the 6 action keys

	private DirectionDownControl directionControl = new DirectionDownControl();



	void Update() {

		directionControl.CheckAxisDown();

		CheckDoubleDirection();

		CheckActionButtons();

	}


	
	private void CheckActionButtons() {

		// Update coolers
		for (int i = 0 ; i < actionCooler.Length ; ++i) {
			actionCooler[i] -= Time.deltaTime;
			if (actionCooler[i] < 0) actionCooler[i] = 0;
		}

		// Check input buttons
		if (Input.GetButtonDown("AttackA"))	actionCooler[0] = actionKeyCooler;
		if (Input.GetButtonDown("AttackB"))	actionCooler[1] = actionKeyCooler;
		if (Input.GetButtonDown("AttackC"))	actionCooler[2] = actionKeyCooler;
		if (Input.GetButtonDown("Jump"))	actionCooler[3] = actionKeyCooler;
		if (Input.GetButtonDown("Special"))	actionCooler[4] = actionKeyCooler;
		if (Input.GetButtonDown("Extra"))	actionCooler[5] = actionKeyCooler;

	}


	private bool CheckBothAxisHoldedAtOnce() {
		return Input.GetAxisRaw("Horizontal") != 0 && Input.GetAxisRaw("Vertical") != 0;
	}

	

	private bool CheckDoubleDirectionForDirection(Direction direction, bool hasPreviousTap) {
		if (directionControl.GetDirectionDown(direction)){
			if (hasPreviousTap && tappedDirection == direction) {
				++tapCount;
			}else{
				tappedDirection = direction;
				tapCount = 1;
			}
			directionCooler = directionKeyCooler;
			return true;
		}
		return false;
	}


	private void CheckDoubleDirection() {

		if (CheckBothAxisHoldedAtOnce()) {
			tapCount = 0;
			directionCooler = 0;
			tappedDirection = Direction.None;
		} else {

			// Check current cooler
			if (directionCooler > 0) {
				directionCooler -= Time.deltaTime;
			}else {
				tapCount = 0;
				directionCooler = 0;
				tappedDirection = Direction.None;
			}

			bool hasPreviousTap = directionCooler > 0 && tapCount > 0;

			// This unused bool has the purpose of simulating a
			// fake if-else-if on the function calls
			#pragma warning disable 219
			bool fakeIfElseIf =
				CheckDoubleDirectionForDirection(Direction.Right, hasPreviousTap)
				|| CheckDoubleDirectionForDirection(Direction.Left, hasPreviousTap)
				|| CheckDoubleDirectionForDirection(Direction.Up, hasPreviousTap)
				|| CheckDoubleDirectionForDirection(Direction.Down, hasPreviousTap)
			;
			#pragma warning restore 219

		}

	}
	
	public override Vector3 GetInputMovement(){
		return new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
	}
	
	public override bool HasInputAttackA(){
		return actionCooler[0] > 0;
	}

	public override bool HasInputAttackB(){
		return actionCooler[1] > 0;
	}

	public override bool HasInputAttackC(){
		return actionCooler[2] > 0;
	}
	
	public override bool HasInputJump(){
		return actionCooler[3] > 0;
	}
	
	public override bool HasInputSpecial(){
		return actionCooler[4] > 0;
	}

	public override bool HasInputExtra(){
		return actionCooler[5] > 0;
	}
	
	public override bool HasInputDoubleLeft(){
		return tapCount > 1 && tappedDirection == Direction.Left;
	}

	public override bool HasInputDoubleRight(){
		return tapCount > 1 && tappedDirection == Direction.Right;
	}

	public override bool HasInputDoubleUp(){
		return tapCount > 1 && tappedDirection == Direction.Up;
	}

	public override bool HasInputDoubleDown(){
		return tapCount > 1 && tappedDirection == Direction.Down;
	}


	public override bool IsHoldingInputAttackA(){
		return Input.GetButton("AttackA");
	}

	public override bool IsHoldingInputAttackB(){
		return Input.GetButton("AttackB");
	}

	public override bool IsHoldingInputAttackC(){
		return Input.GetButton("AttackC");
	}

	public override bool IsHoldingInputJump(){
		return Input.GetButton("Jump");
	}

	public override bool IsHoldingInputSpecial(){
		return Input.GetButton("Special");
	}

	public override bool IsHoldingInputExtra(){
		return Input.GetButton("Extra");
	}

	public override bool HasSpecialInputEvent(string eventID){
		// TODO: Search on the attached SpecialInputEvents for it
		// TODO: Use a dictionary to store them, populate it at initialization
		return false;
	}

}
