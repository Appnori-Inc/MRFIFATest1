using UnityEngine;
using System.Collections;

namespace Billiards
{
    using BallPool;
    using Photon.Realtime;
    using System;
    using System.Collections.Generic;
    using UnityEngine.UI;

    [Serializable]
    public class DisplayPlayerInfo
    {
        BallsUIManager ballsUIManager;

        public Text PlayerName;
        public List<Image> ballsImage;
        public List<Text> ballsText;
        public Image TurnImage;

        public Text CaromScore;

        private BallPoolPlayer cachedData;

        public void Initialize(BallsUIManager manager)
        {
            ballsUIManager = manager;
            PlayerName.text = "NICKNAME";

            for (int i = 0; i < ballsImage.Count; i++)
            {
                ballsText[i].text = "";
                ballsImage[i].sprite = ballsUIManager.defaultBall;
                Color color = ballsUIManager.defaultColor;
                ballsImage[i].color = new Color(color.r, color.g, color.b);
                //ballsImage[i].gameObject.SetActive(false);
            }
        }

        public void SetPlayer(BallPoolPlayer playerData)
        {
            cachedData = playerData;

            PlayerName.text = cachedData.name;
        }

        public void SetTurn(bool isOn)
        {
            var color = TurnImage.color;

            color.a = isOn ? 1 : 0.1f;

            TurnImage.color = color;
        }

        public void SetActiveBallsIds(BallPoolPlayer player)
        {
            var activeBallsIds = player.GetActiveBallsIds();
            if (activeBallsIds == null)
                return;

            //int value = 0;
            //for (int i = 0; i < activeBallsIds.Length; ++i)
            //{
            //    int id = int.Parse(activeBallsIds[i]);
            //    value |= 1 << id;
            //}

            //BilliardsDataContainer.Instance.TargetBallIds.CurrentData = value;


            for (int i = 0; i < ballsImage.Count; i++)
            {
                if (i < activeBallsIds.Length)
                {
                    ballsImage[i].transform.parent.gameObject.SetActive(true);

                    int id = int.Parse(activeBallsIds[i]);
                    ballsText[i].text = id + "";
                    switch (player)
                    {
                        case AightBallPoolPlayer p when p.isSolids:
                            ballsImage[i].sprite = ballsUIManager.solidsBall;
                            break;

                        case AightBallPoolPlayer p when AightBallPoolGameLogic.isBlackBall(id):
                            ballsImage[i].sprite = ballsUIManager.solidsBall;
                            break;

                        default:
                            ballsImage[i].sprite = ballsUIManager.stripesBall;
                            break;
                    }

                    Color color = ballsUIManager.ballsColors[id - 1];
                    ballsImage[i].color = new Color(color.r, color.g, color.b);
                }
                else
                {
                    ballsImage[i].transform.parent.gameObject.SetActive(false);
                }
            }
        }

        public void SetCaromScore(in int score)
        {
            CaromScore.text = score.ToString();
        }

        public void SetCaromText(in string message)
        {
            CaromScore.text = message;
        }
    }


    public class UIIngameDisplay : MonoBehaviour
    {
        [SerializeField]
        private BallsUIManager ballsUIManager;

        /*[SerializeField]
        private RectTransform root;*/
        [SerializeField]
        private RectTransform[] displayCanvas;

        [SerializeField]
        private Text TitleText;
        private int lifeCount = 5;
        [SerializeField]
        private int add = 0;
        private int point_my = 0;
        private int point_other = 0;
       
        [SerializeField]
        private List<Image> single_listLife;
        [SerializeField]
        private Animator lifeAnim;
        [SerializeField]
        private List<Image> listPointBar;
        [SerializeField]
        private List<Image> listPoint;
        [SerializeField]
        private List<Image> multi_listPointMy;
        [SerializeField]
        private List<Image> multi_listPointOther;
        [SerializeField]
        private Animator pointAnim;
        [SerializeField]
        private Animator pointAnimMultiR;
        [SerializeField]
        private Animator pointAnimMultiL;

