using UnityEngine;
using System.Collections.Generic;

public static class CharacterManager {

	static List<Character> characters = new List<Character>();

	public static void Register(Character character){
		if(!characters.Contains(character)){
			characters.Add(character);
		}
	}

	public static void Unregister(Character character){
		if(characters.Contains(character)){
			characters.Remove(character);
		}
	}

	public static Character[] GetCharacters<T>(){
		List<Character> items = new List<Character>();
		foreach(var entry in characters){
			if(entry.GetType() == typeof(T)){
				items.Add(entry);
			}
		}
		return items.ToArray();
	}

	public static Character[] GetCharactersByTag(string tagName){
		List<Character> items = new List<Character>();
		foreach(var entry in characters){
			if(entry.gameObject.tag == tagName){
				items.Add(entry);
			}
		}
		return items.ToArray();
	}

	public static GameObject GetClosestPlayer(Transform transform){
		GameObject player = null;
		float dist = int.MaxValue;
		foreach(var entry in characters){
			if(Vector3.Distance(transform.position, entry.transform.position) < dist && entry.characterType == CharacterType.Player){
				player = entry.gameObject;
			}
		}
		return player;
	}
}
