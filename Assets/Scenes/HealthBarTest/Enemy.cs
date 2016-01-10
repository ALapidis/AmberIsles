using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	public float enemyMaxLife = 100;
	public float enemyCurrLife = 0;

	// Use this for initialization
	void Start () {
		enemyCurrLife = enemyMaxLife;
	}

	// Update is called once per frame
	void Update () {
	
	}
}
