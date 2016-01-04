using UnityEngine;
using System.Collections;

public class ClearAttackTriggersOnExit : StateMachineBehaviour {

	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		animator.ResetTrigger ("Attack1");
		animator.ResetTrigger ("Attack2");
		animator.ResetTrigger ("Attack3");
		animator.ResetTrigger ("ChargedAttack");
		animator.SetBool ("HeavyCombo", false);
	}
}
