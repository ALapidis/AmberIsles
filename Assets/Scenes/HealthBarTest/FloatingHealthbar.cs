using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEditor;

public class FloatingHealthbar : MonoBehaviour {

	#region Properties

	private Enemy enemy;											// Enemy.cs script pointer
	private Transform transformCache;								// Temporary variable to update after each frame to move the healthbar prefab to.
	private Mesh mobMesh;
	private Rect offset;											// Screenspace boundaries of the gameobject for use in offsetting the healthbar

	public Camera viewCamera;										// Gets the main camera for worldspacetoscreenspace calculations.
	public GameObject floatingHealthBarCanvas;						// The canvas to parent the slider to once enabled
	public Slider enemyHealthSlider;                                // Reference to the Enemy health bar Slider UI element.	

	#endregion

	void Start () {

		// pointer to the Enemy,cs script component
		enemy = gameObject.GetComponent<Enemy>();

		// Store the transform of the slider object
		transformCache = enemyHealthSlider.GetComponent<Transform>();

		// Initalize the health bar's current and maximum health
		enemyHealthSlider.maxValue = enemy.enemyMaxLife;
		enemyHealthSlider.value = enemy.enemyCurrLife;

		// Get the bounds of the gameobject's render mesh and pass the height to offset
		mobMesh = gameObject.GetComponent<MeshFilter>().mesh;
		offset = BoundsToScreenRect(mobMesh.bounds);
	}

	void LateUpdate () {

		// Check if the current health is less than the max AND if the healthbar is disabled AND  the enemy tag is not Destructable
		if (enemy.enemyCurrLife < enemy.enemyMaxLife && !enemyHealthSlider.gameObject.activeSelf && enemy.tag != "Destructable") {
			
			EnableHealthBar();
		} else if (enemy.enemyCurrLife == enemy.enemyMaxLife && enemyHealthSlider.gameObject.activeSelf && enemy.tag != "Destructable") {

			DisableHealthBar();
		}

		if (enemy != null) {
			
			if (transformCache != null) {

				// Update the health bar's value after every frame.
				enemyHealthSlider.value = enemy.enemyCurrLife; 

				if (enemy.enemyCurrLife <= 0) {

					Destroy(enemyHealthSlider.gameObject);
				}

				// Move the healthbar to the parent's location after every frame.
				transformCache.localPosition = WorldToScreen(enemy.transform.position);
			}
		} else {
			
			//If the parent is destroyed via death, there is no need for this health bar to exist.
			Destroy(enemyHealthSlider.gameObject);
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
		
		enemyHealthSlider.gameObject.SetActive(false);
		enemyHealthSlider.transform.localPosition = Vector3.zero;
	}

	// Enables and re-parents the floatingHealthbarSlider to the FloatingHealthBarCanvas
	void EnableHealthBar() {
		
		enemyHealthSlider.gameObject.SetActive(true);
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
