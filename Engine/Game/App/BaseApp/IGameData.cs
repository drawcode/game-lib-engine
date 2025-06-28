
public interface IGameData
{
    int currentAIDifficulty { get; }

    void ChangeRaceMode(GameRaceMode changeToMode);
    GameRaceMode GetCurrentRaceMode();
    double GetDefaultDifficultyValueByTypeAndRound(GameDifficultyType newDifficultyType, GameRoundType newRoundType);
    bool IsRaceModeArcade();
    bool IsRaceModeEndless();
    bool IsRaceModeSeries();
    void Reset();
    void SetDifficultyType(GameDifficultyType newDifficultyType);
    void SetDifficultyValueByTypeAndRound(GameDifficultyType newDifficultyType, GameRoundType newRoundType);
    void SetRoundType(GameRoundType newRoundType);
}