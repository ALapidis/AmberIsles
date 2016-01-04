using UnityEngine;
using System.Collections;

public class ExpManager : MonoBehaviour {

	public float currentExperience;
	public float maxExperience;
	public float currentLevel;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void IncreaseExperience(float amount)  
	{  
		currentExperience += amount;
		while (currentExperience >= maxExperience) {
			currentLevel++;  
			//LevelUp();  
			//StatManager.instance.LevelUpStats();  
			currentExperience -= maxExperience;  
			maxExperience = maxExperience * 2;  
		}
	}  

}
