using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
This is a test Unity Component that demonstrates how you can register your existing plugins to be certified with Zone.
While you do not have to make these changes to your existing scripts at all, there is some good reasons to do so.

First of all, if you use this format, you can specify which fields you want to be made visible to Zone, otherwise it will use System.Reflection to introspect on your class
and discover all fields you make available, including private and public. So if you have internal variables which you do not want Zone to touch, it would be a good idea to 
exclude them in the ZonePluginActivator() method.

In addition to that, we also use a registration system which will let your script know when certain fields are under the control of Zone. In these cases if your code is trying 
to update those fields within an update function or some other code, you may want to check that it is not being currently controlled by Zone.

Make sure to add a reference to System.Generic to this class if you intend to use the ZonePluginActivator() method as it returns a List<string>() object
*/

//An example of a simple Enumerator you can use to send to Zones GUI builder

public enum MyEnum
{
	val1 = 1,
	val2 = 2,
	val3 = 3,
}

//A test class that ingerits from MonoBehaviour
public class TestPlugin : MonoBehaviour {

	//a public and serialized int field
	[SerializeField]
	public int SomeInt;

	//a public float field
	public float SomeFloat;

	//a public string field
	[SerializeField]
	public string SomeString;

	//a public boolean field
	public bool SomeBool;

	//a public and serialzed texture field
	[SerializeField]
	public Texture SomeTexture;

	//a public and serialized texture2d field
	[SerializeField]
	public Texture2D SomeTexture2D;

	//an instance of the enumerator declared at the top of this file
	[SerializeField]
	public MyEnum SomeEnum;


	[SerializeField]
	public Vector2 MyVector2;

	[SerializeField]
	public Vector3 MyVector3;

	[SerializeField]
	public Vector4 MyVector4;

	public void DoFoo(string Foo, string Bar)
	{
		Debug.Log(Foo + Bar);
	}

	//a public serialized color
	[SerializeField]
	public Color SomeColor;

	/*
		These boolean variables are used to track when other fields are under control of Zone, again these are completely optional.
	*/

	//track if the SomeInt field is being used by Zone
	private bool SomeIntUnderExternalControl = false;

	//Zone is notifying you that the specified property is now under it's control
	public void ZonePluginZoneEnter(string Property)
	{
		//	Debug.Log("A Zone is now in control of this plugin property: " + Property );
		//if the property is SomeInt
		if(Property == "SomeInt")
		{
			//lock the boolean
			SomeIntUnderExternalControl = true;
		}
	}

	//Zone is notifying you that the spepcified property is no longer under it's control.
	public void ZonePluginZoneExit(string Property)
	{
		//Debug.Log("A zone is no longer in control of this plugin property: " + Property);

		//if the property is SomeInt
		if(Property == "SomeInt")
		{
			//unlock the boolean
			SomeIntUnderExternalControl = false;
		}
	}

	//this is the method Zone will call to get a list of the properties you wish to expose.
	//Only list the items you want to expose here, if you dont want to expose anything, just omit this method entirely
	public static List<string> ZonePluginActivator()
	{
		//the returned data is sent in a pipe delimited string format for each field you wish to expose in the following layout:
		//Name, Type, Min, Max, Protection Level, Description

		//declare the return value list
		List<string> RetVal = new List<string>();


		//adding an int value to be published to Zone
		string Item1 = "SomeInt|System.Int32|0|100|1|This is SomeInt, it is responsible for doing XYZ...";
		RetVal.Add(Item1);

		//adding a float value to be published to Zone
		string Item2 ="SomeFloat|System.Single|0.0|20.0|1|This is SomeFloat, it is responsible for doing XYZ...";
		RetVal.Add(Item2);

		//adding a boolean value to be published to Zone
		string Item3 ="SomeBool|System.Boolean|0|1|1|This is SomeBool, it is responsible for doing XYZ...";
		RetVal.Add(Item3);

		//adding a color value to be published to Zone
		string Item4 ="SomeColor|UnityEngine.Color|0|1|1|This is ComeColor, it is responsible for doing XYZ...";
		RetVal.Add(Item4);

		//adding an enumerated int value to be published to Zone (notice the json style formatting)
		string Item5 ="SomeEnum|Enum{1='val1',2='val2',3='val3'}|0|1|1|This is SomeEnum, it is responsible for doing XYZ...";
		RetVal.Add(Item5);

		//adding a Texture value to be published to Zone
		string Item6 ="SomeTexture|UnityEngine.Texture|0|1|1|This is SomeTexture, it is responsible for doing XYZ...";
		RetVal.Add(Item6);

		//adding a Texture2D value to be published to Zone
		string Item7 ="SomeTexture2D|UnityEngine.Texture2D|0|1|1|This is SomeTexture2D, it is responsible for doing XYZ...";
		RetVal.Add(Item7);

		//adding a string value to be published to Zone
		string Item8 ="SomeString|string|0|0|1|This is SomeString, it is responsible for doing XYZ...";
		RetVal.Add(Item8);

		//adding a string value to be published to Zone
		string Item9 ="MyVector2|UnityEngine.Vector2|0|0|1|This is a Test Vector 2, it is responsible for doing XYZ...";
		RetVal.Add(Item9);

		//adding a string value to be published to Zone
		string Item10 ="MyVector3|UnityEngine.Vector3|0|0|1|This is Test Vector 3, it is responsible for doing XYZ...";
		RetVal.Add(Item10);

		//adding a string value to be published to Zone
		string Item11 ="MyVector4|UnityEngine.Vector4|0|0|1|This is Test Vector 4, it is responsible for doing XYZ...";
		RetVal.Add(Item11);

		//return the list to Zone
		return RetVal;
	}


	//this is the method Zone will call to get a list of the methods you wish to expose.
	//Only list the items you want to expose here, if you dont want to expose anything, just omit this method entirely
	public static List<string> ZonePluginActivatorMethods()
	{
		//crate a List of string to return
		List<string> RetVal = new List<string>();

		//add a Method Signature in C# style (no spaces between the parameters)
		string Method1="DoFoo(System.String Foo,System.String Bar)";

		//add it to the stack
		RetVal.Add(Method1);

		//return the list
		return RetVal;

	}
	// Update is called once per frame
	void Update () 
	{
		//check to see if the SomeInt validation boolean is in use
		if(SomeIntUnderExternalControl == false)
		{
			//if not, continue your normal use of the property
			SomeInt = Random.Range(0,1000000);
		}
		
	}


}