        [SerializeField]
        private DisplayPlayerInfo myInfo;
        [SerializeField]
        private DisplayPlayerInfo otherInfo;
        [SerializeField]
        private DisplayPlayerInfo myInfo_single;

        [SerializeField]
        private RectTransform TurnBoard;

        [SerializeField]
        private Text TurnTimeCounterText;
        [SerializeField]
        private Text TurnTimeCounterText_Single;

        [SerializeField]
        private PlayAgainMenu ResultMenu;

        public static UIIngameDisplay Instance;

        private static event Action<UIIngameDisplay> tempEvent = null;
        public static event Action<UIIngameDisplay> OnInitializedEvent
        {
            add
            {
                if (Instance != null)
                {
                    value?.Invoke(Instance);
                    return;
                }

                tempEvent += value;
            }
            remove
            {
                tempEvent -= value;
            }
        }


        private void Awake()
        {
            if ((object)Instance != null)
            {
                Debug.LogError($"Another instance is alreay exist. remove {typeof(UIIngameDisplay)} instance");
                Destroy(gameObject);
                return;
            }

            Instance = this;
            if (tempEvent != null)
            {
                tempEvent.Invoke(this);
                tempEvent = null;
            }

            myInfo.Initialize(ballsUIManager);
            otherInfo.Initialize(ballsUIManager);
            if (LevelLoader.CurrentMatchInfo.gameType != GameType.PocketBilliards)
                myInfo_single.Initialize(ballsUIManager);

            RebuildLayout();
            ResultMenu.onActive += ResultMenu_onActive;

            ResultMenu.Hide();
            //root.gameObject.SetActive(true);
            CurrLvSetting(LevelLoader.CurrentMatchInfo.level);
            if (LevelLoader.CurrentMatchInfo.gameType != GameType.PocketBilliards)
            {
                InitializeCaromMode();
            }

        }

        private void CurrLvSetting(int Lv)
        {
            if (LevelLoader.CurrentMatchInfo.playingType == PlayType.Single)
            {
                add = 5 * (5 - Lv);
                pointAnim.SetInteger("State", add);
                pointAnim.SetBool("OnAni", false);

                for (int i = 0; i < listPointBar.Count; i++)
                {
                    if (i == Lv - 1)
                        listPointBar[i].gameObject.SetActive(true);
                    else
                        listPointBar[i].gameObject.SetActive(false);
                }

                for (int i = 0; i < listPoint.Count; i++)
                {
                    if (i < 5 * (5 - Lv))
                        listPoint[i].gameObject.SetActive(false);
                    else
                        listPoint[i].gameObject.SetActive(true);
                }
            }
           
        }
        

        private void InitializeCaromMode()
        {
            BilliardsDataContainer.Instance.CaromLife.OnDataChanged += CaromLife_OnDataChanged;
            
            if (LevelLoader.CurrentMatchInfo.playingType == PlayType.Single)
            {
               /* myInfo.PlayerName.text = "Score";
                otherInfo.PlayerName.text = "Life";

                myInfo.SetCaromText(string.Format("{0} / {1}", 0, GameConfig.CurrentCaromSingleTargetScore));
                otherInfo.SetCaromText(string.Format("{0} / {1}", BilliardsDataContainer.Instance.CaromLife.Value, GameConfig.CaromSinglePlayerLife));*/

                myInfo_single.PlayerName.text = GameDataManager.instance.userInfo_mine.nick;
                myInfo_single.SetCaromText(string.Format("{0} / {1}", 0, GameConfig.CurrentCaromSingleTargetScore));
                TurnBoard.gameObject.SetActive(false);
                RebuildLayout();
                displayCanvas[0].gameObject.SetActive(true);
                displayCanvas[1].gameObject.SetActive(false);

               
            }
            else
            {
                myInfo.SetCaromText(string.Format("{0} / {1}", 0, GameDataManager.instance.userInfo_mine.nick));
                otherInfo.SetCaromText(string.Format("{0} / {1}", 0, GameDataManager.instance.userInfos[0].nick));
                displayCanvas[0].gameObject.SetActive(false);
                displayCanvas[1].gameObject.SetActive(true);

            }
        }

