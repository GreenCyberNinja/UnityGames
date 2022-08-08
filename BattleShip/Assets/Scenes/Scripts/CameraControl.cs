using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Notes legend: (IN = Improvement Note), (ON = Optimization Note)
public class CameraControl : MonoBehaviour
{
    ///Camera Control Handles both Camera and Player interactions
    public Transform GG, PG;
    public Camera thisCam;
    public GameMaster Gm;
    public List<GameObject> Bss = new List<GameObject>();
    public int x = 0, camoffset = 10;
    bool lookingAtPg, lookingAtGG;

    public void LookAtGuessGrid()
    {
        ///turns the camera to look at the guess grid
        lookingAtPg = false;
        transform.position = new Vector3(0,camoffset,-camoffset);
        transform.LookAt(GG);
        lookingAtGG = true;
    }
    public void LookAtPlayerGrid()
    {
        ///turns the camera to look at player grid
        lookingAtGG = false;
        transform.position = new Vector3(0,camoffset,0);
        transform.LookAt(PG);
        lookingAtPg = true;
    }

    private void Update()
    {
        ///ON : most of this could be put into a coroutine, Method call, or even State machine
        ///IN : method to not allow a battleship to be placed over another battleShip current Idea of checking after player places
        /// all battleships is not working and needs to be replaced

        ///allows the player to select peg on guess grid
        if (Gm.PlayerTurn && !Gm.PCplacePiece && Gm.HasGameStarted)
        {
            if (lookingAtGG)
            {
                //if (!Gm.IsPcgridset)
                //    Gm.CheckPcGrid();
                Ray ray = thisCam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
                {
                    if (hit.collider.tag == "GuessGridPegs")
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            hit.collider.GetComponent<PegControl>().GuessSelected();
                        }
                    }
                }
            }
            else
                LookAtGuessGrid();
        }

        ///allows player to set pieces on PCgrid
        if (Gm.PCplacePiece)
        {
            if (x < 5)
            {
                if (!lookingAtPg)
                    LookAtPlayerGrid();
                Ray ray = thisCam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
                {

                    if (hit.collider.tag == "PlayerGridPlane" || hit.collider.tag == "PlayerGridPegs")
                    {
                        Bss[x].transform.position = hit.point;

                    }
                    if (Input.GetMouseButtonDown(0))
                    {
                        Bss[x].GetComponent<SnapToGrid>().Snap();
                        x++;
                    }
                }
            }
            else
            {
                Gm.PCplacePiece = false;
            }
            if (Input.GetMouseButtonDown(1))
            {
                Bss[x].GetComponent<SnapToGrid>().Rotate();
            }

        }
    }
}
