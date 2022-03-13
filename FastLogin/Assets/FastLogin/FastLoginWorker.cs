using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using UniRx.Async;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace Turkcell.FastLogin {

	public class FastLoginWorker : IDisposable {

		readonly FastLoginConfiguration config;
		
		private InAppBrowserBridge browserBridge;

		public UserInfo userInfo { get; private set; }

		GameObject gameObject;

		public FastLoginWorker(FastLoginConfiguration config) {
			gameObject = new GameObject(nameof(InAppBrowserBridge));
			this.config = config;

			browserBridge = gameObject.AddComponent<InAppBrowserBridge>();
		}

		public void Dispose() {
			if (gameObject != null)
				GameObject.Destroy(gameObject);
		}

		public async UniTask<UserInfo> Work() {
			InAppBrowser.DisplayOptions displayOptions = new InAppBrowser.DisplayOptions();
			displayOptions.hidesTopBar = true;
			displayOptions.hidesHistoryButtons = true;

			bool waitingUserInfo = true;
			UnityAction<string> onBrowserStartedLoading = async (string url) => {
				Uri loadingURL = new Uri(url);
				string host = loadingURL.Host;

				if (host.Equals(new Uri(config.redirectUrl).Host)) {
					string code = HttpUtility.ParseQueryString(loadingURL.Query).Get("code");

					InAppBrowser.CloseBrowser();

					var accessToken = await GetToken(code);
					userInfo = await GetUserInfo(accessToken.access_token);
					waitingUserInfo = false;
				}
			};
			browserBridge.onBrowserStartedLoading.AddListener(onBrowserStartedLoading);

			InAppBrowser.OpenURL(PrepareAuthorizationEndPoint(), displayOptions);

			await UniTask.WaitUntil(() => { return !waitingUserInfo; });
			return userInfo;
		}

		
		public async UniTask<AccessToken> GetToken(string code) {
			WWWForm form = new WWWForm();
			form.AddField("grant_type", "authorization_code");
			form.AddField("code", code);
			form.AddField("redirect_uri", config.redirectUrl);

			var postRequest = UnityWebRequest.Post(config.tokenEndpoint, form);
			postRequest.downloadHandler = new DownloadHandlerBuffer();
			postRequest.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
			postRequest.SetRequestHeader("Accept", "text/plain, application/json, application/json, application/*+json, application/*+json, text/plain, */*, */*");
			postRequest.SetRequestHeader("Access-Control-Allow-Origin", "mobcon.turkcell.com.tr");
			postRequest.SetRequestHeader("Authorization", "Basic " + Base64Encode(config.clientID + ":" + config.clientSecret));

			await postRequest.SendWebRequest();
			return JsonConvert.DeserializeObject<AccessToken>(postRequest.downloadHandler.text);
		}

		public async UniTask<UserInfo> GetUserInfo(string accessToken) {
			var getRequest = UnityWebRequest.Get(config.userInfoEndpoint);
			getRequest.downloadHandler = new DownloadHandlerBuffer();
			getRequest.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
			getRequest.SetRequestHeader("Accept", "application/json");
			getRequest.SetRequestHeader("Authorization", "Bearer " + accessToken);
			getRequest.SetRequestHeader("Access-Control-Allow-Origin", "mobcon.turkcell.com.tr");

			await getRequest.SendWebRequest();
			return JsonConvert.DeserializeObject<UserInfo>(getRequest.downloadHandler.text);
		}


		string PrepareAuthorizationEndPoint() {
			var authorizationUriBuilder = new UriBuilder(config.authorizationEndpoint);
			var queryCollection = HttpUtility.ParseQueryString(authorizationUriBuilder.Query);
			queryCollection.Set("redirect_uri", config.redirectUrl);
			authorizationUriBuilder.Query = queryCollection.ToString();
			return authorizationUriBuilder.ToString();
		}

		static string Base64Encode(string plainText) {
			var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
			return Convert.ToBase64String(plainTextBytes);
		}

		
	}

}

