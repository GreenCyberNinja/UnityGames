using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Notes legend: (IN = Improvement Note), (ON = Optimization Note)
public class PegControl : MonoBehaviour
{
    /// Peg Control handles Hits, Misses, and Piecesplaced Interactions
    public Renderer rend;
    public GameMaster Gm;
    public Material miss, hit;
    public Collider mycol;
    public int pegx, pegy;
    bool IsSelected;


    void Start()
    {
        rend = GetComponent<Renderer>();
        Gm = GameObject.Find("Guess grid").GetComponent<GameMaster>();
        mycol = GetComponent<Collider>();
        Gm.ConvertToGrid(this.name, out pegx, out pegy);
    }
    private void OnMouseEnter()
    {
        if (transform.parent.tag == "GuessGridPegs" && Gm.PlayerTurn)
            rend.enabled = true;
    }
    private void OnMouseExit()
    {
        if (!IsSelected)
            rend.enabled = false;    
    }
    public void PiecePlaced(int Shipnum)
    {
        Gm.PCGrid[pegx, pegy] = Shipnum;
    }
    public void GuessSelected()
    {
        ///Upon being selected by Player, Peg is checked against AiGrid and Changes to Hit or Missed bassed on check
        if ((Gm.PCMakeGuess(pegx, pegy)))
        {
            Hit();
            Gm.CheckScore();
        }
        else
        {
            Miss();
        }
        Gm.PlayerTurn = false;
    }
    public void Hit()
    {
        /// tells the Peg it is a hit
        IsSelected = true;
        mycol.enabled = false;
        rend.material = hit;
        rend.enabled = true;        
    }
    public void Miss()
    {
        /// tells the Peg it is a miss
        IsSelected = true;
        mycol.enabled = false;
        rend.material = miss;
        rend.enabled = true;  
    }
}
