using Quantic_console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.ComponentModel.Design.ObjectSelectorEditor;
using System.Windows.Forms;
using System.Net.Http.Headers;
using System.Collections.ObjectModel;

namespace Quantic_gui
{
    /**
     * Class responsible for showing user current state of the game and dealing with user moves and showing responses
     */
    internal class GUIBoardViewer : BoardViewer
    {

        Game game;
        GameLogic logic;
        readonly Form1 form;
        Logger logger;

        Piece.ShapeType shape = Piece.ShapeType.CUBE;

        readonly Dictionary<Coordinates, PictureBox> mapFromCoordinates = new Dictionary<Coordinates, PictureBox>();
        readonly Dictionary<Piece.ShapeType, List<PictureBox>> mapFromShapes = new Dictionary<Piece.ShapeType, List<PictureBox>>();

        readonly Dictionary<PictureBox, Coordinates> mapToCoordinates = new Dictionary<PictureBox, Coordinates>();
        readonly Dictionary<PictureBox, Piece.ShapeType> mapToShapesPlayerOne = new Dictionary<PictureBox, Piece.ShapeType>();
        readonly Dictionary<PictureBox, Piece.ShapeType> mapToShapesPlayerTwo = new Dictionary<PictureBox, Piece.ShapeType>();

        PictureBox? selectedPlayerOne = null;
        PictureBox? selectedPlayerTwo = null;

        /**
         * Constructor setting references to game, logic, visual form and logger
         */
        public GUIBoardViewer(Game game, GameLogic logic, Form1 form, Logger logger) : base()
        {
            this.game = game;
            this.logic = logic;
            this.form = form;
            this.logger = logger;
        }

        /**
         * Shows button informing user that game has ended and shows which player has won. Sets reset button
         */
        public override void ShowWin(Piece.PlayerID winner)
        {

            String message = winner == Piece.PlayerID.PLAYER_ONE ? "Player one has won!" : "Player two has won!";
            logger.Log(message);
            logger.Log("\t\tBoard");
            foreach(Square[] squares in game.Board.Squares)
            {
                foreach (Square square in squares)
                {
                    if (square.Piece != null)
                    {
                        logger.Log(square.Piece.ToString());
                    }
                }
            }

            form.Invoke(() =>
            {
                int screenWidth = Screen.PrimaryScreen.Bounds.Width;
                int screenHeight = Screen.PrimaryScreen.Bounds.Height;

                Button btn = new Button();

                btn.Size = new Size(200, 200);
                btn.Location = new Point(screenWidth / 2 - 100, screenHeight / 2 - 100);
                form.Controls.Add(btn);
                btn.Text = message;
                btn.Click += ResetGame;
                btn.BringToFront();
            });
        }

        /**
         * Resets graphical elements, game, logic and logger instance references.
         * Also resets map from/to visual elements
         */
        public void ResetGame(object? sender, EventArgs e)
        {
            if(sender == null)
            {
                return;
            }

            bool computer = game.Player2 is ArtificialPlayer;
            form.Controls.Clear();
            game = new Game();
            logger = new Logger(form);
            logic = new GameLogic(logger);

            mapFromCoordinates.Clear();
            mapFromShapes.Clear();
            mapToCoordinates.Clear();
            mapToShapesPlayerOne.Clear();
            mapToShapesPlayerTwo.Clear();

            SetBoard(form.Controls);

            if (computer)
            {
                game.SetComputerPlayer(logic.Logger!);
            }
            game.State = Game.GameState.FIRST_PLAYER_TURN;
        }

        
        /**
         * Sets board (squares)
         */
        public override void ViewBoard(Board board)
        {
            SetBoard(form.Controls);
        }

        /**
         * Draws figures (player pieces)
         */
        public override void ViewPlayerPieces(Player player)
        {
            DrawFigures(form.Controls);
        }

