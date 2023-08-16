using Quantic_gui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantic_console
{
    /**
     * Main class ensure game flow. Creates board, players and if user wants sets second player to be computer.
     */
    internal class Game
    {
        readonly Player player1;
        Player player2;
        readonly Board board;
        GameState state;

        public enum GameState { FIRST_PLAYER_TURN,SECOND_PLAYER_TURN,FIRST_PLAYER_WON,SECOND_PLAYER_WON}

        public Game() {
            player1 = new User(Piece.PlayerID.PLAYER_ONE);
            player2 = new User(Piece.PlayerID.PLAYER_TWO);
            board = new Board();
            state = new GameState();
        }

        public Player Player1 { get { return player1; } }
        public Player Player2 { get { return player2; } }

        public GameState State
        {
            get { return state; }
            set { state = value; }
        }
        public Board Board { get { return board; } }

        //Sets second player to be played by computer
        public void SetComputerPlayer(Logger logger)
        {
            player2 = new ArtificialPlayer(Piece.PlayerID.PLAYER_TWO,logger);
        }

        //Asks user for decision which scenario of game will be played - player x player, player x computer
        //Used in console version of the app
        public void GetPlayers()
        {
            bool selected = false;
            while (!selected)
            {
                Console.WriteLine("Do you want to play against other user or against computer?");
                Console.WriteLine("Press \"u\" to play against another user or \"c\" to play against computer");
                char choice = Console.ReadKey().KeyChar;
                if (choice.Equals('u'))
                {
                    Console.WriteLine("\nYou have pressed: " + choice + " and chosen to play against another user");
                    selected = true;
                   
                }
                else if (choice.Equals('c'))
                {
                    Console.WriteLine("\nYou have pressed: " + choice + " and chosen to play against computer");
                    player2 = new ArtificialPlayer(Piece.PlayerID.PLAYER_TWO,null);
                    selected = true;
                }
                else
                {
                    Console.WriteLine("\nYou have pressed " + choice + " which is not a valid choice");
                }
            }
        }

        //This function ensures that each player gets their turn and that game will end when one player wins
        //Used in console version of the app
        public void PlayGame(BoardViewer viewer)
        {
            state = GameState.FIRST_PLAYER_TURN;
            GameLogic gameLogic = new GameLogic();
            while(state == GameState.FIRST_PLAYER_TURN || state == GameState.SECOND_PLAYER_TURN)
            {
                Console.WriteLine("First player's turn");

                if (gameLogic.CheckLoss(Piece.PlayerID.PLAYER_ONE))
                {
                    state = GameState.SECOND_PLAYER_WON;
                    viewer.ShowWin(Piece.PlayerID.PLAYER_TWO);
                    break;
                }

                viewer.ViewPlayerPieces(player1);
                Move? move = player1.SelectMove(gameLogic,board,player1,player2);

                while (!GameLogic.CheckMove(move, board, player1))
                {
                    move = player1.SelectMove(gameLogic, board, player1, player2);
                }
                gameLogic.MakeMove(move, board,player1);

                List<Move> moves = gameLogic.GetCurrentPossibleMoves(player1,null);
                foreach(Move move2 in moves)
                {
                    Console.WriteLine($"Move {move2}");
                }

                if (GameLogic.CheckWin(board, move))
                {
                    state = GameState.FIRST_PLAYER_WON;
                    viewer.ShowWin(Piece.PlayerID.PLAYER_ONE);
                }

                if(state != GameState.FIRST_PLAYER_TURN && state != GameState.SECOND_PLAYER_TURN)
                {
                    break;
                }

                viewer.ViewBoard(board);

                Console.WriteLine("Second player's turn");
                if (gameLogic.CheckLoss(Piece.PlayerID.PLAYER_TWO))
                {
                    state = GameState.FIRST_PLAYER_WON;
                    viewer.ShowWin(Piece.PlayerID.PLAYER_ONE);
                    break;
                }

                viewer.ViewPlayerPieces(player2);
                
                move = player2.SelectMove(gameLogic, board, player2, player1);
                
                while (!GameLogic.CheckMove(move, board, player2))
                {
                    move = player2.SelectMove(gameLogic, board, player2, player1);
                }
                gameLogic.MakeMove(move, board,player2);
                
                if (GameLogic.CheckWin(board, move))
                {
                    state = GameState.SECOND_PLAYER_WON;
                    viewer.ShowWin(Piece.PlayerID.PLAYER_TWO);
                }

                List<Move> moves2 = gameLogic.GetCurrentPossibleMoves(player2,null);
                foreach (Move move2 in moves2)
                {
                    Console.WriteLine($"Move {move2}");
                }

                viewer.ViewBoard(board);
            }
        }
    }
}
