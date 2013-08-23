using System;
using System.Collections.Generic;
using System.IO;

public class BaseGameAchievements<T> : DataObjects<T> where T : new() {
    private static T current;
    private static volatile BaseGameAchievements<T> instance;
    private static object syncRoot = new Object();

    public static string BASE_DATA_KEY = "game-achievement-data";

    public static T BaseCurrent {
        get {
            if (current == null) {
                lock (syncRoot) {
                    if (current == null)
                        current = new T();
                }
            }

            return current;
        }
        set {
            current = value;
        }
    }

    public static BaseGameAchievements<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseGameAchievements<T>(true);
                }
            }

            return instance;
        }
        set {
            instance = value;
        }
    }

    public static string ACHIEVE_FIRST_PLAY = "achieve_first_play";
    public static string ACHIEVE_HOUR_1 = "achieve_hour_1";
    public static string ACHIEVE_HOUR_2 = "achieve_hour_2";
    public static string ACHIEVE_HOUR_5 = "achieve_hour_5";
    public static string ACHIEVE_HOUR_10 = "achieve_hour_10";

    public static string ACHIEVE_WIN = "achieve_win";
    public static string ACHIEVE_LOSS = "achieve_loss";

    public static string ACHIEVE_WIN_10 = "achieve_win_10";
    public static string ACHIEVE_WIN_25 = "achieve_win_25";
    public static string ACHIEVE_WIN_50 = "achieve_win_50";
    public static string ACHIEVE_WIN_100 = "achieve_win_100";
    public static string ACHIEVE_WIN_250 = "achieve_win_250";
    public static string ACHIEVE_WIN_500 = "achieve_win_500";
    public static string ACHIEVE_WIN_1000 = "achieve_win_1000";

    public static string ACHIEVE_LOSS_10 = "achieve_loss_10";
    public static string ACHIEVE_LOSS_25 = "achieve_loss_25";
    public static string ACHIEVE_LOSS_50 = "achieve_loss_50";
    public static string ACHIEVE_LOSS_100 = "achieve_loss_100";

    public static string ACHIEVE_CONSECUTIVE_WINS_3 = "achieve_consecutivewins_3";
    public static string ACHIEVE_CONSECUTIVE_WINS_5 = "achieve_consecutivewins_5";
    public static string ACHIEVE_CONSECUTIVE_WINS_10 = "achieve_consecutivewins_10";

    public static string ACHIEVE_CONSECUTIVE_LOSSES_3 = "achieve_consecutivelosses_3";
    public static string ACHIEVE_CONSECUTIVE_LOSSES_5 = "achieve_consecutivelosses_5";
    public static string ACHIEVE_CONSECUTIVE_LOSSES_10 = "achieve_consecutivelosses_10";

    // UI achievements

    public static string ACHIEVE_UI_HELP = "achieve_ui_help";
    public static string ACHIEVE_UI_CREDITS = "achieve_ui_credits";

    public static string ACHIEVE_UI_RACE_QUIT = "achieve_ui_quit";	 // quitter
    public static string ACHIEVE_UI_RACE_RESTART = "achieve_ui_restart";	// mulligan

    public static string ACHIEVE_UI_CUSTOM_AUDIO = "achieve_ui_custom_audio";
    public static string ACHIEVE_UI_CUSTOM_ACTOR = "achieve_ui_custom_actor";

    // endless

    public static string ACHIEVE_ENDLESS_PLAY = "achieve_endless_play";
    public static string ACHIEVE_ENDLESS_WIN = "achieve_endless_win";
    public static string ACHIEVE_ENDLESS_WIN_5 = "achieve_endless_win_5";
    public static string ACHIEVE_ENDLESS_WIN_10 = "achieve_endless_win_10";

    // arcade

    public static string ACHIEVE_ARCADE_PLAY = "achieve_arcade_play";
    public static string ACHIEVE_ARCADE_WIN = "achieve_arcade_win";
    public static string ACHIEVE_ARCADE_WIN_50 = "achieve_arcade_win_50";

    // difficulty

    public static string ACHIEVE_DIFFICULTY_110 = "achieve_difficulty_110";
    public static string ACHIEVE_DIFFICULTY_125 = "achieve_difficulty_125";
    public static string ACHIEVE_DIFFICULTY_150 = "achieve_difficulty_150";

    // score

    public static string ACHIEVE_SCORE_10000 = "achieve_score_10000";
    public static string ACHIEVE_SCORE_100000 = "achieve_score_100000";
    public static string ACHIEVE_SCORE_1000000 = "achieve_score_1000000";

    public BaseGameAchievements() {
        Reset();
    }

    public BaseGameAchievements(bool loadData) {
        Reset();
        path = "data/achievement-data.json";
        pathKey = BASE_DATA_KEY;
        LoadData();
    }
}

public class BaseGameAchievement : AchievementMeta {

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.

    public BaseGameAchievement() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
    }
}