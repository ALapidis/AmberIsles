using UnityEngine;
using System.Collections;

namespace Completed
{   
	public class Loader : MonoBehaviour 
	{
		public GameObject gameManager;          //GameManager prefab to instantiate.
		//public GameObject soundManager;         //SoundManager prefab to instantiate.


		void Awake () {
			//Check if a GameManager has already been assigned to static variable GameManager.instance or if it's still null
			if (GameManager.Instance == null) {

				//Find a reference to the Managment game object
				//Transform managment = GameObject.Find("Managment").transform;

				//Instantiate gameManager prefab and create a reference to the instance
				GameObject gmInstance = (GameObject)Instantiate(gameManager);

				//Parents the Game Manager instance to the Managment object for organization
				gmInstance.transform.SetParent(GameObject.Find("Managment").transform);
			}

			//Check if a SoundManager has already been assigned to static variable GameManager.instance or if it's still null
//			if (SoundManager.instance == null)
//
//				//Instantiate SoundManager prefab
//				Instantiate(soundManager);
		}
	}
}