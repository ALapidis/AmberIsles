using UnityEngine;

public class FollowPlayer : MonoBehaviour {

	Character character;

	void Start(){
		character = GetComponent<Character>();
	}

	void Update(){
		GameObject player = CharacterManager.GetClosestPlayer(transform);
		transform.LookAt(player.transform.position);
		transform.Translate(transform.forward * character.moveSpeed * Time.deltaTime);
	}

}
