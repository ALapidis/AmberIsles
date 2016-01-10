using UnityEngine;
using System.Collections;

public class DisableTranisitions : StateMachineBehaviour {

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.SetBool ("DisableTransitions", true);
		animator.SetBool ("HeavyCombo", false);
	}

}
