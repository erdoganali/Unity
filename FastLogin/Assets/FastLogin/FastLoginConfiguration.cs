using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Turkcell.FastLogin {

	[CreateAssetMenu(fileName = "FastLoginConfiguration", menuName = "Config/FastLogin")]
	public class FastLoginConfiguration : ScriptableObject {

		[Header("Credentials")]
		public string clientID;
		public string clientSecret;

		[Header("Urls")]
		[TextArea] public string redirectUrl;
		[Header("")]
		[TextArea] public string authorizationEndpoint;
		[Header("")]
		[TextArea] public string tokenEndpoint;
		[Header("")]
		[TextArea] public string userInfoEndpoint;

		

	}

}
