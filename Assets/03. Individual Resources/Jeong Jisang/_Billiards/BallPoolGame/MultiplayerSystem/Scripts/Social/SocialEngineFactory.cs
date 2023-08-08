using UnityEngine;
using System.Collections;

#if UNITY_ANDROID

namespace BallPool
{
    using NetworkManagement;
    using UnityEngine;

#if UNITY_ANDROID && USE_PICO && GPGS
    //using GooglePlayGames.BasicApi;
    //using GooglePlayGames.OurUtils;
#endif

    internal class SocialEngineFactory
    {
        internal static SocialEngine GetPlatformSocialEngine()
        {
            if (Application.isEditor)
            {
                return new SocialExample();
            }

#if UNITY_ANDROID && USE_PICO && GPGS
            //GPGS
            return null;

#elif UNITY_ANDROID && USE_PICO && NO_GPGS
            //KT
            return null;
#endif
            //dummy
            return new SocialExample();
        }
    }
}

#endif
