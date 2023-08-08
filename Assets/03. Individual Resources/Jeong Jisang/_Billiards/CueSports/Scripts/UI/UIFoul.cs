using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BallPool;
using UnityEngine.Animations;

namespace Billiards
{

    public class UIFoul : MonoBehaviour
    {
        [SerializeField]
        private GameObject root;
        [SerializeField]
        private PositionConstraint positionConstraint;
        [SerializeField]
        private AudioSource source;
        [SerializeField]
        private AudioClip foulClip_eng;
        [SerializeField]
        private AudioClip foulClip_chn;

        private CoroutineWrapper animationRoutine;
        private CoroutineWrapper waitRoutine;

        private void Awake()
        {
            switch (LevelLoader.CurrentMatchInfo.gameType)
            {
                case GameType.PocketBilliards:
                    AightBallPoolGameLogic.OnFoul += AightBallPoolGameLogic_OnFoul;
                    AightBallPoolGameLogic.OnFoulWithDelay += AightBallPoolGameLogic_OnFoulWithDelay;
                    break;

                //case GameType.CaromThree:
                //    CaromGameLogic.OnFoul += AightBallPoolGameLogic_OnFoul;
                //    CaromGameLogic.OnFoulWithDelay += AightBallPoolGameLogic_OnFoulWithDelay;
                //    break;

                case GameType.CaromFour:
                    CaromFourGameLogic.OnFoul += AightBallPoolGameLogic_OnFoul;
                    CaromFourGameLogic.OnFoulWithDelay += AightBallPoolGameLogic_OnFoulWithDelay;
                    break;
            }

            animationRoutine = CoroutineWrapper.Generate(this);
            waitRoutine = CoroutineWrapper.Generate(this);
            root.SetActive(false);

            //SetLocalization();
            GameSettingCtrl.AddLocalizationChangedEvent(SetLocalization);
        }

        private void SetLocalization(LanguageState state)
        {
            var isLanguageChn = state == LanguageState.tchinese ||
                state == LanguageState.schinese;

            if (isLanguageChn)
                source.clip = foulClip_chn;
            else
                source.clip = foulClip_eng;
        }

        private void AightBallPoolGameLogic_OnFoulWithDelay(AightBallPoolGameLogic.FoulType arg1, (BallListener, BallPool.Mechanics.PocketListener) arg2, float arg3)
        {
            waitRoutine.Start(waitForDelay()).SetOnComplete(() => AightBallPoolGameLogic_OnFoul(arg1, arg2));
            IEnumerator waitForDelay()
            {
                yield return new WaitForSeconds(arg3);
            }
        }

        private void AightBallPoolGameLogic_OnFoul(AightBallPoolGameLogic.FoulType type, (BallListener, BallPool.Mechanics.PocketListener) info)
        {
            var count = positionConstraint.sourceCount;
            if (count != 0)
            {
                while (positionConstraint.sourceCount > 0)
                {
                    positionConstraint.RemoveSource(0);
                }
            }

            positionConstraint.AddSource(new ConstraintSource() { sourceTransform = GetSourceTransform(), weight = 1 });

            source.Play();
            BilliardsDataContainer.Instance.isFoulVoicePlaying.Value = true;
            StartCoroutine(WaitWhilePlay(source, () => BilliardsDataContainer.Instance.isFoulVoicePlaying.Value = false));

            animationRoutine.StartSingleton(RunAnimation()).SetOnComplete(() => root.SetActive(false));

            //localFunctions
            IEnumerator RunAnimation()
            {
                root.SetActive(true);
                yield return new WaitForSeconds(1.5f);
            }

            Transform GetSourceTransform()
            {
                switch (type)
                {
                    case AightBallPoolGameLogic.FoulType.Scratch: return info.Item2.transform;
                    case AightBallPoolGameLogic.FoulType.BadHit: return info.Item1.transform;
                    case AightBallPoolGameLogic.FoulType.NoHit: return info.Item1.transform;
                    case AightBallPoolGameLogic.FoulType.WeakBreak: return info.Item1.transform;
                    default: return null;
                }
            }
        }

        private IEnumerator WaitWhilePlay(AudioSource target, System.Action onComplete)
        {
            yield return new WaitWhile(() => target.isPlaying);
            onComplete?.Invoke();
        }


        private void OnDestroy()
        {
            CaromGameLogic.OnFoul -= AightBallPoolGameLogic_OnFoul;
            CaromGameLogic.OnFoulWithDelay -= AightBallPoolGameLogic_OnFoulWithDelay;

            CaromFourGameLogic.OnFoul -= AightBallPoolGameLogic_OnFoul;
            CaromFourGameLogic.OnFoulWithDelay -= AightBallPoolGameLogic_OnFoulWithDelay;

            AightBallPoolGameLogic.OnFoulWithDelay -= AightBallPoolGameLogic_OnFoulWithDelay;
            AightBallPoolGameLogic.OnFoul -= AightBallPoolGameLogic_OnFoul;
        }
    }

}