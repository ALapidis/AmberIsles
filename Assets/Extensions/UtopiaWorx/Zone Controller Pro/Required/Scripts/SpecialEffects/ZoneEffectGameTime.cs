/*
Zone Controller Pro - By John Rossitter
john@smarterphonelabs.com
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZoneEffectGameTime : MonoBehaviour {

	[SerializeField]
	public float GameTime;

	private float GameTimeMonitor;

	// Use this for initialization
	void Start () 
	{
		GameTimeMonitor = GameTime;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(GameTimeMonitor != GameTime)
		{
			GameTimeMonitor = GameTime;
			Time.timeScale = GameTime;

			AudioSource[] AS = FindObjectsOfType<AudioSource>();
			foreach(AudioSource MyAS in AS)
			{
				MyAS.pitch = Time.timeScale;

			}


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
		string Item1 = "GameTime|System.Single|0.1|2|1|Game Speed";
		RetVal.Add(Item1);


		//return the list to Zone
		return RetVal;
	}
}
