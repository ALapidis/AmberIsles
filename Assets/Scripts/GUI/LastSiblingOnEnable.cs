using UnityEngine;
using System.Collections;

public class LastSiblingOnEnable : MonoBehaviour {

		void OnEnable() {
			this.transform.SetAsLastSibling();
		}
}
