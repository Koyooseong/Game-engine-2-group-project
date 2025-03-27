using System.Collections.Generic;
using UnityEngine;


namespace Game
{
    /// <summary>
    /// 게임 결과 데이터를 저장하는 전역 클래스입니다.
    /// 다른 클래스에서 쉽게 접근하여 결과를 확인할 수 있습니다.
    /// </summary>
    public static class GameResultManager
    {
        #region Variables
        /// <summary>
        /// 최종 수심 결과 (m 단위)
        /// </summary>
        public static int FinalDepth { get; set; }
        /// <summary>물고기 타입별 잡힌 수</summary>
        private static Dictionary<FishType, int> caughtFishDict = new();

        #endregion



        #region Public Methods

        /// <summary>
        /// 물고기 수량을 증가 또는 감소시킵니다. (음수 허용)
        /// </summary>
        public static void AddFish(FishType type, int amount)
        {
            if (!caughtFishDict.ContainsKey(type))
                caughtFishDict[type] = 0;

            caughtFishDict[type] += amount;

            if (caughtFishDict[type] < 0)
                caughtFishDict[type] = 0;

            Log.System($"{type} 수량 변경: {caughtFishDict[type]}");
        }

        /// <summary>
        /// 특정 물고기의 누적 획득 수를 반환합니다.
        /// </summary>
        public static int GetFishCount(FishType type)
        {
            return caughtFishDict.TryGetValue(type, out int count) ? count : 0;
        }
        #endregion
    }
}

