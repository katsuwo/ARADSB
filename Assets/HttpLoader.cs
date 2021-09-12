using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class HttpLoader : MonoBehaviour {
	[SerializeField] private string serverAddress;
	[SerializeField] private int port = 5000;
	[SerializeField] private HttpResultParser _parser;

	private string getAllDataEndPoint = "all";
	private string getUpdateDataEndPoint = "lastupdate";
    
	public enum HttpLoadState {
		HTTPLOAD_ALLDATA,
		HTTPLOAD_UPDATE,
		HTTPLOAD_MAX
	}
    
	private HttpLoadState _httpLoadState;

	private CancellationToken _token;
	private double lastUpdate = 0;

	private int _frameCount = 0;
	// Start is called before the first frame update
	void Start() {
//		Debug.Log("Unity Main Thread ID:" + Thread.CurrentThread.ManagedThreadId);
		_token = this.GetCancellationTokenOnDestroy();
	}

	// Update is called once per frame
	async void Update() {
		string url = "";
		
		if (_frameCount++ % 6 != 0) return; 
		
		if (_httpLoadState == HttpLoadState.HTTPLOAD_ALLDATA) {
			url = $"http://{serverAddress}:{port}/{getAllDataEndPoint}";
		}
		else {
			url = $"http://{serverAddress}:{port}/{getUpdateDataEndPoint}/{lastUpdate}";
		} 
		var result = await DoAsync(url, _token);
		if (result != "") {
			if (_httpLoadState == HttpLoadState.HTTPLOAD_ALLDATA) _httpLoadState = HttpLoadState.HTTPLOAD_UPDATE;
			lastUpdate = _parser.parse(result);
		}
	}

	private async UniTask<string> DoAsync(string url, CancellationToken token) {
		try {
			var result = await GetAsync(url, token);
			return result;
		}
		catch (InvalidOperationException e) {
			Debug.LogException(e);
			return "";
		}
	}
    
	private async UniTask<string> GetAsync(string url,CancellationToken token) {
		using (var uwr = UnityWebRequest.Get(url)) {
			await uwr.SendWebRequest().WithCancellation(token);
			return uwr.downloadHandler.text;
		}
	}
	
	//map.GeoToWorldPosition
}