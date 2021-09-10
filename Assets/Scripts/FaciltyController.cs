using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using UnityEngine;

public class FaciltyController : MonoBehaviour {
	public double latitude;
	public double longitude;
	public AbstractMap map;
	public float initialY = 0.11f;		//ZoomLevel15の時のY値
	public float initialScale = 0.002f;	//ZoomLevel15の時のScale
	
	//RJTT 35.54702, 139.77648
	//139.78025,35.55457

	// Start is called before the first frame update
	void Start() {
		// var localPos = gameObject.transform.localPosition;
		// var cord = map.WorldToGeoPosition(localPos);
		// Debug.Log(cord);
		map.MapVisualizer.OnMapVisualizerStateChanged += (ModuleState s) => {
			switch (s) {
				case ModuleState.Finished:
					AlignToMap();
					break;
				case ModuleState.Working:
				case ModuleState.Initialized:
					break;
				default:
					break;
			}
		};
	}

	// Update is called once per frame
	void Update() {
	}

	public void AlignToMap() {
		Vector3 worldPosition = map.GeoToWorldPosition(new Vector2d(latitude, longitude), true);
		gameObject.transform.parent = map.transform;

		float scale = initialScale / Mathf.Pow(2f, 15f - map.Zoom);
		float y = initialY / Mathf.Pow(2f, 15f - map.Zoom);
		gameObject.transform.localPosition = new Vector3(worldPosition.x, y, worldPosition.z);
		gameObject.transform.localScale = new Vector3(scale, scale, scale);
	}
}
