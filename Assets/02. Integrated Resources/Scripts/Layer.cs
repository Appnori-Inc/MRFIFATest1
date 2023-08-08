using UnityEngine;
using System.Collections;
using Billiards;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Appnori
{

    public static class Layer
    {
        public enum GameType
        {
            None,
            Billiards,
            Archery,
            Basketball,
            Bowling,
            Badminton,
            Golf,
            PingPong,
            Lobby,
            Test,
            Dart,
            Baseball,
            Boxing,
            Boxing_HB,
            Boxing_DM,
            Boxing_AI,
            Tennis,
            Boxing_MT,
        }

        /// <summary>
        /// unity m_LayerCollisionMatrix는 c802feffc804feffc840feffffffffffc808feffc800ffffffffffffffffffffc806feffc95dfeffca5ffeffd806feffc806feffc800feffcc06feffc800feffe800ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff
        /// 의 꼴로 나타나게 됨.
        /// 이는 4바이트씩 끊어서 순차적으로 0번 레이어부터 31번레이어까지의 각 데이터를 저장하며,
        /// 저장된 데이터는 리틀에디안으로 변환하면 아래와 같음.
        /// 0 1111
        /// 1 1111
        /// 2 1111
        /// 3 1111
        ///   3210
        /// </summary>

        const string billiardsMatrix = "c90480ffc80080ffc84480ffffffffffc80080ffc80081ffffffffffffffffffc806a0ffc85d80ffcd5fb8ffc80680ffc80680ffc80080ffcc0680ffc80080ffe80081ffc80080ffc80084ffc80480ffc804a0ffc805b0ffc80080ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff";
        const string ArMatrix = "c802feffc804feffc840feffffffffffc808feffc800ffffffffffffffffffffc806feffc95dfeffca5ffeffd806feffc806feffc800feffcc06feffc800feffe800ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff";
        const string ArhceryMatrix = "c80000fac80000fac80000faffffffffc80000fac80000faffffffffffffffffc81420fec80000fac80536f9c80010fac81100fac80000fac80000fac80000fac80000fac80400fac8041cfac80004fac80c34fac80530fac80000fac80000f8c80400f8fffb7ffac80100f8ffffffffffffffffffffffffffffffffffffffff";
        const string BasketballMatrix = "c80000fac80000fac80000faffffffffc80000fac80000faffffffffffffffffc80420fec80000fac80536f9c80010fac80000fac80000fac80000fac80000fac80000fac80400fac8041cfac80004fac80c34fac80530fac80000fac80000f8c80400f8fffb7ffac80100f8ffffffffffffffffffffffffffffffffffffffff";
        const string BowlingMatrix = "c80080ffc80080ffc80080ffffffffffc80080ffc80080ffffffffffffffffffc824a0ffc80080ffc805beffc80080ffc80080ffc80180ffc80080ffc80080ffc80080ffc80480ffc80480ffc80480ffc804a0ffc805b0ffc80080ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff";
        const string BadmintonMatrix = "c80080ffc80080ffc80080ffffffffffc80080ffc80080ffffffffffffffffffc824a0ffc80080ffc805beffc80080ffc80080ffc80180ffc80080ffc80080ffc80080ffc80480ffc80480ffc80480ffc804a0ffc805b0ffc80080ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff";
        const string LobbyMatrix = "c90400fac80000fac80000faffffffffc80000fac80000faffffffffffffffffc81420fec80000fac90536f9c80010fac81100fac80000fac80000fac80000fac80000fac80400fac8041cfac80004fac80c34fac80530fac80000fac80000f8c80400f8fffb7ffac80100f8ffffffffffffffffffffffffffffffffffffffff";
        const string TestMatrix = "c9001cfac80000fae80000faffffffffc80000facc0001faffffffffffffffffc80420fec80000fac87d3ef9c80400fac80400fac80400fac84400fac80000fae80001fac80402fac90404fac90408fac90420fac80530fac80000fac80000f8c80400f8fffb7ffac80100f8ffffffffffffffffffffffffffffffffffffffff";


        const string GolfMatrix = "ffc3ffffff83ffffff83ffffffffffffff83ffffff83ffffffffffffffffffffff83ffffff85ffffc882ffffc890ffffc888ffffc880ffffc980ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff";
        const string PingPongMatrix = "c80480ffc80080ffc80080ffffffffffc80080ffe80080ffffffffffffffffffc800a0ffc80480ffc902beffc80080ffc80080ffc80080ffc80080ffc80080ffc80080ffc80480ffc80480ffc80480ffc804a0ffc805b0ffc80080ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff";
        const string DartMatrix = "c80080ffc80080ffc80080ffffffffffc80080ffc80080ffffffffffffffffffc800a0ffc80080ffc818beffc80480ffc80480ffc80080ffc80080ffc80080ffc80080ffc80480ffc80480ffc80480ffc804a0ffc805b0ffc80080ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff";
        const string BaseballMatrix = "c80080ffc80080ffc80080ffffffffffc80080ffc80080ffffffffffffffffffc800a0ffc80080ffc818beffc80480ffc80480ffc80080ffc80080ffc80080ffc80080ffc80480ffc80480ffc80480ffc804a0ffc805b0ffc80080ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff";
        const string BoxMatrix = "c90180ffc80080ffc84080ffffffffffc80080ffc80081ffffffffffffffffffc906a0ffc85980ffc859beffc80680ffc80680ffc80080ffcc0680ffc80080ffe80081ffc80480ffc80480ffc80480ffc804a0ffc805b0ffc80080ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff";
        const string BoxingHBMatrix = "7ffdffff7ffdffff7ffdffffffffffff7ffdffff7ffdffffbfffffff48feffff7ffeffffc8fdffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff";
        const string BoxingDMMatrix = "ffffffffffffffffffffffffb7ffffffffffffffffffffffb7fffffffffffffffffffffffffdffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff";
        const string BoxingAIMatrix = "ff0080ffff0080ffff4080ffffffffffff0080ffff0081ffffffffffffffffffc803a0ffc84580ffc842beffc80080ffc80080ffc80080ffcc0680ffc80080ffe80081ffc80480ffc80480ffc80480ffc804a0ffc805b0ffc80080ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff";
        const string BoxingMTMatrix = "ff0080ffff0080ffff4080ffffffffffff0080ffff0081ffffffffffffffffffc803a0ffc84580ffc842beffc80080ffc80080ffc80080ffcc0680ffc80080ffe80081ffc80480ffc80480ffc80480ffc804a0ffc805b0ffc80080ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff";

        const string TennisMatrix = "c80480ffc80080ffc80080ffffffffffc80080ffe80080ffffffffffffffffffc800a0ffc80480ffc902beffc80080ffc80080ffc80080ffc80080ffc80080ffc80080ffc80480ffc80480ffc80480ffc804a0ffc805b0ffc80080ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff";
        static ReadOnlyCollection<string> BoxingHBMatrixLayers { get; } = new ReadOnlyCollection<string>(new string[]
        {
            "Glove",
            "SandBag",
        });

        static ReadOnlyCollection<string> BoxingDMMatrixLayers { get; } = new ReadOnlyCollection<string>(new string[]
        {
            "Glove",
            "Dummy",
            "DummyBoundary",
        });

        static ReadOnlyCollection<string> BoxingAIMatrixLayers { get; } = new ReadOnlyCollection<string>(new string[]
        {
            "Player",
            "Enemy",
            "Ring",
            "Brando",
            "Crowd"
        });

        static ReadOnlyCollection<string> BoxingMTMatrixLayers { get; } = new ReadOnlyCollection<string>(new string[]
        {
        });

        static ReadOnlyCollection<string> billiardsMatrixLayers { get; } = new ReadOnlyCollection<string>(new string[]
        {
            "Cloth",
            "CueBall",
            "Ball",
            "Board",
            "Pocket",
            "NoVisible",
            "Ceiling",
            "CueControl",
            "RayReactor",
            "Character",
            "Permit"
        });

        static ReadOnlyCollection<string> basketballMatrixLayers { get; } = new ReadOnlyCollection<string>(new string[]
     {
            "Cloth",
            "Ball"
     });

        static ReadOnlyCollection<string> BoxingMatrixLayers { get; } = new ReadOnlyCollection<string>(new string[]
        {
            "Obstacle",
            "ObstacleDetect",
            "stencilRoot",
            "stencilReflection"
        });

        static string LittleEndian(string num)
        {            
            int number = Convert.ToInt32(num, 16);
            byte[] bytes = BitConverter.GetBytes(number);
            string retval = "";
            foreach (byte b in bytes)
                retval += b.ToString("X2");
            return retval;
        }

        private static bool GetDataFromMatrix(string targetMatrix, int layer1, int layer2)
        {
            var target = LittleEndian(targetMatrix.Substring(layer1 * 8, 8));
            int number = Convert.ToInt32(target, 16);

            return (number & 1 << layer2) == 0;
        }

        public static void SetIgnoreLayer(GameType type)
        {
            Debug.Log(type);
            string targetMatrix = string.Empty;
            switch (type)
            {
                case GameType.None: break;
                case GameType.Billiards: targetMatrix = billiardsMatrix; break;
                case GameType.Archery: targetMatrix = ArhceryMatrix; break;
                case GameType.Basketball: targetMatrix = BasketballMatrix; break;
                case GameType.Bowling: targetMatrix = BowlingMatrix; break;
                case GameType.Badminton: targetMatrix = BadmintonMatrix; break;
                case GameType.Lobby: targetMatrix = LobbyMatrix; break;
                case GameType.Test: targetMatrix = TestMatrix; break;
                case GameType.Golf: targetMatrix = GolfMatrix; break;
                case GameType.PingPong: targetMatrix = PingPongMatrix; break;
                case GameType.Dart: targetMatrix = DartMatrix; break;
                case GameType.Baseball: targetMatrix = BaseballMatrix; break;
                case GameType.Boxing: targetMatrix = BoxMatrix; break;
                case GameType.Boxing_HB: targetMatrix = BoxingHBMatrix; break;
                case GameType.Boxing_DM: targetMatrix = BoxingDMMatrix; break;
                case GameType.Boxing_AI: targetMatrix = BoxingAIMatrix; break;
                case GameType.Boxing_MT: targetMatrix = BoxingMTMatrix; break;
                case GameType.Tennis: targetMatrix = TennisMatrix; break;

                default: break;
            }

            for (int i = 0; i < 31; ++i)
            {
                for (int j = 0; j < 31; ++j)
                {
                    bool ignore = GetDataFromMatrix(targetMatrix, i, j);
                    Physics.IgnoreLayerCollision(i, j, ignore);
                }
            }
        }

        public static int NameToLayer(GameType type, string name)
        {            
            ReadOnlyCollection<string> targetCollection = null;
            switch (type)
            {
                case GameType.None: break;
                case GameType.Billiards: targetCollection = billiardsMatrixLayers; break;
                case GameType.Basketball: targetCollection = basketballMatrixLayers; break;
                case GameType.Boxing: targetCollection = BoxingMatrixLayers; break;
                case GameType.Boxing_HB: targetCollection = BoxingHBMatrixLayers; break;
                case GameType.Boxing_DM: targetCollection = BoxingDMMatrixLayers; break;
                case GameType.Boxing_AI: targetCollection = BoxingAIMatrixLayers; break;
                case GameType.Boxing_MT: targetCollection = BoxingMTMatrixLayers; break;
                default: break;
            }

            if (!targetCollection.Contains(name))
                return LayerMask.NameToLayer(name);

            return targetCollection.IndexOf(name) + 8;
        }

        public static string LayerToName(GameType type, int layer)
        {
            if (layer < 8)
            {
                //system Layer
                return LayerMask.LayerToName(layer);
            }

            ReadOnlyCollection<string> targetCollection = null;
            switch (type)
            {
                case GameType.None: break;
                case GameType.Billiards: targetCollection = billiardsMatrixLayers; break;
                case GameType.Basketball: targetCollection = basketballMatrixLayers; break;
                case GameType.Boxing: targetCollection = BoxingMatrixLayers; break;
                case GameType.Boxing_HB: targetCollection = BoxingHBMatrixLayers; break;
                case GameType.Boxing_DM: targetCollection = BoxingDMMatrixLayers; break;
                case GameType.Boxing_AI: targetCollection = BoxingAIMatrixLayers; break;
                case GameType.Boxing_MT: targetCollection = BoxingMTMatrixLayers; break;
                default: break;
            }

            return targetCollection[layer - 8];
        }
    }

}


