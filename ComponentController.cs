using UnityEngine;
using System.Collections;

namespace Model{
	public class ComponentController : MonoBehaviour {
		public GameObject[] Elevation;
		public GameObject[] Garage;
		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		/*Leave all house objects as active, but change their visibility layer according
		to the house state. An inspector helper class would list category 
		arrays such as elevation, garage, etc and it would be up to the user
		to classify objects as being part of that category through drag-and-drop.
		On a state change, Linq would search dynamically for objects that
		match the new state and set them to visible.
		*/
	}
}

