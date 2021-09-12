using System;
using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using UnityEngine;

public class Aircraft : MonoBehaviour {
	// Start is called before the first frame update
	public AbstractMap map;
	public bool isInitialized;
	public Vector2d latitudeLongitude;
	public double lastUpdateTime = 0;
	public PositionBuffer positionBuffer;

	void Start() {
		latitudeLongitude = new Vector2d(0, 0);
		map.OnInitialized += () => isInitialized = true;
		positionBuffer = new PositionBuffer();
	}

	// Update is called once per frame
	void Update() { }

	public void SetGeoPosition(double latitude, double longitude, double _altitude, double updatetime) {
		//	if (isInitialized == false) return;
		var altitude = _altitude / 3 / 3;
		if (altitude < 0) altitude = 0;
		latitudeLongitude = new Vector2d(latitude, longitude);
		Vector3 position = map.GeoToWorldPosition(latitudeLongitude);
		transform.position = new Vector3((float) position.x, (float) altitude, (float) position.z);
		Vector3 movement = positionBuffer.update(position.x,altitude, position.z, updatetime); 
		
		GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		marker.transform.localPosition = transform.localPosition;
	}

	public bool ComparePosition(double latitude, double longitude) {
		if (latitudeLongitude.x == latitude && latitudeLongitude.y == longitude) return true;
		return false;
	}
}

public class PositionBuffer {
	public const int MAXBUFFERSIZE = 10;
	public List<PositionObject> positionList;
	public List<float> magnitudeList;

	public PositionBuffer() {
		positionList = new List<PositionObject>();
		magnitudeList = new List<float>();
	}

	public Vector3 update(double x, double y, double altitude, double time) {
		Vector3 pos = new Vector3((float) x, (float) y, (float) altitude);
		PositionObject posObj = new PositionObject(pos, time);
		positionList.Add(posObj);
		if (positionList.Count >= MAXBUFFERSIZE) {
			positionList.RemoveAt(0);
		}

		Vector3 currentMovementVector = new Vector3(0, 0, 0);
		if (positionList.Count >= 2) {
			var newPos = positionList[positionList.Count - 1];
			var oldPos = positionList[positionList.Count - 2];
			var mag1Sec = get1SecMagnitude(newPos, oldPos);
			currentMovementVector = (newPos.position) - (oldPos.position );
		}
	
		return currentMovementVector;
	}

	public float get1SecMagnitude(PositionObject newPos, PositionObject oldPos) {
		double timeDiff = newPos.updateTime - oldPos.updateTime;
		Vector3 vectDiff = newPos.position - oldPos.position;
		float mag1Sec = vectDiff.magnitude * (1.0f / (float) timeDiff);
		return mag1Sec;
	}
}

public class PositionObject {
	public Vector3 position;
	public double updateTime;

	public PositionObject(Vector3 position, double updateTime) {
		this.position = position;
		this.updateTime = updateTime;
	}
}