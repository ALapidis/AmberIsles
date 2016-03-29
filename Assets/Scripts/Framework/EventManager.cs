using UnityEngine;
using System.Collections;

public class EventManager : MonoBehaviour 
{
	public delegate void ClickAction();
	public static event ClickAction OnClicked;


	void OnGUI()
	{
		if(GUI.Button(new Rect(Screen.width / 2 - 50, 5, 100, 30), "Click"))
		{
			if(OnClicked != null)
				OnClicked();
		}
	}

// To register a method with the event manager.. add this block to the script.

//	void OnEnable()
//	{
//		EventManager.OnClicked += Method;
//	}
}