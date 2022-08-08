using UnityEngine;

/// Notes legend: (IN = Improvement Note), (ON = Optimization Note)
public abstract class AiBaseState
{
    /// The AiBaseClass for creating the Battleship finite state Ai
    public abstract void EnterState(GameMaster gm);

    public abstract void UpdateState(GameMaster gm);

}
