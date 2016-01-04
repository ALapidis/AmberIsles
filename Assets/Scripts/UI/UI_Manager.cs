using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_Manager : MonoBehaviour {

    private static UI_Manager instance;
    public static UI_Manager Instance { get { return instance; } }						// Creates an instance of UI Manager to manage the scene.

    [SerializeField]
    private Camera viewCamera;															// Gets the main camera for worldspacetoscreenspace calculations.

    [SerializeField]
    private Canvas mainCanvas;															// Which canvas object should the UI prefab be created under?
    
	[SerializeField]
    private UI_HealthBar healthBarPrefab;												// The prefab to use for the healthbar instance.

    void Awake()
    {
        instance = this;															
    }

	// Instanciates a healthbar prefab and passes the gameObject to the variable "target" in the healthbar script attached to that prefab. 
	// Also grabs the parent of the gameObject (which is just an empty object for healthbar placement) to get access to the NPC's scripts, and assigns it to "mob".
    public void CreateHealthBar(GameObject target)
    {
        UI_HealthBar healthBar = Instantiate(healthBarPrefab);
        healthBar.gameObject.transform.parent = mainCanvas.transform;
        healthBar.transform.localPosition = Vector3.zero;
        healthBar.target = target;
		healthBar.mob = target.transform.parent.gameObject;
    }

    //Returns the screen positon from world positon.
    public Vector3 WorldToScreen(Vector3 worldPos)
    {
        //Converts world position to viewport position based on viewing camera
        Vector3 screenPos = viewCamera.WorldToViewportPoint(worldPos);

        //Scale to Screen for canvas. Viewport location is (0.0f TO 1.0f)
        screenPos.x *= Screen.width;
        screenPos.y *= Screen.height;

        //Shift TopLeft
        screenPos.x -= Screen.width * 0.50f;
        screenPos.y -= Screen.height * 0.50f;

        screenPos.z = 0;

        return screenPos;
    }
}
