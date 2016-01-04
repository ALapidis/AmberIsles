using UnityEngine;
using System.Collections;

public class DeadZoneFollow : MonoBehaviour 
{
	public float deadZone = 5f;			// Dead Zone the camera will not track
	private float deadZoneY = 0f;		// Dead Zone the camera will not track
	private float Smooth = 2f;			// How smoothly the camera catches up with it's target movement in the x axis.
	public float heightOffset = 15;		// Camera height offset
	public float distanceOffset = -30;	// Camera distance offset (negative value in Unity 5)
	private Transform player;			// Reference to the player's transform.


	void Awake () {
		// Setting up the reference.
		player = GameObject.Find("PlayerTemp").transform;
	}

	void FixedUpdate () {
		TrackPlayer();
	}
		
	void TrackPlayer()
	{
		// Store the players current position with offsets taken into account
		Vector3 target = new Vector3(Mathf.Abs(transform.position.x - player.position.x), Mathf.Abs(transform.position.y - (player.position.y + heightOffset)), Mathf.Abs(transform.position.z - (player.position.z + distanceOffset)));

		// Should each axis move?  This contains 3 bools "(yes, no, no)"
		Vector3 shouldMove = new Vector3(target.x > deadZone ? 1.0f : 0.0f, target.y > deadZoneY ? 1.0f : 0.0f, target.z > deadZone ? 1.0f : 0.0f);

		// Apply smoothing here
		shouldMove = shouldMove * Smooth * Time.deltaTime; 

		// The opposite of should move
		Vector3 dontMove = new Vector3(1.0f, 1.0f, 1.0f) - shouldMove; 
		
		// New position
		Vector3 newPosition = player.position + new Vector3(0.0f, heightOffset, distanceOffset);
		
		// Vector3.Lerp using 3 bools
		target = Vector3.Scale(transform.position, dontMove) + Vector3.Scale(newPosition, shouldMove);
		
		// Set the camera's position to the target position with offsets
		transform.position = target;
	}
	
}
