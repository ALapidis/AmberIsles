using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class testMenu : MonoBehaviour {

	public GameObject inputUI;

	[SerializeField]
	private Animator[] animators;	//Array of sections for the tab system to animate everything at once

	[SerializeField]
	private RectTransform invRect;	//Reference to the inventoryPanel's rect transform

	[SerializeField]
	private RectTransform charRect;	//Reference to the inventoryPanel's rect transform

	private bool isReset = false;	//Checks weather the order of the character menu has been reset

	void Start () {

		charRect = animators[3].gameObject.GetComponent<RectTransform>();
	}

	void LateUpdate() {

		//menu_close_anim is playing AND isReset = false
		if (animators[3].GetCurrentAnimatorStateInfo(0).IsName("Menu_Closed_Idle") && isReset == false) {
			ResetMenuOrder();
		}
	}

	public void ResetMenuOrder() {

		charRect.SetAsLastSibling();	//Resets the character panel as the active tab
		invRect.SetAsLastSibling();		//Resets the inventory panel as the active tab
		isReset = true;
	}

	public void NotReset() {

		isReset = false;
	}

	public void ToggleMenu() {

		foreach ( Animator animator in animators) {	//Runs through the array and triggers the bool and triggers in each animator
	
			if (!animator.GetBool("menuIsOpen")) {
				animator.SetTrigger("openMenu");
				animator.SetBool("menuIsOpen", true);
				inputUI.SetActive(false);

			} else {
				animator.SetTrigger("closeMenu");
				animator.SetBool("menuIsOpen", false);
				inputUI.SetActive(true);
			}
		}
	}		
}
