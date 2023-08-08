#if !UNITY_EDITOR
#if UNITY_ANDROID
#define ANDROID_DEVICE
#elif UNITY_IPHONE
#define IOS_DEVICE
#elif UNITY_STANDALONE_WIN
#define WIN_DEVICE
#endif
#endif
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
#if UNITY_ANDROID
public class Callback_LeaderBoard : MonoBehaviour
{

    //private static string IS_SUCCESS = "isSuccess";
    //private static string MSG = "msg";
    //private static string CODE = "code";
    ///// <summary>
    ///// 登陆后本地缓存一份token，用于查询
    ///// </summary>
    ///// <param name="LoginInfo"></param>
    //public void LoginCallback(string LoginInfo)
    //{
    //    JsonData jsrr = JsonMapper.ToObject(LoginInfo);
    //    if (jsrr[IS_SUCCESS] != null)
    //    {
    //        CommonDic.getInstance().isSuccess = jsrr[IS_SUCCESS].ToString();
    //    }
    //    if (jsrr[MSG] != null)
    //    {
    //        CommonDic.getInstance().loginMsg = jsrr[MSG].ToString();
    //    }
 
    //    if (LeaderBoardCtrl.instance != null)
    //    {
    //        LeaderBoardCtrl.instance.LoginUpdate();
    //    }
    //}

    //public void UserInfoCallback(string userInfo)
    //{
    //    CommonDic.getInstance().user_info = userInfo;
    //}

    //public void ActivityForResultCallback(string activity)
    //{
    //    PicoPaymentSDK.jo.Call("authCallback", activity);
    //}
}
#endif