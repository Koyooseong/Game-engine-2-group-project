/// <summary>
/// 일시정지 및 재개 기능을 제공하는 모든 대상이 구현해야 하는 인터페이스입니다.
/// GamePauseManager가 일괄 제어합니다.
/// </summary>
public interface IGamePause
{
    /// <summary>
    /// 일시정지 시 호출됩니다.
    /// </summary>
    void Pause();

    /// <summary>
    /// 게임이 재개될 때 호출됩니다.
    /// </summary>
    void Resume();
}
