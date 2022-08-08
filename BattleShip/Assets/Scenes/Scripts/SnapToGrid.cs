using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Notes legend: (IN = Improvement Note), (ON = Optimization Note)
public class SnapToGrid : MonoBehaviour
{
    ///SnapToGrid attempts to aid Player Interactions and limit how players place pieces on the board
    float offset = 0.1f;
    Vector3 pegpos;
    string CurPeg;
    int x, y, Shipnum;
    public int shipspaces;
    GameMaster Gm;
    Collider thisCol, ShipCol;
    bool IsUp = false, IsLeft = false, IsRotated = false;
    
    private void Start()
    {
        Gm = GameObject.Find("Guess grid").GetComponent<GameMaster>();
        thisCol = GetComponent<Collider>();
        ShipCol = transform.GetChild(0).GetComponent<Collider>();
        string[] words = name.Split(' ');
        int Shipnum = int.Parse(words[1]);
        transform.GetChild(0).GetComponent<SetPcGrid>().ShipNum = Shipnum;
    }
    private void OnTriggerEnter(Collider other)
    {
        /// ON: All of this logic could be put in a Method call or Coroutine
        if (other.tag == "PlayerGridPegs")
        {
            other.GetComponent<Renderer>().enabled = true;
            pegpos = other.bounds.center - new Vector3(0, offset, 0);
            x = other.GetComponent<PegControl>().pegx;
            y = other.GetComponent<PegControl>().pegy;
            ///ON: should move bellow string parse into method and/or store shipspace as class variable
            string[] words = this.name.Split(' ');
            int shipspaces = int.Parse(words[1]);
            ///IN: move bellow if/else into seperate method
            ///Below if/else is logic that will rotate piece so it is always inside grid takes ship length and direction 
            if (!IsRotated)
            {
                if ((x - shipspaces) < 0)
                {
                        if (IsUp)
                        {
                            transform.eulerAngles =  new Vector3(0,0,0);
                            IsUp = false;
                        }
                }
                if ((x + shipspaces) > 9)
                {
                    if (!IsUp)
                    {
                        transform.eulerAngles =  new Vector3(0,180,0);
                            IsUp = true;
                    }
                }
            }
            else
            {
                if (y - shipspaces < 0)
                {
                        if (IsLeft)
                        {
                            transform.eulerAngles =  new Vector3(0,-90,0);
                            IsLeft = false;
                        }
                }
                if (y + shipspaces > 9)
                {
                        if (!IsLeft)
                        {
                            transform.eulerAngles =  new Vector3(0,90,0);
                            IsLeft = true;
                        }
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        other.GetComponent<Renderer>().enabled = false;
    }
    public void Snap()
    {
        ///method snaps collider to center of Peg collider(saved in pegpos)
        transform.position = pegpos;
        thisCol.enabled = false;
        ShipCol.enabled = true;
    }
    public void ResetPiece()
    {
        ///for future implementation if player places piece incorrectly
        thisCol.enabled = true;
        ShipCol.enabled = false;
    }
    public void Rotate()
    {
        ///upon call rotates -90 degrees if it is already rotated rotates 90 to original orientation 
        if (IsRotated)
        {
            transform.eulerAngles =  new Vector3(0,0,0);
            IsRotated = false;
            IsUp = false;
        }
        else
        {
            transform.eulerAngles =  new Vector3(0,90,0);
            IsRotated = true;
            IsLeft = true;
        }
    }
}
