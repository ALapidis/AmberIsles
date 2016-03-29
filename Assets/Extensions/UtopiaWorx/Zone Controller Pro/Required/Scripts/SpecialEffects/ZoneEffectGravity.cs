/*
Zone Controller Pro - By John Rossitter
john@smarterphonelabs.com
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZoneEffectGravity : MonoBehaviour {

	[SerializeField]
	public float Gravity;

	private float GravityMonitor;

	// Use this for initialization
	void Start () 
	{
		GravityMonitor = Gravity;
	}

	// Update is called once per frame
	void Update () 
	{
		if(GravityMonitor != Gravity)
		{
			GravityMonitor = Gravity;
			Physics.gravity = new Vector3(0,Gravity,0);

		}
	}

	//this is the method Zone will call to get a list of the properties you wish to expose.
	//Only list the items you want to expose here, if you dont want to expose anything, just omit this method entirely
	public static List<string> ZonePluginActivator()
	{
		//the returned data is sent in a pipe delimited string format for each field you wish to expose in the following layout:
		//Name, Type, Min, Max, Protection Level

		//declare the return value list
		List<string> RetVal = new List<string>();


		//adding an int value to be published to Zone
		string Item1 = "Gravity|System.Single|-19.81|19.81|1|Adjusts the overall powere of vertical gravity.";
		RetVal.Add(Item1);


		//return the list to Zone
		return RetVal;
	}
}
