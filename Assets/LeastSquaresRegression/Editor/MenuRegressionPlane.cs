using UnityEditor;
using UnityEngine;

public class MenuRegressionPlane : MonoBehaviour {

	[MenuItem ("GameObject/3D Object/RegressionPlane")]
	static void CreateRegressionPlane(){
		//GameObject plane = GameObject.CreatePrimitive (PrimitiveType.Plane);
		GameObject plane = Instantiate(Resources.Load("DSplane")) as GameObject;
		plane.name = "RegressionPlane";
		plane.AddComponent<RegressionPlane> ();
	}

}
