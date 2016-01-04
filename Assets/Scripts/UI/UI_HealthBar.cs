using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_HealthBar : MonoBehaviour
{
    [HideInInspector]
    public GameObject target;										// gameObject where the healthbar prefab will exist/track.
	public GameObject mob;											// Parent gameObject of the healthbar's gameObject. (The NPC root where the scripts are attached)
	
	private Transform transformCache;								// Temporary variable to update after each frame to move the healthbar prefab to.
	private MeleeEnemy targetMob;									// Initalize targetMob as a MeleeEnemy class.

	public Slider enemyHealthSlider;                                // Reference to the Enemy health bar Slider UI element.	

	
    void Start()
    {
		// Get the current parent's transform.
        transformCache = GetComponent<Transform>();
		targetMob = mob.GetComponent<MeleeEnemy>();
		// Assign the health bars current and max values.
		enemyHealthSlider.maxValue = targetMob.maxHealth;
		enemyHealthSlider.value = targetMob.health;
    }

	void LateUpdate()
    {
        if (target != null)
        {
            if (transformCache != null)
            {
				// Update the health bar's value after every frame.
				enemyHealthSlider.value = targetMob.health; 
				if (targetMob.health <= 0)
					Destroy(gameObject);
				// Move the healthbar to the parent's location after every frame.
            	transformCache.localPosition = UI_Manager.Instance.WorldToScreen(target.transform.position);
            }
        }
        else
        {
            //If the parent is destroyed via death, there is no need for this health bar to exist.
            Destroy(gameObject);
        }
    }
}
