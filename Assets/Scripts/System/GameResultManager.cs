using UnityEngine;


namespace Game
{
    /// <summary>
    /// 게임 결과 데이터를 저장하는 전역 클래스입니다.
    /// 다른 클래스에서 쉽게 접근하여 결과를 확인할 수 있습니다.
    /// </summary>
    public static class GameResultManager
    {
        /// <summary>
        /// 최종 수심 결과 (m 단위)
        /// </summary>
        public static int FinalDepth { get; set; }

        // 추후 점수, 보상, 콜렉션 등 확장 가능
        // public static int TotalScore { get; set; }
        // public static int CaughtFishCount { get; set; }
    }
}

