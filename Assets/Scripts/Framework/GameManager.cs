using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	private static GameManager _instance = null;              	//Static instance of GameManager which allows it to be accessed by any other script.

	public static GameManager Instance {
		get { return _instance; }
		set { _instance = value; }
	}
		
	//Awake is always called before any Start functions
	void Awake() {

		//Check if instance already exists
		if (Instance == null) {

			//if not, set instance to this
			Instance = this;
		}

		//If instance already exists and it's not this:
		else if (Instance != this) {
			//Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
			Destroy(gameObject);    
		}

		//Sets this to not be destroyed when reloading scene
		DontDestroyOnLoad(gameObject);
	}
		
	void Update() {

	}
}