using System;
using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.Map;
using UnityEngine;
using MiniJSON;

public class HttpResultParser : MonoBehaviour {
	[SerializeField] private AbstractMap _map;
	private Dictionary<string, GameObject> _aircraftDict;
	private Dictionary<string, Aircraft> _aircraftComponentDict;
	private bool _isInitialized;
	void Start() {
		_aircraftDict = new Dictionary<string, GameObject>();
		_aircraftComponentDict = new Dictionary<string, Aircraft>();
		_map.OnInitialized += () => {
			_isInitialized = true;
		};
	}

	void Update() {
	}

	public double parse(string text) {
		IDictionary js = (IDictionary)Json.Deserialize(text);
		var items = (IDictionary)js["Items"];
		var keys = items.Keys;
		foreach (KeyValuePair<string, object> kvp in items as Dictionary<string, object>) {
			try {
				var tmpDic = (IDictionary)Json.Deserialize(kvp.Value as string);
				string icao = (string)tmpDic["icao"];
				
//				if (icao != "85155E") continue;
				
				var tmpCallsign = (string)tmpDic["callsign"];
				var tmpLongitude = (double)tmpDic["longitude"];
				var tmpLatitude = (double)tmpDic["latitude"];
				var tmpAltitude = double.Parse((string)tmpDic["altitude"]) * 0.33;
				var tmpUpdateTime = (double) tmpDic["update_time_stamp"];
				if (tmpLatitude == 0.0 && tmpLongitude == 0.0 && tmpAltitude == 0.0) continue; 
				Debug.Log($"{icao} : {tmpCallsign} {tmpLatitude} {tmpLongitude}");
				
				if (_aircraftComponentDict.ContainsKey(icao) == false) {
//					GameObject newAircraft = new GameObject();
					GameObject newAircraft = GameObject.CreatePrimitive (PrimitiveType.Sphere);
					newAircraft.name = icao;
					Aircraft aircraftComponent = newAircraft.AddComponent<Aircraft>() as Aircraft;
					aircraftComponent.map = _map;
					aircraftComponent.isInitialized = _isInitialized;
					aircraftComponent.lastUpdateTime = tmpUpdateTime;
					_aircraftDict.Add(icao, newAircraft);
					_aircraftComponentDict.Add(icao, aircraftComponent);
				}
		
				Aircraft aircraftCp = _aircraftComponentDict[icao];
				if (aircraftCp.ComparePosition(tmpLatitude, tmpLongitude) == false) {
					aircraftCp.SetGeoPosition(tmpLatitude, tmpLongitude, tmpAltitude, tmpUpdateTime);
				}
			}
			catch (Exception e) {
			}
		}
		return double.Parse((string)js["ReadTime"]);
	}
}