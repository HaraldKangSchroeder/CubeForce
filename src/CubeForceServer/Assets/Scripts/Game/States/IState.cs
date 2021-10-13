public interface IState
{
    void enter();
    void execute();
    void exit();
    IState getNextState();

    StateID getStateID();

    bool isFinished();

    void finishState();

    void handlePlayerKey(int id,bool key_state, PlayerKeys player_key);
    void handlePlayerDirection(int id,float mouse_angle);

}

public enum StateID
{
    LOBBY,
    LEVEL_0_ENTER,
    LEVEL_0_EXECUTE,
    LEVEL_0_EXIT,
    LEVEL_0_TRANSITION,
    LEVEL_1_ENTER,
    LEVEL_1_EXECUTE,
    LEVEL_1_EXIT,
    LEVEL_1_TRANSITION,
    LEVEL_2_ENTER,
    LEVEL_2_EXECUTE,
    LEVEL_2_EXIT,
    LEVEL_2_TRANSITION,
    LEVEL_3_ENTER,
    LEVEL_3_EXECUTE,
    LEVEL_3_EXIT,
    LEVEL_3_TRANSITION,
    AWARD_CEREMONY
}


public enum PlayerKeys
{
    MOVE_UP,
    MOVE_LEFT,
    MOVE_DOWN,
    MOVE_RIGHT,
    SHOOT,
    SHIELD,
    DODGE
}