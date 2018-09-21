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
        //public static SolidBrush MasterBrush = new SolidBrush(Color.ForestGreen);
        
        //private GameObject[] gameObjects;
        private GridManager gridManager;
        private Dictionary<Direction, Corner> cornerDict;
        
        List<Vector2> playBoard;
        private Hex selectedHex;
        private Corner selectedCorner;

        private List<Player> playerList;

        private Queue<Stone> defRoundStoneQueue;
        private Queue<Stone> curRoundStoneQueue;
        private List<Stone> stoneList;




        public override void Init()
        {
            base.Init();
            variant = Constants.PLAYSCOPE_HEXS_PER_SIDE;
            gridManager = new GridManager();
            gridManager.Init();

            playerList = new List<Player>();
            var player1 = new Player(Color.White);
            player1.Init();
            var white = new Stone(player1.PresentedColor);
            //white.MoveTo( gridManager.GetHex(0, 0));
            player1.DefaultStone = white;

            var player2 = new Player(Color.Black);
            player2.Init();
            var black = new Stone(player2.PresentedColor);
            //black.MoveTo( gridManager.GetHex(0, 0));
            player2.DefaultStone = black;

            playerList.Add(player1);
            playerList.Add(player2);

            defRoundStoneQueue = new Queue<Stone>();
            curRoundStoneQueue = new Queue<Stone>();
            stoneList = new List<Stone>();

            ResetNewTurn();


            

            GeneratePlayBoard();
            
        }

        private void Restart()
        {
            isFirstTime = false;
            foreach (var player in playerList)
            {
                player.Reset();
            }
            gridManager.Reset();
            stoneList.Clear();
            ResetNewTurn();
        }

        private void ResetNewTurn()
        {
            foreach (var player in playerList)
            {
                defRoundStoneQueue.Enqueue(player.DefaultStone);
                curRoundStoneQueue.Enqueue(new Stone(player.DefaultStone.Color));
            }
        }

        private void GeneratePlayBoard()
        {
            var leftmostPoint = gridManager.GetHex(- gridManager.HexesPerSide,0);
            var setOfPoints = gridManager.GetAllPositions();
            // find convex hull
            //playBoard = JarvisConvexHull(leftmostPoint.Position, setOfPoints);

            cornerDict= gridManager.SetHexagonPlayingScope(variant);
            
        }

        private List<Vector2> JarvisConvexHull(Vector2 leftmostPoint,List<Vector2> setOfPoints)
        {
            List<Vector2> convexPointList = new List<Vector2>();
            int i = 0;
            Vector2 endPoint = new Vector2(0,0);
            var pointOnHull = leftmostPoint;
            do
            {
                convexPointList.Add(pointOnHull);
                endPoint = setOfPoints[0];

                for (int j = 1; j < setOfPoints.Count; j++)
                {
                    if (endPoint == pointOnHull
                        || IsLeftOfLine(setOfPoints[j], convexPointList[i], endPoint))
                    {
                        endPoint = setOfPoints[j];
                    }
                }

                i++;
                //setOfPoints.Remove(endPoint);
                pointOnHull = endPoint;
                

            } while (endPoint != convexPointList[0]);
            return convexPointList;
        }

        private bool IsLeftOfLine(Vector2 checkPoint, Vector2 convexPoint, Vector2 endPoint)
        {

            var expression = (endPoint.Y - convexPoint.Y) * (checkPoint.X - endPoint.X)
                - (checkPoint.Y - endPoint.Y) * (endPoint.X - convexPoint.X);

            if (expression >= 0)
            {
                if (expression == 0)
                {
                    return (endPoint - convexPoint).GetLength() < (checkPoint - convexPoint).GetLength();
                }
                else
                    return true;
            }
            else
                return false;
        }

        public override void Load()
        {
            base.Load();
        }

        public void HandleMouseInput(object sender, MouseEventArgs e)
        {

            var point = e.Location;
            if (e.Button == MouseButtons.Left)
            {
                debug = "raw pos =" + point.ToString() + "\n";
                var transformedPos = Utils.PixelToPosition(point, gridManager.Origin, gridManager.UnitRad);
                if (gridManager.IsPositionInPlayScope(transformedPos))
                {
                    selectedHex = gridManager.GetHex(transformedPos);
                    debug += "tfpos=" + selectedHex.Position.ToString();

                    BundleArgs eventArgs = new BundleArgs();
                    eventArgs.AddItem("hex", selectedHex);

                    playerList[currPlayerIndex].NextCommand(new EventHandler(EventType.MOVE_STONE, eventArgs));
                }
                else
                {
                    debug += transformedPos.ToString() + " is over game scope!";
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                Console.WriteLine(System.Diagnostics.Process.GetCurrentProcess().Threads.Count);
               
            }
        }

        float delayDuration = 0f;
        int variant;
        int nextDir = 0;
        int currPlayerIndex = 0;
        public override void Update(float dt)
        {
            if (CanPlayNewRound())
            {

                var cmd = playerList[currPlayerIndex].GetCommand();
                if (cmd == null)
                    return;
                ExecuteCommand(cmd);
                if(curRoundStoneQueue.Count <= 0)
                {
                    ResetNewTurn();
                    NextPlayer();
                }
            }
            else
            {
                status = "FINISH";
                if (!isFirstTime)
                {
                    isFirstTime = true;
                    gridManager.TagLabelAlgorithm(playerList);
                }

                //gridManager.ShowScore(playerList);

            }
           

        }


        private void ExecuteCommand(EventHandler cmd)
        {
            if(cmd.type == EventType.MOVE_STONE)
            {
                var bundle = cmd.eArgs as BundleArgs;
                var hex =  bundle.GetItemByKey<Hex>("hex");

                MoveStone(hex);
            }
        }

        private void NextPlayer()
        {
            currPlayerIndex = (currPlayerIndex + 1) % playerList.Count;
        }

        private bool CanPlayNewRound()
        {

            return gridManager.TotalPlaygroundHexes - stoneList.Count >= playerList.Count * playerList.Count;
        }


        private void MoveStone(Hex selectedHex)
        {
            
            if (curRoundStoneQueue.Count > 0
                && !selectedHex.IsHold)
            {
                Stone s = curRoundStoneQueue.Dequeue();
                s.MoveTo(selectedHex);
                stoneList.Add(s);


            }

        }

        internal void HandleInput(Queue<EventHandler> eventQueue)
        {
            //while(eventQueue.Count != 0)
            //{
            //    var inputEvent = eventQueue.Dequeue();
            //    if(inputEvent.type == EventType.MOUSE)
            //    {
            //        var mouseEArgument = (MouseEventArgs)inputEvent.eArgs;
                    
            //         HandleMouseInput(mouseEArgument);
            //    }
            //    else if(inputEvent.type == EventType.KEYBOARD)
            //    {
            //        var keyEArgument = (KeyEventArgs) inputEvent.eArgs;
            //        HandleKeyboardInput(keyEArgument);
                    
            //    }

            //}
        }

        public void HandleKeyboardInput(object sender, KeyEventArgs e)
        {
            var oldVariant = variant;
            if(e.KeyCode == Keys.W)
            {
                variant++;
                variant = Math.Min(variant, gridManager.HexesPerSide);
            }
            else if(e.KeyCode == Keys.S)
            {
                variant--;
                variant = Math.Max(variant,2);
            }

            if(oldVariant != variant)
            {

                cornerDict= gridManager.SetHexagonPlayingScope(variant);

                oldVariant = variant;
            }
        }

        Point p1 = new Point(0, 0);
        Point p2 = new Point(1, 1);
        private Hex lastSelectedHex;
        private Corner lastSelectedCorner;
        public override void Draw()
        {
            //hex.Draw();

            gridManager.Draw();

            foreach (var stone in stoneList)
            {
                stone.Draw();
            }

            gridManager.TestDrawLabels();

            foreach (var corner in cornerDict)
            {
                corner.Value.Draw();
            }

        }
        string debug;
        string edit;
        string status = "PLAYING";
        private bool isFirstTime = false;

        public override void OnGUI()
        {
            GUI.Begin();

            GUI.Label(new Rectangle(0, Height * 3 / 4,100,32), "post-edit:" + edit);
            GUI.Label(new Rectangle(0, Height * 1 / 4,100,100), "variant=" + variant);
            GUI.Label(new Rectangle(0, Height / 2, 100, 100), "debug  =" + debug);
            GUI.Label(new Rectangle(Width/2-128,16,128,16), "CURRENT PLAYER-"+currPlayerIndex);
            GUI.Label(new Rectangle(Width /2-64, 32, 128, 16), "STATUS-" + status);
            GUI.Label(new Rectangle(Width / 2 - 64, 48, 128, 16), "PLAYER1_SCORE:"+playerList[0].Score);
            GUI.Label(new Rectangle(Width / 2 - 64, 64, 128, 16), "PLAYER2_SCORE:" + playerList[1].Score);

            GUI.Button(new Rectangle(Width * 1 / 4, 64, 128, 16), 
                "RESET", () =>
                 {
                     this.Restart();
                 });

            //edit = GUI.TextBox(new Rectangle(0, 0,320,64), edit);

            GUI.End();
        }

    }
}