//Data
/*

c800feff(def)    1100 1000 0000 0000 1111 1110 1111 1111
c800feff(tra)    1100 1000 0000 0000 1111 1110 1111 1111
c840feff(ign)    1100 1000 0100 0000 1111 1110 1111 1111
ffffffff(***)    1111 1111 1111 1111 1111 1111 1111 1111
c800feff(wat)    1100 1000 0000 0000 1111 1110 1111 1111
c800ffff(ui-)    1100 1000 0000 0000 1111 1110 1111 1111
ffffffff(***)    1111 1111 1111 1111 1111 1111 1111 1111
ffffffff(***)    1111 1111 1111 1111 1111 1111 1111 1111
c806feff(clo)    1100 1000 0000 0110 1111 1110 1111 1111
c85dfeff(cue)    1100 1000 0101 1101 1111 1110 1111 1111
c85ffeff(bal)    1100 1000 0101 1111 1111 1110 1111 1111
c806feff(boa)    1100 1000 0000 0110 1111 1110 1111 1111
c806feff(poc)    1100 1000 0000 0110 1111 1110 1111 1111
c800feff(nov)    1100 1000 0000 0000 1111 1110 1111 1111
cc06feff(cel)    1100 1100 0000 0110 1111 1110 1111 1111
c800feff(cue)    1100 1000 0000 0000 1111 1110 1111 1111
e800ffff(ray)    1110 1000 0000 0000 1111 1111 1111 1111
                 **uw *itd ccnp bbcc         r         
                 7654 3210 

                int 32 little-endian
              d  111111111111 1110 0000 0000 1100 1000
              t  111111111111 1110 0000 0000 1100 1000
              i  111111111111 1110 0100 0000 1100 1000
              *  111111111111 1111 1111 1111 1111 1111
              w  111111111111 1110 0000 0000 1100 1000
              u  111111111111 1111 0000 0000 1100 1000
              *  111111111111 1111 1111 1111 1111 1111
              *  111111111111 1111 1111 1111 1111 1111
              c  111111111111 1110 0000 0110 1100 1000
              c  111111111111 1110 0101 1101 1100 1000
              b  111111111111 1110 0101 1111 1100 1000
              b  111111111111 1110 0000 0110 1100 1000
              p  111111111111 1110 0000 0110 1100 1000
              n  111111111111 1110 0000 0000 1100 1000
              c  111111111111 1110 0000 0110 1100 1100
              c  111111111111 1110 0000 0000 1100 1000
              r  111111111111 1111 0000 0000 1110 1000
                                 r ccnp bbcc **uw *itd   

11111111111111111111111111111111
1111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111






                 1100 1000 0000 0010 1111111011111111  d
                 1100 1000 0000 0100 1111111011111111  t
                 1100 1000 0100 0000 1111111011111111
                 1111 1111 1111 1111 1111111111111111
                 1100 1000 0000 1000 1111111011111111   w
                 1100 1000 0000 0000 1111111111111111
                 1111 1111 1111 1111 1111111111111111
                 1111 1111 1111 1111 1111111111111111
                 1100 1000 0000 0110 1111111011111111
                 1100 1001 0101 1101 1111111011111111
                 1100 1010 0101 1111 1111111011111111
                 1101 1000 0000 0110 1111111011111111
                 1100 1000 0000 0110 1111111011111111
                 1100 1000 0000 0000 1111111011111111
                 1100 1100 0000 0110 1111111011111111
                 1100 1000 0000 0000 1111111011111111
                 1110 1000 0000 0000 1111111111111111
                                     
0	FFFE00C8	-130872
4	FFFE00C8	-130872
8	FFFE40C8	-114488
12	FFFFFFFF	-1
16	FFFE00C8	-130872
20	FFFF00C8	-65336
24	FFFFFFFF	-1
28	FFFFFFFF	-1
32	FFFE06C8	-129336
36	FFFE5DC8	-107064
40	FFFE5FC8	-106552
44	FFFE06C8	-129336
48	FFFE06C8	-129336
52	FFFE00C8	-130872
56	FFFE06CC	-129332
60	FFFE00C8	-130872
64	FFFF00E8	-65304
68	FFFFFFFF	-1
72	FFFFFFFF	-1
76	FFFFFFFF	-1
80	FFFFFFFF	-1
84	FFFFFFFF	-1
88	FFFFFFFF	-1
92	FFFFFFFF	-1
96	FFFFFFFF	-1
100	FFFFFFFF	-1
104	FFFFFFFF	-1
108	FFFFFFFF	-1
112	FFFFFFFF	-1
116	FFFFFFFF	-1
120	FFFFFFFF	-1
124	FFFFFFFF	-1





111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111                     
                                     
111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111


                     
  10  000 0000 0000 1 
d 00  000 0000 0000 0 
t 00  000 0000 0000 0 
i 00  000 0100 0000 0 
               
  00  000 0000 0000 0 
  00  000 0000 0000 1 
               
c 00  000 0000 0110 0 
c 00  000 0101 1101 0 
b 00  000 0101 1111 0 
b 00  000 0000 0110 0 
p 00  000 0000 0110 0 
n 00  000 0000 0000 0 
c 00  100 0000 0110 0 
c 00  000 0000 0000 0 
r 10  000 0000 0000 1 
      i   ccnp bbcc 
111111111111111111111111111111111111111
111111111111111111111111111111111111111
111111111111111111111111111111111111111
111111111111111111111111111111111111111
111111111111111111111111111111111111111
111111111111111111111111111111111111111
111111111111111111111111111111111111111
111111111111111111111111111111111111111
111111111111111111111111111111111111111
111111111111111111111111111111111111111
111111111111111111111111111111111111111
111111111111111111111111111111111111111
111111111111111111111111111111111111111
111111111111111111111111111111111111111
111111111111111111111111111111111111111
11111111111111111111


ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff





 
 */
