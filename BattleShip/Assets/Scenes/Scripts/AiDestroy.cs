using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Notes legend: (IN = Improvement Note), (ON = Optimization Note)
public class AiDestroy : AiBaseState
{
    int Shipx, Shipy, Dir, mover;
    bool dirselected, IsUp, IsLeft, RandomSelect, IsVert;
    public override void EnterState(GameMaster gm)
    {
        Shipx = gm.AicurX;
        Shipy = gm.AicurY;
        RandomSelect = false;
        dirselected = false;
        mover = 1;
    }

    public override void UpdateState(GameMaster gm)
    {
        if(!gm.PlayerTurn)
        {
            /// when ship is initially hit Ai will check around hit in order to atain direction it will then follow said
            /// direction until it misses then return to search mode
            if (!dirselected)
                ShipHeading(gm);
            else
            {
                if (IsUp || IsLeft && mover == 1)
                    mover *= -1;
                else
                    mover = 1;
                if (IsVert)
                    if (gm.AiGuess(Shipx + mover, Shipy))
                        Shipx += mover;
                    else
                    {
                        gm.SwitchState(gm.AiSearchState);
                        gm.CheckScore();
                    }
                else
                {
                    if (gm.AiGuess(Shipx, Shipy + mover))
                        Shipy += mover;
                    else
                    {
                        gm.SwitchState(gm.AiSearchState);
                        gm.CheckScore();
                    }
                }
            }
        }
    }
    public void ShipHeading(GameMaster gm)
    {
        ///Randomly selects a direction at first then cycles through all possiblities

        /// IN : With current search pattern design I could just implement 2 directions, Down and Right.
        ///      I decided to go ahead and implement all four directions to make future intergrations easier
        if (!RandomSelect)
            Dir = Random.Range(1, 5);

        switch (Dir)
        {
            case 1:
                //up
                if (Shipx != 0)
                    if (gm.AiGuess(Shipx - 1, Shipy))
                    {
                        IsUp = true;
                        IsVert = true;
                        dirselected = true;
                    }
                break;
            case 2:
                //left
                if (Shipy != 0)
                    if (gm.AiGuess(Shipx, Shipy - 1))
                    {
                        IsLeft = true;
                        IsVert = false;
                        dirselected = true;
                    }
                break;
            case 3:
                //down
                if (Shipx != 9)
                    if (gm.AiGuess(Shipx + 1, Shipy))
                    {
                        IsUp = false;
                        IsVert = true;
                        dirselected = true;
                        Shipx += 1;
                    }
                break;
            case 4:
                //right
                if (Shipy != 9)
                    if (gm.AiGuess(Shipx, Shipy + 1))
                    {
                        IsLeft = false;
                        IsVert = false;
                        dirselected = true;
                        Shipy += 1;
                    }
                break;
            default:
                // if Dir does not equal any case sets it to 1
                Debug.Log(Dir);
                Dir = 0;
                break;
            
        }
        Dir++;

    }

}
