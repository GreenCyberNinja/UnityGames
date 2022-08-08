using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Notes legend: (IN = Improvement Note), (ON = Optimization Note)
public class SetPcGrid : MonoBehaviour
{
    ///used to select all pegs in length of a ship and pass the ships number
    public int ShipNum;

    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<PegControl>().PiecePlaced(ShipNum);
        other.enabled = false;
    }
}