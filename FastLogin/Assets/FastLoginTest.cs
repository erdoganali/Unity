using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FastLoginTest : MonoBehaviour
{
    public Text text;

    public async void OnClickFastLogin()
    {
        var info = await FastLogin.GetUserInfo();
        text.text = "Phone: " + info.phone_number + "" + info.phone_number_verified; 
    }

    
}
