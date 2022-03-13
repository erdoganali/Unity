using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Turkcell.FastLogin;
using UniRx.Async;

public class FastLogin
{

	/// <summary>
	/// Gets user info from fastLogin servers with the configuration stored in resources folder root location with file name of FastLoginConfiguration
	/// </summary>
	/// <returns></returns>
	public async static UniTask<UserInfo> GetUserInfo() {
		return await GetUserInfo(Resources.Load<FastLoginConfiguration>("FastLoginConfiguration"));
	}

	public async static UniTask<UserInfo> GetUserInfo(FastLoginConfiguration config) {
		FastLoginWorker worker = new FastLoginWorker(config);
		var userInfo  = await worker.Work();
		worker.Dispose();
		return userInfo;
	}

}
