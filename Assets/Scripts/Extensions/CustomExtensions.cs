using UnityEngine;
using System.Collections;

public static class CustomExtensions {

	public static void TurnOnOrOffGameObject( this GameObject gO) {

		if (gO.activeSelf) {

			gO.SetActive(false);
		} else {

			gO.SetActive(true);
		}
	}
}
