using Omega.Ai;
using Omega.Test;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Omega
{
    class GameScreen : Scene
    {
    //    //public static SolidBrush MasterBrush = new SolidBrush(Color.ForestGreen);

    //    //private GameObject[] gameObjects;

    //    private GameState gameState;
    //    private GridMan gridMan;

    //    //private GridManager gridManager;
    //    //private Dictionary<Direction, Corner> cornerDict;
        
    //    private Hex selectedHex;
    //    private Corner selectedCorner;

    //    private List<Player> playerList;

    //    private ManualPlayer huPlayer;

    //    private Queue<Stone> defRoundStoneQueue;
    //    private Queue<Stone> curRoundStoneQueue;
    //    private List<Stone> stoneList;

    //    Command inputMoveStoneCommand;
    //    private Form form;

    //    public GameScreen(Form1 f)
    //    {
    //        this.form = f;
    //    }


    //    public override void Init()
    //    {
    //        base.Init();
    //        variant = Constants.PLAYSCOPE_HEXS_PER_SIDE;

    //        gameState = new GameState(variant, Constants.MIN_HEXS_PER_SIDE);

    //        playerList = new List<Player>();

    //        huPlayer = new ManualPlayer(1);
    //        huPlayer.Init();

    //        var player1 = new Player(1);
    //        player1.Init();
    //        var white = new Stone(player1.PresentedColor);
    //        //white.MoveTo( gridManager.GetHex(0, 0));
    //        player1.DefaultStone = white;

    //        var player2 = new RandomAIPlayer(gameState, 2); //new Player(2);
    //        player2.Init();
    //        var black = new Stone(player2.PresentedColor);
    //        //black.MoveTo( gridManager.GetHex(0, 0));
    //        player2.DefaultStone = black;

    //        inputMoveStoneCommand = new Command(CommandType.MoveStone,0, new Vector2(0, 0));

    //        playerList.Add(player1);
    //        playerList.Add(player2);

    //        defRoundStoneQueue = new Queue<Stone>();
    //        curRoundStoneQueue = new Queue<Stone>();
    //        stoneList = new List<Stone>();

    //        gridMan = new GridMan(gameState,
    //            Constants.GRID_ORIGIN,
    //            Constants.GRID_HEX_RADIUS);


    //        gameState.AddPlayer(player1);
    //        gameState.AddPlayer(player2);
            
    //        //gridManager = new GridManager();
    //        //gridManager.Init();

    //        //foreach (var player in playerList)
    //        //      defRoundStoneQueue.Enqueue(player.DefaultStone);

    //        GeneratePlayBoard();
    //        //ResetNewTurn();
    //        gameState.ResetBoard();
    //        gameState.ResetNewTurn();

            

            
    //    }

    //    private void Restart()
    //    {
    //        isFirstTime = false;
    //        gameState.ResetBoard();
    //        foreach (var player in playerList)
    //        {
    //            player.Reset();
    //        }
    //        //gridManager.Reset();
    //        stoneList.Clear();
    //        //ResetNewTurn();
    //    }

    //    private void ResetNewTurn()
    //    {
    //        curRoundStoneQueue.Clear();
    //        foreach (var player in playerList)
    //        {
    //            curRoundStoneQueue.Enqueue(new Stone(player.DefaultStone.Color));
    //        }


    //    }

    //    private void GeneratePlayBoard()
    //    {
    //        //var leftmostPoint = gridManager.GetHex(- gridManager.HexesPerSide,0);
    //        //var setOfPoints = gridManager.GetAllPositions();
    //        //// find convex hull
    //        ////playBoard = JarvisConvexHull(leftmostPoint.Position, setOfPoints);

    //        //cornerDict= gridManager.SetHexagonPlayingScope(variant);
    //        gridMan.Init();
    //    }

    //    private List<Vector2> JarvisConvexHull(Vector2 leftmostPoint,List<Vector2> setOfPoints)
    //    {
    //        List<Vector2> convexPointList = new List<Vector2>();
    //        int i = 0;
    //        Vector2 endPoint = new Vector2(0,0);
    //        var pointOnHull = leftmostPoint;
    //        do
    //        {
    //            convexPointList.Add(pointOnHull);
    //            endPoint = setOfPoints[0];

    //            for (int j = 1; j < setOfPoints.Count; j++)
    //            {
    //                if (endPoint == pointOnHull
    //                    || IsLeftOfLine(setOfPoints[j], convexPointList[i], endPoint))
    //                {
    //                    endPoint = setOfPoints[j];
    //                }
    //            }

    //            i++;
    //            //setOfPoints.Remove(endPoint);
    //            pointOnHull = endPoint;
                

    //        } while (endPoint != convexPointList[0]);
    //        return convexPointList;
    //    }

    //    private bool IsLeftOfLine(Vector2 checkPoint, Vector2 convexPoint, Vector2 endPoint)
    //    {

    //        var expression = (endPoint.Y - convexPoint.Y) * (checkPoint.X - endPoint.X)
    //            - (checkPoint.Y - endPoint.Y) * (endPoint.X - convexPoint.X);

    //        if (expression >= 0)
    //        {
    //            if (expression == 0)
    //            {
    //                return (endPoint - convexPoint).GetLength() < (checkPoint - convexPoint).GetLength();
    //            }
    //            else
    //                return true;
    //        }
    //        else
    //            return false;
    //    }

    //    public override void Load()
    //    {
    //        base.Load();
    //    }


    //    public void HandleMouseInput(object sender, MouseEventArgs e)
    //    {
            
    //        var point = e.Location;
            
    //        if (e.Button == MouseButtons.Left)
    //        {
    //            debug = "raw pos =" + point.ToString() + "\n";
    //            var transformedPos = Utils.PixelToPosition(point,Constants.GRID_ORIGIN,Constants.GRID_HEX_RADIUS);

    //            //if (gridManager.IsPositionInPlayScope(transformedPos))
    //            if (!gameState.CheckGameOver()&&gridMan.IsPositionInPlayScope(transformedPos))
    //            {
    //                var presentIdOfStone = gameState.GetNextStone();
    //                //playerList[0].NextCommand(new Command(CommandType.MoveStone, presentIdOfStone, transformedPos));
    //                debug += "tfpos=" + transformedPos.ToString();
                    
                    

    //                //if (roundQueue.Count <= 0)
    //                //{
    //                //    ResetNewTurn();
    //                //    NextPlayer();
    //                //}


    //                //    selectedHex = gridManager.GetHex(transformedPos);
    //                //    BundleArgs eventArgs = new BundleArgs();
    //                //    eventArgs.AddItem("hex", selectedHex);
    //                //    playerList[currPlayerIndex].NextCommand(new EventHandler(EventType.MOVE_STONE, eventArgs));
    //            }
    //            else
    //            {
    //                debug += transformedPos.ToString() + " is over game scope!";

    //            }
    //        }
    //        else if (e.Button == MouseButtons.Right)
    //        {
    //            Console.WriteLine(System.Diagnostics.Process.GetCurrentProcess().Threads.Count);
               
    //        }
    //    }

    //    float delayDuration = 0f;
    //    int variant;
    //    int nextDir = 0;
    //    EventHandler lastCommand;


    //    public override void Update(float dt)
    //    {
             
    //        if (!gameState.CheckGameOver())
    //        {
    //            var command1 = playerList[0].GetCommand();
    //            var command2 = playerList[1].GetCommand();

    //            gameState.AddCommand(command1);
    //            gameState.AddCommand(command2);

    //            gameState.Cycle(dt);
    //        }
    //        else
    //        {
    //            if (!isFirstTime)
    //            {
    //                isFirstTime = true;
    //                gameState.UpdateScores();

    //                //re-learning if need
    //                foreach (var player in playerList)
    //                {
    //                    player.GameOver(gameState.GetWinner());
    //                }
    //            }
    //        }

    //        //if (CanPlayNewRound())
    //        //{
    //        //    var cmd = playerList[currPlayerIndex].GetHumanCommand();
    //        //    if (cmd == null)
    //        //        return;
    //        //    lastCommand = cmd.Clone();
    //        //    ExecuteCommand(cmd);
    //        //    if(curRoundStoneQueue.Count <= 0)
    //        //    {
    //        //        ResetNewTurn();
    //        //        NextPlayer();
    //        //    }
    //        //}
    //        //else
    //        //{
    //        //    status = "FINISH";
    //        //    if (!isFirstTime)
    //        //    {
    //        //        isFirstTime = true;
    //        //        //gridManager.TagLabelAlgorithm(playerList);
    //        //    }

    //        //    //gridManager.ShowScore(playerList);

    //        //}
           

    //    }
    //    public void OnBtnUndoClicked()
    //    {
    //        //U clicked
            
    //        var undoState = gameState.Undo();
    //        if(undoState != null)
    //        {
    //            gameState = undoState;
    //            gridMan.SetGameState(gameState);
                
    //            if (isFirstTime)
    //            {
    //                isFirstTime = false;
    //                foreach (var player in playerList)
    //                {
    //                    player.Score = 0;
    //                }
    //            }
    //        }
    //    }
    //    public void Undo()
    //    {
    //        if (lastCommand != null)
    //        {
    //            //PROBLEMMMMM hex still hold holder
    //            stoneList.RemoveAt(stoneList.Count - 1);

    //            //if (curRoundStoneQueue.Count == playerList.Count)
    //            //{
    //            //    NextPlayer();
    //            //    curRoundStoneQueue.Dequeue();
    //            //}
    //            //else
    //            //{
    //            //    ResetNewTurn();
    //            //}

    //            if (lastCommand.type == EventType.MOVE_STONE)
    //            {
    //                var bundle = lastCommand.eArgs as BundleArgs;
    //                var hex = bundle.GetItemByKey<Hex>("hex");

    //                hex.Holder = null;
    //            }
    //            lastCommand = null;
    //        }
    //    }


    //    private void ExecuteCommand(EventHandler cmd)
    //    {
    //        if(cmd.type == EventType.MOVE_STONE)
    //        {
    //            var bundle = cmd.eArgs as BundleArgs;
    //            var hex =  bundle.GetItemByKey<Hex>("hex");

    //            MoveStone(hex);
    //        }
    //    }
       
    //    private void NextPlayer()
    //    {
    //        //currPlayerIndex = (currPlayerIndex + 1) % playerList.Count;
    //    }

    //    private bool CanPlayNewRound()
    //    {
    //        return false;
    //        //return gridManager.TotalPlaygroundHexes - stoneList.Count >= playerList.Count * playerList.Count;
    //    }
        
    //    private void MoveStone(Hex selectedHex)
    //    {
            
    //        if (curRoundStoneQueue.Count > 0
    //            && !selectedHex.IsHold)
    //        {
    //            Stone s = curRoundStoneQueue.Dequeue();
    //            s.MoveTo(selectedHex);
    //            stoneList.Add(s);
    //        }

    //    }
        
        

    //    int oldVariant=0;
    //    public void HandleKeyboardInput(object sender, KeyEventArgs e)
    //    {
    //        if (e.KeyCode == Keys.W)
    //        {
    //            variant++;
    //            variant = Math.Min(variant, gameState.HexesPerSide);
    //        }
    //        else if (e.KeyCode == Keys.S)
    //        {
    //            variant--;
    //            variant = Math.Max(variant,2);
    //        }
    //        else if(e.KeyCode == Keys.U)
    //        {
    //            OnBtnUndoClicked();
    //        }

    //        if(oldVariant != variant)
    //        {
    //            //cornerDict= gridManager.SetHexagonPlayingScope(variant);
    //            oldVariant = variant;
    //            gameState.UpdatePlayBoard(oldVariant - 1);
    //            gridMan.SetupConerns();
    //        }
    //    }

    //    Point p1 = new Point(0, 0);
    //    Point p2 = new Point(1, 1);
    //    private Hex lastSelectedHex;
    //    private Corner lastSelectedCorner;
    //    public override void Draw()
    //    {
    //        //hex.Draw();

    //        gridMan.Draw();

    //        //gridManager.Draw();

    //        //foreach (var stone in stoneList)
    //        //{
    //        //    stone.Draw();
    //        //}

    //        //gridManager.TestDrawLabels();

    //        //foreach (var corner in cornerDict)
    //        //{
    //        //    corner.Value.Draw();
    //        //}

    //    }

        
    //    string debug;
    //    string edit;
    //    string status = "PLAYING";
    //    private bool isFirstTime = false;
    //    Rectangle rect;
    //    public override void OnGUI()
    //    {
    //        GUI.Begin();
    //        GUI.Label(new Rectangle(0, Height * 3 / 4, 100, 32), "post-edit:" + edit);
    //        GUI.Label(new Rectangle(0, Height * 1 / 4, 100, 100), "variant=" + variant);
    //        GUI.Label(new Rectangle(0, Height / 2, 100, 100), "debug  =" + debug);
    //        GUI.Label(new Rectangle(Width / 2 - 64, 16, 128, 16), "CURRENT PLAYER-" + gameState.CurrentPlayerId);
    //        GUI.Label(new Rectangle(Width / 2 - 64, 48, 128, 16), "PLAYER1_SCORE:" + playerList[0].Score);
    //        GUI.Label(new Rectangle(Width / 2 - 64, 64, 128, 16), "PLAYER2_SCORE:" + playerList[1].Score);

    //        //GUI.Button(new Rectangle(0, 0, 100, 64),
    //        //    "RESET", () =>
    //        //    {
    //        //        this.Restart();

    //        //    });
    //        //GUI.Button(new Rectangle(0, 64, 100, 64),
    //        //    "UNDO", () =>
    //        //    {
    //        //        this.Undo();

    //        //    });
    //        //////edit = GUI.TextBox(new Rectangle(0, 0,320,64), edit);

    //        GUI.End();
    //    }
    }
}
