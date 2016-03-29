using UnityEngine;
using System.Collections;

public static class IncreasExperience {

	private static int xpToGive = 0;
	//private static LevelUp levelUpScript = new LevelUp();

	public static void AddExperience() {
//		xpToGive = GameInformation.PlayerLevel * 100;
//		GameInformation.CurrentXP += xpToGive;
		CheckToSeeIfPlayerLeveled();
		Debug.Log(xpToGive);
	}

	private static void CheckToSeeIfPlayerLeveled() {
//		if(GameInformation.CurrentXP >= GameInformation.RequiredXP) {
			// then we've leveled up
//			levelUpScript.LevelUpCharacter();
//		}
	}
}