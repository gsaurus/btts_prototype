using UnityEngine;

public abstract class InputProvider: MonoBehaviour {
	// Gives characters all info they need to perform their moves.
	// Examples of providers: Keyboard; AI; network; touch screen;

	// Movement
	public virtual Vector3 GetInputMovement()	{ return Vector3.zero; }
	public virtual bool HasInputDoubleLeft() 	{ return false; }
	public virtual bool HasInputDoubleRight()	{ return false; }
	public virtual bool HasInputDoubleUp()		{ return false; }
	public virtual bool HasInputDoubleDown()	{ return false; }
	// Usually used to run and blitz moves
	// Run in a touchpad can for example be decided by how far the touch is from the start touch point


	// Six button input
	public virtual bool HasInputAttackA()	{ return false; }
	public virtual bool HasInputAttackB()	{ return false; }
	public virtual bool HasInputAttackC()	{ return false; }
	public virtual bool HasInputJump()		{ return false; }
	public virtual bool HasInputSpecial()	{ return false; }
	public virtual bool HasInputExtra()		{ return false; }
	public virtual bool IsHoldingInputAttackA()	{ return false; }
	public virtual bool IsHoldingInputAttackB()	{ return false; }
	public virtual bool IsHoldingInputAttackC()	{ return false; }
	public virtual bool IsHoldingInputJump()	{ return false; }
	public virtual bool IsHoldingInputSpecial()	{ return false; }
	public virtual bool IsHoldingInputExtra()	{ return false; }
	// Three types of attack.
	// Example 1: In Streets of Rage A is standard combos, B is back attack, C is combo finalization
	// Example 2: In Street Fighter A is low, B is med, C is high
	// Extra can be used for cop-calls, etc

	public virtual bool HasSpecialInputEvent(string specialID)	{ return false; }
	// Check if a specific input event happened.
	// This can be used to detect key combinations,
	// or for extra AI movements
	
}