        /**
         * Draws each figure for both players and sets on click listeners enabling selection of the piece/shape
         */
        private void DrawFigures(Control.ControlCollection Controls)
        {
            List<string> filenames = new List<string>
            {
                "cylinder.png",
                "cube.png",
                "pyramid.png",
                "sphere.png"
            };

            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;

            for (int i = -2; i < 3; i++)
            {
                if (i == 0)
                {
                    continue;
                }

                int j = -2;
                foreach (string filename in filenames)
                {
                    Image image = Image.FromFile((i > 0 ? "" : "color_") + filename);

                    PictureBox pictureBox = new PictureBox
                    {
                        Image = image,
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        Location = new Point(screenWidth / 2 + 700 * (i > 0 ? 1 : -1) + 200 * (i - 1) - 100, screenHeight / 2 + 200 * j),
                        Width = 200,
                        Height = 200
                    };

                    switch (filename)
                    {
                        case "cylinder.png":
                            
                            if (i > 0)
                            {
                                mapToShapesPlayerTwo.Add(pictureBox, Piece.ShapeType.CYLINDER);
                                if (!mapFromShapes.ContainsKey(Piece.ShapeType.CYLINDER))
                                {
                                    mapFromShapes.Add(Piece.ShapeType.CYLINDER, new List<PictureBox>());
                                }
                                mapFromShapes[Piece.ShapeType.CYLINDER].Add(pictureBox);
                            }
                            else
                            {
                                mapToShapesPlayerOne.Add(pictureBox, Piece.ShapeType.CYLINDER);

                            }
                            break;
                        case "cube.png":
                            
                            if (i > 0)
                            {
                                mapToShapesPlayerTwo.Add(pictureBox, Piece.ShapeType.CUBE);

                                if (!mapFromShapes.ContainsKey(Piece.ShapeType.CUBE))
                                {
                                    mapFromShapes.Add(Piece.ShapeType.CUBE, new List<PictureBox>());
                                }
                                mapFromShapes[Piece.ShapeType.CUBE].Add(pictureBox);
                            }
                            else
                            {
                                mapToShapesPlayerOne.Add(pictureBox, Piece.ShapeType.CUBE);
                            }
                            break;
                        case "pyramid.png":
                            if (i > 0)
                            {
                                mapToShapesPlayerTwo.Add(pictureBox, Piece.ShapeType.PYRAMID);
                                if (!mapFromShapes.ContainsKey(Piece.ShapeType.PYRAMID))
                                {
                                    mapFromShapes.Add(Piece.ShapeType.PYRAMID, new List<PictureBox>());

                                }
                                mapFromShapes[Piece.ShapeType.PYRAMID].Add(pictureBox);
                            }
                            else
                            {
                                mapToShapesPlayerOne.Add(pictureBox, Piece.ShapeType.PYRAMID);
                            }
                            break;
                        case "sphere.png":
                            if (i > 0)
                            {
                                mapToShapesPlayerTwo.Add(pictureBox, Piece.ShapeType.SPHERE);
                                if (!mapFromShapes.ContainsKey(Piece.ShapeType.SPHERE))
                                {
                                    mapFromShapes.Add(Piece.ShapeType.SPHERE, new List<PictureBox>());
                                }
                                mapFromShapes[Piece.ShapeType.SPHERE].Add(pictureBox);
                            }
                            else
                            {
                                mapToShapesPlayerOne.Add(pictureBox, Piece.ShapeType.SPHERE);
                            }

                            break;
                    }

                    Controls.Add(pictureBox);
                    pictureBox.Click += ProcessShapeClicked;
                    j++;
                }
            }
        }

        /**
         * Processes that user has clicked on shape picture box, based on which player currently 
         * plays and whatever a shape was already picked
         */
        private void ProcessShapeClicked(object? sender, EventArgs e)
        {
            if(sender == null)
            {
                return;
            }

            if (sender is PictureBox clickedPictureBox)
            {
                PictureBox? selected = game.State == Game.GameState.FIRST_PLAYER_TURN ? selectedPlayerOne : selectedPlayerTwo;
                Dictionary<PictureBox, Piece.ShapeType> mapToShapes = game.State == Game.GameState.FIRST_PLAYER_TURN ? mapToShapesPlayerOne : mapToShapesPlayerTwo;

                if (!mapToShapes.ContainsKey(clickedPictureBox))
                {
                    return;
                }

                if (selected != null)
                {
                    selected.BorderStyle = BorderStyle.None;
                    shape = Piece.ShapeType.CUBE;

                }

                if (selected != sender)
                {
                    clickedPictureBox.BorderStyle = BorderStyle.FixedSingle;
                    if (game.State == Game.GameState.FIRST_PLAYER_TURN) { 
                        selectedPlayerOne = (PictureBox)sender;
                    }
                    else
                    {
                        selectedPlayerTwo = (PictureBox)sender;
                    }

                    shape = mapToShapes[clickedPictureBox];
                }
            }
        }

        /**
         * Called after user clicks a square on board. Gets current player and processes user move
         */
        private void ProcessSquareClicked(object? sender, EventArgs e)
        {
            if(sender == null)
            {
                return;
            }

            if (sender is PictureBox clickedPictureBox)
            {
                if (game.State == Game.GameState.FIRST_PLAYER_TURN && game.Player1 is User)
                {
                    ProcessUserMove(selectedPlayerOne, sender, clickedPictureBox,
                        Piece.PlayerID.PLAYER_ONE, game.Player1, Piece.PlayerID.PLAYER_TWO, game.Player2);
                }
                else if (game.State == Game.GameState.SECOND_PLAYER_TURN && game.Player2 is User)
                {
                    ProcessUserMove(selectedPlayerTwo, sender, clickedPictureBox,
                        Piece.PlayerID.PLAYER_TWO, game.Player2, Piece.PlayerID.PLAYER_ONE, game.Player1);
                }

            }
        }

