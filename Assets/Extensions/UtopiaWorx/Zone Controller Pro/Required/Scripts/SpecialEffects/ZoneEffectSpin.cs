/*
Zone Controller Pro - By John Rossitter
john@smarterphonelabs.com
*/
using UnityEngine;
using System.Collections;

public class ZoneEffectSpin : MonoBehaviour {

	[SerializeField]
	public float YSpin;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(new Vector3(0,YSpin * Time.timeScale,0));
	}
}
