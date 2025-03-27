using UnityEngine;

namespace Game
{
    /// <summary>
    /// 현재 보유 골드를 관리하는 전역 클래스입니다.
    /// 골드 획득 및 조회 기능을 제공합니다.
    /// </summary>
    public static class GoldManager
    {
        #region Variables

        /// <summary>
        /// 현재 보유 골드
        /// </summary>
        private static int currentGold = GameConstants.DEFAULT_GOLD;

        #endregion

        #region Public Methods

        /// <summary>
        /// 골드를 추가합니다.
        /// </summary>
        public static void AddGold(int amount)
        {
            currentGold += amount;
            Log.System($"골드 +{amount} → 총 보유: {currentGold}");
        }

        /// <summary>
        /// 현재 골드를 반환합니다.
        /// </summary>
        public static int GetGold()
        {
            return currentGold;
        }

        #endregion
    }
}