        /**
         * Converts user interaction to move, checks whatever it is a valid move and if so plays it and if the oponent is 
         * controled by computer calls function to find reply move
         */
        private void ProcessUserMove(PictureBox? selected, object sender, PictureBox clickedPictureBox,
            Piece.PlayerID turn, Player player, Piece.PlayerID next_turn, Player nextPlayer)
        {
            if (selected != null)
            {
                Coordinates coordinates = mapToCoordinates[clickedPictureBox];
                Move move = new Move(coordinates.X, coordinates.Y, turn, shape);

                if (GameLogic.CheckMove(move, game.Board, player))
                {
                    game.State = turn == Piece.PlayerID.PLAYER_ONE ? Game.GameState.SECOND_PLAYER_TURN : Game.GameState.FIRST_PLAYER_TURN;
                    logic.MakeMove(move, game.Board, player);
                    ShowMove((PictureBox)sender, turn == Piece.PlayerID.PLAYER_ONE);

                    selectedPlayerOne = null;
                    selectedPlayerTwo = null;

                    if (GameLogic.CheckWin(game.Board, move))
                    {
                        ShowWin(turn);
                    }
                    else
                    {
                        if (nextPlayer is ArtificialPlayer)
                        {
                            FindReplyMove(nextPlayer, next_turn, turn, player);
                        }
                    }

                }
            }
        }

        /**
         * Checks whatever computer player has lost and if the game is still on finds reply move
         */
        private void FindReplyMove(Player currentPlayer, Piece.PlayerID turn,
            Piece.PlayerID previousTurn, Player previousPlayer)
        {
            if (logic.CheckLoss(turn))
            {
                ShowWin(previousTurn);
            }

            Thread moveThread = new Thread(() =>
            {
                Move? replyMove = currentPlayer.SelectMove(logic, game.Board, currentPlayer,previousPlayer);
                if (replyMove != null)
                {
                    logic.MakeMove(replyMove, game.Board, currentPlayer);

                    ShowComputerMove(replyMove);

                    if (GameLogic.CheckWin(game.Board, replyMove))
                    {
                        ShowWin(turn);
                    }
                    else
                    {
                        game.State = game.State == Game.GameState.SECOND_PLAYER_TURN ? Game.GameState.FIRST_PLAYER_TURN : Game.GameState.SECOND_PLAYER_TURN;
                        if (logic.CheckLoss(previousTurn))
                        {
                            ShowWin(turn);
                        }
                    }
                }

            });

            moveThread.Start();
        }

        /**
         * Graphically shows move depending on selected shape and clicked square
         */
        private void ShowMove(PictureBox where, bool playerOne)
        {
            form.Invoke(new Action(() =>
            {
                if (playerOne)
                {
                        selectedPlayerOne!.Location = where.Location;
                        selectedPlayerOne!.BringToFront();
                        selectedPlayerOne!.Click -= ProcessShapeClicked;
       
                }
                else
                {
                    selectedPlayerTwo!.Location = where.Location;
                    selectedPlayerTwo!.BringToFront();
                    selectedPlayerTwo!.Click -= ProcessShapeClicked;
                }


                where.Visible = false;
            }));

        }

        /**
         * Converts computer move to visual elements and calls same function to show move as after human turn
         */
        private void ShowComputerMove(Move move)
        {
            if (!mapFromShapes.ContainsKey(move.Shape) || mapFromShapes[move.Shape].Count == 0)
            {
                logger.Log("Error - invalid move: " + move);
                throw new Exception("invalid move: " + move);
            }
            selectedPlayerTwo = mapFromShapes[move.Shape].First();
            mapFromShapes[move.Shape].Remove(selectedPlayerTwo);

            foreach (Coordinates coordinates in mapFromCoordinates.Keys)
            {
                if (coordinates.X == move.X && coordinates.Y == move.Y)
                {
                    ShowMove(mapFromCoordinates[coordinates], false);
                }
            }
        }

        /**
         * Sets each square on board with on click listeners depending on player, calls function to set available pieces
         */
        public void SetBoard(Control.ControlCollection Controls)
        {
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;


            for (int row = -2; row < 2; row++)
            {
                for (int col = -2; col < 2; col++)
                {
                    Image image = (row + col) % 2 == 0 ? Image.FromFile(@"white.png") : Image.FromFile(@"black.png");

                    PictureBox pictureBox = new PictureBox
                    {
                        Image = image,
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        Location = new Point(screenWidth / 2 + 200 * row, screenHeight / 2 + 200 * col),
                        Width = 200,
                        Height = 200
                    };

                    Controls.Add(pictureBox);

                    mapToCoordinates.Add(pictureBox, new Coordinates(row + 2, col + 2));
                    mapFromCoordinates.Add(new Coordinates(row + 2, col + 2), pictureBox);
                    pictureBox.Click += ProcessSquareClicked;
                }
            }

            DrawFigures(Controls);
        }
    }
}
