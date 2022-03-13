﻿using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class InAppBrowserBridge : MonoBehaviour {

	[System.Serializable]
	public class BrowserLoadingCallback: UnityEvent<string> {
		
	}

	[System.Serializable]
	public class BrowserLoadingWithErrorCallback: UnityEvent<string, string> {
		
	}

	public BrowserLoadingCallback onJSCallback = new BrowserLoadingCallback();

	public BrowserLoadingCallback onBrowserFinishedLoading = new BrowserLoadingCallback();

	public BrowserLoadingCallback onBrowserStartedLoading = new BrowserLoadingCallback();

	public BrowserLoadingWithErrorCallback onBrowserFinishedLoadingWithError = new BrowserLoadingWithErrorCallback();

	public UnityEvent onBrowserClosed = new UnityEvent();

	public UnityEvent onAndroidBackButtonPressed = new UnityEvent();

	public bool enableLogging = false;

	void OnBrowserJSCallback(string callback) {
		if (enableLogging)
			Debug.Log("InAppBrowser: JS Message: " + callback);
		onJSCallback.Invoke(callback);
	}

	void OnBrowserFinishedLoading(string url) {
		if (enableLogging)
			Debug.Log("InAppBrowser: FinishedLoading " + url);
		onBrowserFinishedLoading.Invoke(url);
	}

	void OnBrowserStartedLoading(string url) {
		if (enableLogging)
			Debug.Log("InAppBrowser: StartedLoading " + url);
		onBrowserStartedLoading.Invoke(url);
	}

	void OnBrowserFinishedLoadingWithError(string urlAndError) {
		if (enableLogging)
			Debug.Log("InAppBrowser: FinishedLoading With Error " + urlAndError);
		string[] parts = urlAndError.Split(',');
		Debug.Log("URL:"+parts[0]);
		Debug.Log("ERROR:"+parts[1]);
		onBrowserFinishedLoadingWithError.Invoke(parts[0], parts[1]);
	}

	void OnBrowserClosed() {
		if (enableLogging)
			Debug.Log("InAppBrowser: Closed");
		onBrowserClosed.Invoke();
	}

	void OnAndroidBackButtonPressed() {
		if (enableLogging)
			Debug.Log("InAppBrowser: Android back button pressed");
		onAndroidBackButtonPressed.Invoke();
	}
}
