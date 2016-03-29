using UnityEngine;
using System.Collections;

public class LevelUp {

	public int maxPlayerLevel = 50;

	public void LevelUpCharacter() {
		// check to see if current XP is greater than required
//		if (GameInformation.CurrentXP > GameInformation.RequiredXP) {
//			GameInformation.CurrentXP -= GameInformation.RequiredXP;
//		} else {
//			GameInformation.CurrentXP = 0;
//		}
//		if (GameInformation.PlayerLevel < maxPlayerLevel) {
//			GameInformation.PlayerLevel += 1;
//		}else {
//			GameInformation.PlayerLevel = maxPlayerLevel;
//		}
		//give player stat points
		//determine next ammount of exp
		DetermineRequiredXP();
	}

	private void DetermineRequiredXP() {
//		float temp = (GameInformation.PlayerLevel * 1000) + 250;
//		GameInformation.RequiredXP = (int)temp;
	}

}
