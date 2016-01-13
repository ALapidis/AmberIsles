using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEditor;

/* 																										*/
/* This class handles the enabling, and disabling of the floating healthbar uGUI slider under the 		*/
/* NPC. The script also passes the npc's health to the slider, and updates it's screenspace position	*/
/* based on the 3D object's world space. Healthbar is disabled once NPC's health regenerates to full.	*/ 

public class FloatingHealthbar : MonoBehaviour {

	#region Properties

	public MeleeEnemy enemy;											// Enemy.cs script pointer
	private Transform transformCache;								// Temporary variable to update after each frame to move the healthbar prefab to.
	private Mesh mobMesh;
	private Rect offset;											// Screenspace boundaries of the gameobject for use in offsetting the healthbar
	private Camera viewCamera;										// Gets the main camera for worldspacetoscreenspace calculations.
	private Quaternion rotation;

	public Canvas floatingHealthBarCanvas;						// The canvas to parent the slider to once enabled
	public Slider enemyHealthSlider;                                // Reference to the Enemy health bar Slider UI element.	

	#endregion

	void Start () {
		
		viewCamera = Camera.main;

		// pointer to the Enemy,cs script component
		enemy = gameObject.GetComponent<MeleeEnemy>();

			// Store the transform of the slider object
		transformCache = enemyHealthSlider.GetComponent<Transform>();

		// Initalize the health bar's current and maximum health
		enemyHealthSlider.maxValue = enemy.maxHealth;
		enemyHealthSlider.value = enemy.health;

		// Get the bounds of the gameobject's render mesh and pass the height to offset
		mobMesh = gameObject.GetComponent<MeshFilter>().mesh;
		offset = BoundsToScreenRect(mobMesh.bounds);
	}

	void LateUpdate () {

		// Check if the current health is less than the max AND if the healthbar is disabled AND  the enemy tag is not Destructable
		if (enemyHealthSlider != null) {
			if (enemy.health < enemy.maxHealth && !enemyHealthSlider.gameObject.activeSelf && enemy.tag != "Destructable") {
				
				EnableHealthBar();
			} else if (enemy.health == enemy.maxHealth && enemyHealthSlider.gameObject.activeSelf && enemy.tag != "Destructable") {

				DisableHealthBar();
			} 
		}


		if (enemy != null) {
			
			if (transformCache != null) {

				// Update the health bar's value after every frame.
				enemyHealthSlider.value = enemy.health; 

				// Move the healthbar to the parent's location after every frame.
				transformCache.localPosition = WorldToScreen(enemy.transform.position);
			}
		} 
	}

	// Gets the mesh origin and farthest extents as a Rect
	public Rect BoundsToScreenRect(Bounds bounds) {
		
		Vector3 origin = Camera.main.WorldToScreenPoint(new Vector3(bounds.min.x, bounds.max.y, 0f));
		Vector3 extent = Camera.main.WorldToScreenPoint(new Vector3(bounds.max.x, bounds.min.y, 0f));

		return new Rect(origin.x, Screen.height - origin.y, extent.x - origin.x, origin.y - extent.y);
	}

	// Re-parent back to the gameobject and disables the floatingHealthbarSlider
	void DisableHealthBar() {

		enemyHealthSlider.gameObject.transform.SetParent(gameObject.transform, false);
		enemyHealthSlider.gameObject.SetActive(false);
		enemyHealthSlider.transform.localPosition = Vector3.zero;
	}

	// Enables and re-parents the floatingHealthbarSlider to the FloatingHealthBarCanvas
	void EnableHealthBar() {
		
		enemyHealthSlider.gameObject.SetActive(true);
		enemyHealthSlider.gameObject.transform.SetParent(floatingHealthBarCanvas.transform, false);
		enemyHealthSlider.transform.localPosition = Vector3.zero;
	}

	//Returns the screen positon from world positon.
	public Vector3 WorldToScreen(Vector3 worldPos) {
		
		//Converts world position to viewport position based on viewing camera
		Vector3 screenPos = viewCamera.WorldToViewportPoint(worldPos);

		//Scale to Screen for canvas. Viewport location is (0.0f TO 1.0f)
		screenPos.x *= Screen.width;
		screenPos.y *= Screen.height;

		//Shift TopLeft
		screenPos.x -= Screen.width * 0.50f;
		screenPos.y -= ((Screen.height * 0.50f) - ((offset.height / 1.50f) * gameObject.transform.localScale.y));

		screenPos.z = 0;

		return screenPos;
	}
}
