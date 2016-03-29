/*
Zone Controller Pro - By John Rossitter
john@smarterphonelabs.com
*/
using UnityEngine;
using System.Collections;

public class ZoneEffectSimpleRotation : MonoBehaviour 
{
	
	[SerializeField]
	public float X;
	
	[SerializeField]
	public float Y;
	
	[SerializeField]
	public float Z;
	
	[SerializeField]
	public float RotationSpeed;
	
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		float DiffX =0.0f;
		float DiffY =0.0f;
		float DiffZ =0.0f;
		if(X != 0.0f)
		{
		if(transform.rotation.eulerAngles.x	> X)
		{
			DiffX = DiffX - RotationSpeed;
		}
		else
		{
			DiffX = DiffX + RotationSpeed;
		}
		}
		
		if(Y != 0.0f)
		{
		if(transform.rotation.eulerAngles.y	> Y)
		{
			DiffY = DiffY - RotationSpeed;
		}
		else
		{
			DiffY = DiffY + RotationSpeed;
		}
		}
		
		if(Z != 0.0f)
		{
		if(transform.rotation.eulerAngles.z	> Z)
		{
			DiffZ = DiffZ - RotationSpeed;
		}
		else
		{
			DiffZ = DiffZ + RotationSpeed;
		}
		}
		transform.Rotate(new Vector3(DiffX,DiffY,DiffZ));
	}
}
