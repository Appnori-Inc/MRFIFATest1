using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkManagement
{
    /// <summary>
    /// The login manager.
    /// </summary>
    public class LoginManager
    {
        private static bool _isLogined;
        public static bool logined
        {
            get 
            { 
                if (!_isLogined)
                {
                    _isLogined = DataManager.GetIntData("IsLogined") == 1;
                }
                return _isLogined;
            }
            set 
            { 
                _isLogined = value; 
                DataManager.SetIntData("IsLogined", _isLogined?1:0); 
            }
        }
        public static bool loginedFacebook
        {
            get 
            { 
                return FacebookManager.instance.IsLoggedIn;
            }
        }

        public static bool TotalLogined
        {
            get
            {
#if UNITY_ANDROID && !NO_GPGS && USE_PICO
                //pico with gpgs
                //TODO : insert GPGS here
                return true;

#elif !UNITY_ANDROID && NO_GPGS
                //vive with steam?
                return true;

#else
                //Develop
                Debug.Log($"accesse in Development code : {typeof(LoginManager)} , TotalLogined");
                return true;
#endif
            }
        }

    }
}
