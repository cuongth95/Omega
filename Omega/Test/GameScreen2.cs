using Omega.Ai;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Omega.Test
{
    public class GameScreen2:Scene
    {
        private GameState gameState;
        private GridMan gridMan;
        private ManualPlayer huPlayer;
        private ABNMAIPlayer testAIPlayer;
        private ABNMAIPlayer aiPlayer;
        private Form mainForm;
        private int variant;
        private int oldVariant = 0;
        private string debug;
        private bool flagGameOver;
        private SpriteBatch sp;
        private Rectangle decoRect;
        private bool debugAIPerTurn;
        private bool debugTestAIPerTurn;

        private Color currentColor;

        public GameScreen2(Form1 mainForm)
        {
            this.mainForm = mainForm;
            sp = SpriteBatch.GetInstance();
            variant = Constants.PLAYSCOPE_HEXS_PER_SIDE;

            gameState = new GameState(variant, Constants.MIN_HEXS_PER_SIDE);

            gridMan = new GridMan(gameState,
                Constants.GRID_ORIGIN,
                Constants.GRID_HEX_RADIUS);

            //human player
            huPlayer = new ManualPlayer(1,gameState);
            
            //ai player

            //aiPlayer = new ABNMAIPlayer(2, gameState,2); //new RandomAIPlayer(2, gameState);//

            aiPlayer = new ABNMAIPlayer(2, gameState, 2, new int[] {
                60,20,5,1
            }, new int[] {
                //60,20,5,-1
                 50,10,5,1
            });

            //testAIPlayer = new ABNMAIPlayer(1, gameState, 2, new int[] {
            //    10,5,8
            //}, new int[] {
            //    15,5,10
            //});

            //comon settings
            decoRect = new Rectangle(0, 0, 256, 256);
        }
        public override void Init()
        {
            //init periods
            currentColor = Constants.COLOR_DECORATION_RECT;
            base.Init();
            huPlayer.Init();
            aiPlayer.Init();
            //testAIPlayer.Init();
            gridMan.Init();
            //register new player
            gameState.AddPlayer(huPlayer);
            //gameState.AddPlayer(testAIPlayer);
            gameState.AddPlayer(aiPlayer);

            //gameState.UpdatePlayBoard(2);
            //reset new game
            Reset();
        }

        public override void Reset(bool isActive = false)
        {

            gameState.RestartNewGame();
            gridMan.SetGameState(gameState);
            debugAIPerTurn = false;
            debugTestAIPerTurn = false;
            flagGameOver = false;
        }

        public override void Load()
        {
            base.Load();
            //ResourceManager rm = ResourceManager.GetInstance();
            //rm.LoadTexture("test_crown.png");

        }

        public void HandleMouseInput(object sender, MouseEventArgs e)
        {

            var point = e.Location;

            if (e.Button == MouseButtons.Left && gameState.CurrentPlayerId == huPlayer.PlayerId)
            {
                debug = "raw pos =" + point.ToString() + "\n";
                var transformedPos = Utils.PixelToPosition(point, Constants.GRID_ORIGIN, Constants.GRID_HEX_RADIUS);

                if (!gameState.CheckGameOver() && gameState.Board.ContainsKey(transformedPos) && !flagGameOver)
                {
                    Command nextCommand = gameState.GetNextStone(CommandType.MoveStone, transformedPos);
                    huPlayer.NextCommand(nextCommand);
                    debug += "tfpos=" + transformedPos.ToString();
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
        public void HandleKeyboardInput(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
            {
                variant++;
                variant = Math.Min(variant, gameState.HexesPerSide);
            }
            else if (e.KeyCode == Keys.S)
            {
                variant--;
                variant = Math.Max(variant, 2);
            }
            else if (e.KeyCode == Keys.U)
            {
                OnBtnUndoClicked();
            }
            else if(e.KeyCode == Keys.Y)
            {
                OnBtnDoubleUndoClicked();
            }
            else if(e.KeyCode == Keys.Space)
            {
                if(!debugAIPerTurn && gameState.CurrentPlayerId == aiPlayer.PlayerId)
                    debugAIPerTurn = true;
            }
            else if(e.KeyCode == Keys.R)
            {
                Reset();
            }
            if (oldVariant != variant)
            {
                oldVariant = variant;
                gameState.UpdatePlayBoard(variant - 1);
                gridMan.SetupConerns();
            }
        }
        public void OnBtnUndoClicked()
        {
            //U pressed
            var undoState = gameState.Undo();
            if (undoState != null)
            {
                gameState.AssignFrom( undoState);
                gridMan.SetGameState(gameState);
                gameState.ResetPlayersScore();
                if (flagGameOver)
                {
                    flagGameOver = false;

                    
                }
            }
        }
        public void OnBtnDoubleUndoClicked()
        {
            //Y pressed
            var undoState = gameState.Undo();
            if (undoState != null)
            {
                var prevUndoState = undoState.Undo();
                if (prevUndoState != null)
                {
                    var prevPrevUndoState = prevUndoState.Undo();
                    if (prevPrevUndoState != null)
                    {
                        gameState.AssignFrom(prevPrevUndoState);
                        gridMan.SetGameState(gameState);

                        if (flagGameOver)
                        {
                            flagGameOver = false;

                            gameState.ResetPlayersScore();

                        }
                    }
                }
            }
        }


        int testMatches = -1;
        public override void Update(float dt)
        {
            if (!flagGameOver)
            {
                if (!gameState.CheckGameOver())
                {
                    Command command1 = huPlayer.GetCommand();
                    Command command2 = null;
                   
                    if (debugAIPerTurn)
                    {
                        command2 = aiPlayer.GetCommand();
                        debugAIPerTurn = false;
                    }
                    gameState.AddCommand(command1);
                    gameState.AddCommand(command2);

                    gameState.Cycle(dt);

                    if (gameState.CurrentPlayerId == 1)
                        decoRect = new Rectangle(Width - 256, 0, 256, 128);
                    else
                        decoRect = new Rectangle(Width - 256, Height - 128, 256, 128);
                }
                else
                {

                    //calculate score
                    //gameState.UpdateScores();
                    gameState.UnionFindAlgorithm();


                    //re-learning if need
                    //huPlayer.GameOver(gameState.GetWinners());
                    //aiPlayer.GameOver(gameState.GetWinners());

                    huPlayer.GameOver(gameState.GetWinner());
                    //testAIPlayer.GameOver(gameState.GetWinner());
                    aiPlayer.GameOver(gameState.GetWinner());

                    testMatches++;



                    //Reset();
                }
            }

        }
        public override void Draw()
        {

            sp.FillRectangle(decoRect, Constants.COLOR_DECORATION_RECT);
            gridMan.Draw();

            
        }
        public override void OnGUI()
        {
            GUI.Begin();
            GUI.Label(new Rectangle(0, Height * 1 / 4, 100, 100), "variant- play radius=" + variant);
            GUI.Label(new Rectangle(0, Height / 2, 100, 100), "debug  =" + debug);
            GUI.Label(new Rectangle(Width / 2 - 64, 16, 128, 16), "CURRENT PLAYER-" + gameState.CurrentPlayerId);
            GUI.Label(new Rectangle(Width / 2 - 64, 48, 128, 16), "PLAYER"+huPlayer.PlayerId+"_SCORE:" + huPlayer.Score);
            //GUI.Label(new Rectangle(Width / 2 - 64, 48, 128, 16), "PLAYER" + testAIPlayer.PlayerId + "_SCORE:" + testAIPlayer.Score);
            GUI.Label(new Rectangle(Width / 2 - 64, 64, 128, 16), "PLAYER" + aiPlayer.PlayerId + "SCORE:" + aiPlayer.Score);
            
            if (gameState.CurrentPlayerId == 1)
            {
                GUI.Label(new Rectangle(Width - 64, 64, 128, 16), "PLAYER " + huPlayer.PlayerId, Color.Black, currentColor);
                //GUI.Label(new Rectangle(Width - 64, 64, 128, 16), "PLAYER " + testAIPlayer.PlayerId, Color.Black, currentColor);

                GUI.Label(new Rectangle(Width - 64, Height - 64, 128, 16), "PLAYER " + aiPlayer.PlayerId, Color.Black, Color.White);
            }
            else
            {
                GUI.Label(new Rectangle(Width - 64, 64, 128, 16), "PLAYER " + huPlayer.PlayerId, Color.Black, Color.White);
                //GUI.Label(new Rectangle(Width - 64, 64, 128, 16), "PLAYER " + testAIPlayer.PlayerId, Color.Black, Color.White);

                GUI.Label(new Rectangle(Width - 64, Height - 64, 128, 16), "PLAYER " + aiPlayer.PlayerId, Color.Black, currentColor);
            }

            GUI.Label(new Rectangle(Width - 200, Height/2, 200, 128),
                "W:increase playing hexes per side\n"
                + "S:decrease playing hexes per side\n"
                + "U:undo 1 turn\n"
                + "Y:undo player turn\n"
                + "R:reset new round\n"
                + "Space:next AI turn"
                , Color.Black,Color.Salmon);

            GUI.End();

        }
    }
}
