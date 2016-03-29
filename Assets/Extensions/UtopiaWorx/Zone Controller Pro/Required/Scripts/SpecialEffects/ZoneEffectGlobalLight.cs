/*
Zone Controller Pro - By John Rossitter
john@smarterphonelabs.com
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZoneEffectGlobalLight : MonoBehaviour {

	[SerializeField]
	public float AmbientIntensity;

	private float AmbientIntensityMonitor;

	// Use this for initialization
	void Start () 
	{
		AmbientIntensityMonitor = AmbientIntensity;
	}

	// Update is called once per frame
	void Update () 
	{
		if(AmbientIntensityMonitor != AmbientIntensity)
		{
			AmbientIntensityMonitor = AmbientIntensity;
			RenderSettings.ambientIntensity = AmbientIntensity;

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
		string Item1 = "AmbientIntensity|System.Single|0.0|8.0|1|Power of the overall Global Lighting";
		RetVal.Add(Item1);


		//return the list to Zone
		return RetVal;
	}
}
