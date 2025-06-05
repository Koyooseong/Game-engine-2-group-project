using UnityEngine;

/// <summary>
/// 씬 진입 시 자동으로 초기화될 수 있는 시스템 인터페이스입니다.
/// </summary>
public interface IInitializable
{
    /// <summary>
    /// 게임 시작 시 호출되는 초기화 함수입니다.
    /// </summary>
    void Init();
}
