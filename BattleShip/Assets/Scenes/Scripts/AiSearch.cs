using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Notes legend: (IN = Improvement Note), (ON = Optimization Note)
public class AiSearch : AiBaseState
{
    int AiDiagonal = 0, AiSearchX = 0, AiSearchY = 0; 
    public override void EnterState(GameMaster gm)
    {
    }

    public override void UpdateState(GameMaster gm)
    {
        ///Ai searches Diagonally from (0, 0) for ship upon hit IsFound becomes true and State is changed

        /// IN : Would Like to implement a miss Counter, were after so many misses it could change its Search algorithm
        if (!gm.PlayerTurn)
        {
            if (gm.AiGuess(AiSearchX, AiSearchY))
            {
                gm.AicurX = AiSearchX;
                gm.AicurY = AiSearchY;
                Debug.Log("Hit");
                gm.SwitchState(gm.AiDestroyState);
            }
            if (AiDiagonal < 10)
            {
                if (AiSearchY == 0)
                {
                    AiDiagonal += 1;
                    AiSearchY = AiDiagonal;
                    AiSearchX = 0;
                }
                else
                {
                    AiSearchY -= 1;
                    AiSearchX += 1;
                }
            }
            else 
            {
                if (AiDiagonal == 9)
                    AiSearchX = 9;
                if (AiSearchX == 9)
                {
                    AiSearchX = AiDiagonal - 9;
                    AiDiagonal += 1;
                    AiSearchY = 9;
                }
                else
                {
                    AiSearchY -= 1;
                    AiSearchX += 1;
                }
            }
        }
    }
}
