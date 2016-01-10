using UnityEngine;
using System.Collections;

/* 																																*/
/* This class handles the display and inputs from the abilities buttons near the attack/evade/block array of buttons.			*/
/* It will handle the construction of a list, populate it from the skills the player has assigned to each button, then handle	*/
/* the display and upkeep of those buttons as well as passing their effects to the combat manager when triggered.				*/

public class PlayerAbilitiesController : MonoBehaviour {


	/* 		Ability Cooldown Psudo Code */
	//		if (mouse click & Character.Cooldown = False)
	//		Character.cTimer = 0
	//		Character.cEndTime = txtEndTimer.text
	//		Character.Cooldown = True
	//		CooldownBar.Width = 0
	//		CooldownBar.AnimationFrame = 1
	//
	//		If Character.Cooldown = True
	//		Character.cTimer = Character.cTimer + dt
	//		CooldownBar.Width = (CooldownBar.MaxWidth / Character.cEndTime) * Character.cTimer
	//
	//		If Character.cTimer >= Character.cEndTime
	//		Character.cTimer = -1
	//		Character.Cooldown = False
	//		CooldownBar.Width = CooldownBar.MaxWidth
	//		CooldownBar.AnimationFrame = 0


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