        private void CaromLife_OnDataChanged(int life) // 3구,4구 싱글 life event 함수 
        {
            if (LevelLoader.CurrentMatchInfo.playingType != PlayType.Single)
                return;

            //otherInfo.SetCaromText(string.Format("{0} / {1}", life, GameConfig.CaromSinglePlayerLife));
            StartCoroutine(DisableLife(life));
        }

        private IEnumerator DisableLife(int target)
        {

            lifeAnim.SetInteger("State", target+1);
            lifeAnim.SetBool("OnAni", true);
            yield return new WaitForSeconds(1f);
            lifeAnim.SetBool("OnAni", false);
        }


       


        private void ResultMenu_onActive(bool isActive)
        {
            //root.gameObject.SetActive(!isActive);
            if(LevelLoader.CurrentMatchInfo.playingType == PlayType.Single)
                displayCanvas[0].gameObject.SetActive(!isActive);
            else
                displayCanvas[1].gameObject.SetActive(!isActive);
            RebuildLayout();
        }

        /// <summary>
        /// 3 depth (vertical root , horizontal player, vertical ball list) => call 3 times
        /// </summary>
        private void RebuildLayout()
        {
            if (LevelLoader.CurrentMatchInfo.gameType == GameType.PocketBilliards)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(displayCanvas[0]);
                LayoutRebuilder.ForceRebuildLayoutImmediate(displayCanvas[0]);
                LayoutRebuilder.ForceRebuildLayoutImmediate(displayCanvas[0]);
            }
            else
            {
                if (LevelLoader.CurrentMatchInfo.playingType == PlayType.Single)
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(displayCanvas[0]);
                    LayoutRebuilder.ForceRebuildLayoutImmediate(displayCanvas[0]);
                    LayoutRebuilder.ForceRebuildLayoutImmediate(displayCanvas[0]);
                    LayoutRebuilder.ForceRebuildLayoutImmediate(displayCanvas[0]);
                    LayoutRebuilder.ForceRebuildLayoutImmediate(displayCanvas[0]);
                }
                else
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(displayCanvas[1]);
                    LayoutRebuilder.ForceRebuildLayoutImmediate(displayCanvas[1]);
                    LayoutRebuilder.ForceRebuildLayoutImmediate(displayCanvas[1]);
                    LayoutRebuilder.ForceRebuildLayoutImmediate(displayCanvas[1]);
                    //.ForceRebuildLayoutImmediate(displayCanvas[1]);
                }
            }
        }

        public void SetAvatar(AightBallPoolPlayer player) { /*use player.avatar*/ /*Not Implemented */ }

        public void SetPlayer(BallPoolPlayer player)
        {
            (player.playerId == 0 ? myInfo : otherInfo).SetPlayer(player);
            RebuildLayout();
        }

        public void SetActiveBallsIds(BallPoolPlayer player)
        {
            if(player.playerId == 0)
            {
                var activeBallsIds = player.GetActiveBallsIds();
                if (activeBallsIds == null)
                    return;

                int value = 0;
                for (int i = 0; i < activeBallsIds.Length; ++i)
                {
                    int id = int.Parse(activeBallsIds[i]);
                    value |= 1 << id;
                }

                BilliardsDataContainer.Instance.TargetBallIds.Value = value;
            }

            (player.playerId == 0 ? myInfo : otherInfo).SetActiveBallsIds(player);
            RebuildLayout();
        }

        public void SetTurn(int idx, bool value)
        {
            if (idx == 0)
                myInfo.SetTurn(value);
            else
                otherInfo.SetTurn(value);

            RebuildLayout();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeRaio">between 0 and 1 value</param>
        public void SetTime(float timeRaio)
        {
            if (LevelLoader.CurrentMatchInfo.playingType == PlayType.Multi)
                TurnTimeCounterText.text = Mathf.FloorToInt(Mathf.Lerp(GameConfig.MaxTurnTime, 0, timeRaio)).ToString();
            /*else if (LevelLoader.CurrentMatchInfo.playingType == PlayType.Single)
                TurnTimeCounterText_Single.text = Mathf.FloorToInt(Mathf.Lerp(GameConfig.MaxTurnTime, 0, timeRaio)).ToString();*/
        }


        public void SetPlayerScore(in int score, in int targetScore)
        {
            if (LevelLoader.CurrentMatchInfo.playingType == PlayType.Single)
            {
                myInfo_single.SetCaromText(string.Format("{0} / {1}", score, targetScore));
                StartCoroutine(OnAnimPoint(score));
            }
            else
            {
                myInfo.SetCaromText(string.Format("{0} / {1}", score, targetScore));
                StartCoroutine(OnAnimPointMulti(pointAnimMultiL, score));
            }
        }

        private void MutiPointAnim(int Point, bool ismy)
        {
            //point_my
            //point_other

            // 현재 점수 받아서 이전 점수 보다 많은지 적은지 체크 하고
            // 많으면 왼쪽 칩을 오른쪽으로 적으면 오른쪽칩을 왼쪽으로

            // 왼쪽 자신 
            // x -121.5 -> - 365
            // -243.5
            // 한칸 +10


            // 오른쪽 다른 사람
            // x 417.5 -> 174
            // -243.5
            // 한칸 +10

            Image target = multi_listPointMy[point_my];
            bool isPoint = true;


            if(ismy) // 내 점수 
            {
                target = multi_listPointMy[point_my];
                if(Point < point_my+1) isPoint = false;
            }
            else // 상대 점수 
            {
                target = multi_listPointOther[point_other];
                if (Point < point_other+1) isPoint = false;
            }




            StartCoroutine(update());

            IEnumerator update()
            {
                float initx = target.rectTransform.anchoredPosition.x;
                float addx = 0;

                while (true)
                {
                    if (isPoint &&target.rectTransform.anchoredPosition.x - initx <= -243.5f)
                    {
                        if (ismy) point_my = Point-1; // 현재 내 점수 갱신 
                        else point_other = Point-1; // 현재 상대 점수 갱신 
                        yield break;
                    }
                    if (!isPoint && target.rectTransform.anchoredPosition.x - initx >= 243.5f)
                    {
                        if (ismy) point_my = Point-1; // 현재 내 점수 갱신 
                        else point_other = Point-1; // 현재 상대 점수 갱신 
                        yield break;
                    }

                    if (isPoint) addx -= Time.deltaTime;
                    else addx += Time.deltaTime;

                    Vector2 addVec = new Vector2(target.rectTransform.anchoredPosition.x + addx, 0);

                    target.rectTransform.anchoredPosition = addVec;

                    yield return null;
                }
            }
        }


        private IEnumerator OnAnimPointMulti(Animator target,int point)
        {
            target.SetInteger("State", point);
            target.SetBool("OnAni", true);
            yield return new WaitForSeconds(1f);
            target.SetBool("OnAni", false);
        }


        private IEnumerator OnAnimPoint(int point)
        {
            pointAnim.SetInteger("State", point + add);
            pointAnim.SetBool("OnAni", true);
            yield return new WaitForSeconds(1f);
            pointAnim.SetBool("OnAni", false);
        }

        public void SetPlayerScoreFromNetwork(in int score, in int targetScore)
        {
            otherInfo.SetCaromText(string.Format("{0} / {1}", score, targetScore));
            StartCoroutine(OnAnimPointMulti(pointAnimMultiR, score));
        }

        private void OnDestroy()
        {
            BilliardsDataContainer.Instance.CaromLife.OnDataChanged -= CaromLife_OnDataChanged;
            Instance = null;
        }

    }

}