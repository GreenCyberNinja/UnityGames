using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// Notes legend: (IN = Improvement Note), (ON = Optimization Note)
public class GameMaster : MonoBehaviour
{
    /// The GameMaster acts as the central hub for all translation methods and grid implementation 

    public bool PlayerTurn, PCplacePiece, HasGameStarted = false, IsPcgridset = false;
    public int[,] PCGrid;
    int[,] AiGrid;
    public int AicurX, AicurY, GridSize = 10;
    bool AiTurn, IsEasy, IsFound, IsDestroyed;
    public GameObject PCPegGrid, WinPanel, WinText, ResetPanel;
    public Dictionary<int, int> AiBattleShips, PCBattleShips;
    public Dictionary<int, string> Confirmedshots;

    AiBaseState currentState;
    public AiSearch AiSearchState = new AiSearch();
    public AiDestroy AiDestroyState = new AiDestroy();


    private void Start()
    {
        InitGrids();
        AiBattleShips = new Dictionary<int, int>{{1, 2}, {2, 3}, {3, 3}, {4, 4}, {5, 5}};
        PCBattleShips = new Dictionary<int, int>{{1, 2}, {2, 3}, {3, 3}, {4, 4}, {5, 5}};
        Confirmedshots = new Dictionary<int, string>{};
        currentState = AiSearchState;
        PlayerTurn = true;
    }
    private void Update()
    {
        if (!PlayerTurn && !PCplacePiece)
        {
            currentState.UpdateState(this);
        }
    }
    public void CheckScore()
    {
        ///Checks both BattleShip Dictionarys to ensure all ships are destroyed
        bool AiWin = true, PCWin = true;

        foreach (KeyValuePair<int, int> ship in PCBattleShips)
        {
            if (ship.Value != 0)
            {
                AiWin = false;
                break;
            }
        }
        foreach (KeyValuePair<int, int> ship in AiBattleShips)
        {
            if (ship.Value != 0)
            {
                PCWin = false;
                break;
            }
        }
        if (AiWin)
            EndGame("Ai");
        if (PCWin)
            EndGame("You");
    }
    public void StartGame()
    {
        ///sets bools that control logic in CameraControl to true and populates AiGrid with Battleship
        PCplacePiece = true;
        HasGameStarted = true;
        SetAiPieces();
    }
    public void EndGame(string Winner)
    {
        ///turns on WinPanel and Displays the Winner either Ai or You
        WinPanel.SetActive(true);
        WinText.GetComponent<Text>().text = "Winner: " + Winner;
        HasGameStarted = false;
    }
    public void RestartGame()
    {
        ///Restarts Game
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void QuitGame()
    {
        ///Quits Game
        Application.Quit();
    }
    public void SwitchState(AiBaseState State)
    {
        /// allows AiStates to change state
        currentState = State;
        State.EnterState(this);
    }
    public bool AiGuess(int x, int y)
    {
        ///checks PCGrid if Hit or Miss and tells the appropiate peg

        string peg = ConvertToName(x, y);
        ///Checks if x/y is greater than 9 or less than 0 to prevent DestroyState from hitting null
        if (x > 9 || y > 9 || x < 0 || y < 0)
        {
            Debug.Log("<"+x.ToString()+y.ToString()+">");
            return false;
        }
        ///Checks if shot has already been made by checking Confirmedshots if already selected
        if (!Confirmedshots.ContainsValue(peg))
        {
            Confirmedshots.Add(Confirmedshots.Count + 1, peg);
            if(PCGrid[x, y] != 0)
            {
                PCPegGrid.transform.Find(peg).GetComponent<PegControl>().Hit();
                PlayerTurn = true;
                return true;
            }
            PCPegGrid.transform.Find(peg).GetComponent<PegControl>().Miss();
            PlayerTurn = true;
            return false;
        }
        return false;
    }
    public void CheckPcGrid()
    {
        ///checks to make sure all ships are placed correctly
        /// ON : Not Currently Working
        IDictionary<int, int> PCcheck = new Dictionary<int, int>{{1, 2}, {2, 3}, {3, 3}, {4, 4}, {5, 5}};
        
        for (int x = 0; x != GridSize; x++)
        {
            for (int y = 0; y != GridSize; y++)
            {
                if (PCGrid[x, y] != 0)
                {
                    if (PCcheck.ContainsKey(PCGrid[x, y]))
                    {
                        PCcheck[PCGrid[x, y]] -= 1;
                    }
                }
            
            }
        }
        foreach (KeyValuePair<int, int> ship in PCcheck)
        {
            if (ship.Value != 0)
            {
                Debug.Log(ship.Value);
                ResetPanel.SetActive(true);
            }
        }
        Debug.Log("Done");
        IsPcgridset = true;

    }
    public void SetAiPieces()
    {
        //Sets game pieces in the AiGrid matrix the ships are labeled based on how many shots required to sink i.e.(2,3,3,4,5)
        int Shipnum = 5;
        int ShipsPlaced = 0;
        int ShipPart = 5;
        while (ShipsPlaced != 5)
        {
            int x = Random.Range(0, GridSize);
            int y = Random.Range(0, GridSize);
            int dir = Random.Range(0, 2);
            if (Shipnum == 2)
                ShipPart = 3;
            else if (Shipnum == 1)
                ShipPart = 2;
            else
                ShipPart = Shipnum;

            if (dir == 0)
            {
                //place piece vertically
                    if (x - ShipPart < 0)
                        x += ShipPart;
                if (IsColumnClear(x, y))
                {

                    while (ShipPart != 0)
                    {
                        AiGrid[x - ShipPart, y] = Shipnum;
                        ShipPart--;
                    }
                    ShipsPlaced++;
                    Shipnum--;
                }
            }
            else
            {
                //place piece horizontally
                if (IsRowClear(x, y))
                {
                    if (y - ShipPart < 0)
                        y += ShipPart;
                    while (ShipPart != 0)
                    {
                        AiGrid[x, y - ShipPart] = Shipnum;
                        ShipPart--;
                    }
                    ShipsPlaced++;
                    Shipnum--;
                }
            }
            string msg = "" + x.ToString() + y.ToString();

        }
    }
    public bool IsRowClear(int column, int row)
    {
        ///checks if the row is clear of pieces 
        for (int i = 0; i != GridSize; i++)
        {
            if (AiGrid[column, i] != 0)
            {
                return false;
            }
        }
        return true;
    }
    public bool IsColumnClear(int column, int row)
    {
        ///checks if the column is clear of pieces
        for (int i = 0; i != GridSize; i++)
        {
            if (AiGrid[i, row] != 0)
            {
                return false;
            }
        }
        return true;
    }
    public void AiGridcheck()
    {
        ///prints the Aigrid in debug.log for testing purposes
        string msg = "";

        for (int x = 0; x != GridSize; x++)
        {
            for (int y = 0; y != GridSize; y++)
            {
                msg = msg + AiGrid[x,y].ToString() + ", ";
            }
            Debug.Log(msg);
            msg = "";
        }
    }
    public void PcGridcheck()
    {
        ///prints the PCGrid in debug.log for testing purposes
        string msg = "";

        for (int x = 0; x != GridSize; x++)
        {
            for (int y = 0; y != GridSize; y++)
            {
                msg = msg + PCGrid[x,y].ToString() + ", ";
            }
            Debug.Log(msg);
            msg = "";
        }
    }
    public void InitGrids()
    {
        ///initlizes the grid and fills it with 0s
        AiGrid = new int[GridSize, GridSize];
        PCGrid = new int[GridSize, GridSize];
        for (int x = 0; x != GridSize; x++)
        {
            for (int y = 0; y != GridSize; y++)
            {
                AiGrid[x, y] = 0;
                PCGrid[x, y] = 0;
            }
        }

    }
    public bool PCMakeGuess(int x, int y)
    {
        ///when a peg is selected on Guess grid its coordinates are passed it then checks Aigrid if hit(true) or miss(false)
        if (AiGrid[x, y] != 0)
        {
            int boat = AiGrid[x, y];
            AiBattleShips[boat] -= 1;
            AiGrid[x, y] = 0;
            return true;
        }
        return false;
    }
    public void ConvertToGrid(string name, out int x, out int y)
    {
        ///converts name of Peg into grid coordinates 
        char[] delim = {' ', '(', ')'};
        string[] words = name.Split(delim);
        int num = int.Parse(words[2]);
        if (num < 10)
        {
            x = 0;
            y = num;
        }
        else
        {
            x = num / 10;
            y = num % 10;
        }
    }
    public string ConvertToName(int x, int y)
    {
        ///converts coordinates into the Name of Peg
        if (x != 0)
        {
            int num = x * 10 + y;
            return("Peg (" + num.ToString() + ")");
        }
        return("Peg (" + y.ToString() + ")");

    }

}
