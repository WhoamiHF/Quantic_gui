using Quantic_gui;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Numerics;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Quantic_console
{
    /**
     * This class is referee/game master. Checks validity of turns, makes moves selected by users and
     * provides functions to check whatever a player has won (has 4 in row) or lost (has no moves)
     */
    internal class GameLogic
    {
        Dictionary<Piece.ShapeType, HashSet<Coordinates>> possibleTurnsPlayerOne;
        Dictionary<Piece.ShapeType, HashSet<Coordinates>> possibleTurnsPlayerTwo;
        Logger? logger;

        // No logging will be available by using this constructor, fills dictionary for each player
        //with possible locations for different shapes the player has
        public GameLogic()
        {
            possibleTurnsPlayerOne = new Dictionary<Piece.ShapeType, HashSet<Coordinates>>();
            possibleTurnsPlayerTwo = new Dictionary<Piece.ShapeType, HashSet<Coordinates>>();

            GetAllPossibleMovesAtTheStartOfTheGame(possibleTurnsPlayerOne);
            GetAllPossibleMovesAtTheStartOfTheGame(possibleTurnsPlayerTwo);
            logger = null;
        }

        // Logging will be performed via given logger, fills dictionary for each player
        //with possible locations for different shapes the player has
        public GameLogic(Logger logger)
        {
            possibleTurnsPlayerOne = new Dictionary<Piece.ShapeType, HashSet<Coordinates>>();
            possibleTurnsPlayerTwo = new Dictionary<Piece.ShapeType, HashSet<Coordinates>>();

            GetAllPossibleMovesAtTheStartOfTheGame(possibleTurnsPlayerOne);
            GetAllPossibleMovesAtTheStartOfTheGame(possibleTurnsPlayerTwo);
            this.logger = logger;
        }

        /** Copy constructor, used for copying possible moves during minimax 
         * Logging will not be performed, copies possible moves
        */
        public GameLogic(GameLogic other)
        {
            possibleTurnsPlayerOne = new Dictionary<Piece.ShapeType, HashSet<Coordinates>>();
            foreach (var entry in other.possibleTurnsPlayerOne)
            {
                possibleTurnsPlayerOne[entry.Key] = new HashSet<Coordinates>(entry.Value);
            }

            possibleTurnsPlayerTwo = new Dictionary<Piece.ShapeType, HashSet<Coordinates>>();
            foreach (var entry in other.possibleTurnsPlayerTwo)
            {
                possibleTurnsPlayerTwo[entry.Key] = new HashSet<Coordinates>(entry.Value);
            }

            logger = null;
        }

        /**
         * Fills dictionary with all possible moves player has at the start of the game
         */
        public static void GetAllPossibleMovesAtTheStartOfTheGame(Dictionary<Piece.ShapeType, HashSet<Coordinates>> possibleMoves)
        {
            foreach (Piece.ShapeType shape in Enum.GetValues(typeof(Piece.ShapeType)))
            {
                HashSet<Coordinates> coordinates = new HashSet<Coordinates>();
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        coordinates.Add(new Coordinates(i, j));
                    }
                }
                possibleMoves.Add(shape, coordinates);

            }
        }

        /**
         * Gets player and set of shapes to consider (or null) and gets all currently possible moves with specified shapes
         */
        public List<Move> GetCurrentPossibleMoves(Player player,HashSet<Piece.ShapeType>? shapes)
        {
            shapes ??= GetPlayersShapes(player);

            List<Move> result = new List<Move>();
            foreach (Piece.ShapeType shape in shapes)
            {
                HashSet<Coordinates> possibilities = GetPossibleMovesWithSpecifiedShape(player, shape);
                foreach (Coordinates coordinates in possibilities)
                {
                    result.Add(new Move(coordinates.X, coordinates.Y,player.Owner,shape));
                }
            }
            return result;
        }

        /**
         * Gets all coordinates where given player can place given shape
         */
        public HashSet<Coordinates> GetPossibleMovesWithSpecifiedShape(Player player, Piece.ShapeType shape)
        {
            if(player.Owner == Piece.PlayerID.PLAYER_ONE)
            {
                if (possibleTurnsPlayerOne.ContainsKey(shape))
                {
                    return possibleTurnsPlayerOne[shape];
                }
                return new HashSet<Coordinates>();
            }

            if (possibleTurnsPlayerTwo.ContainsKey(shape))
            {
                return possibleTurnsPlayerTwo[shape];

            }
        
            return new HashSet<Coordinates>();
          
        }

        /**
         * Gets set of shapes which player owns (not placed)
         */
        public static HashSet<Piece.ShapeType> GetPlayersShapes(Player player)
        {
            HashSet<Piece.ShapeType> shapes = new HashSet<Piece.ShapeType>();
            foreach (Piece piece in player.Pieces)
            {
                if (!shapes.Contains(piece.Shape))
                {
                    shapes.Add(piece.Shape);
                }
            }
            return shapes;
        }

        /**
         * Checks whatever given move can be played on given board by given player
         */
        public static bool CheckMove(Move? move, Board board, Player player)
        {
            if(move == null)
            {
                return false;
            }

            return CheckAvaibility(move, player) && CheckTurn(move, player.Owner) && CheckEmptiness(move, board)
                && CheckColumn(move, board) && CheckRow(move, board) && CheckArea(move, board);
        }

        //Checks whatever player is on their turn
        private static bool CheckTurn(Move move, Piece.PlayerID player)
        {
            return move.Player == player;
        }

        //Checks whatever the square is empty
        private static bool CheckEmptiness(Move move, Board board)
        {
            return board.Squares[move.X][move.Y].Piece == null;
        }

        //Checks whatever the shape is not already present in the column
        private static bool CheckColumn(Move move, Board board)
        {
            bool allOkay = true;

            for (int i = 0; i < 4; i++)
            {
                //Console.WriteLine("Checking " + move.X + "," + i);
                bool oneOkay = board.Squares[move.X][i].Piece == null;
                oneOkay = oneOkay || board.Squares[move.X][i].Piece.Shape != move.Shape;
                oneOkay = oneOkay || board.Squares[move.X][i].Piece.Owner == move.Player;

                allOkay = allOkay && oneOkay;
            }

            return allOkay;
        }

        //Checks whatever the shape is not already present in the row
        private static bool CheckRow(Move move, Board board)
        {
            bool allOkay = true;

            for (int i = 0; i < 4; i++)
            {
                //Console.WriteLine("Checking " + i + "," + move.Y);
                bool oneOkay = board.Squares[i][move.Y].Piece == null;
                oneOkay = oneOkay || board.Squares[i][move.Y].Piece.Shape != move.Shape;
                oneOkay = oneOkay || board.Squares[i][move.Y].Piece.Owner == move.Player;

                allOkay = allOkay && oneOkay;
            }

            return allOkay;
        }

        //Checks whatever the shape is not already present in the 2x2 area
        private static bool CheckArea(Move move, Board board)
        {
            bool allOkay = true;

            int startOfAreaX = ((int)(move.X / 2)) * 2;
            int startOfAreaY = ((int)(move.Y / 2)) * 2;

            for (int x = startOfAreaX; x <= startOfAreaX + 1; x++)
            {
                for (int y = startOfAreaY; y <= startOfAreaY + 1; y++)
                {
                    //Console.WriteLine("Checking " + x + "," + y); 
                    bool oneOkay = board.Squares[x][y].Piece == null;
                    oneOkay = oneOkay || board.Squares[x][y].Piece.Shape != move.Shape;
                    oneOkay = oneOkay || board.Squares[x][y].Piece.Owner == move.Player;

                    allOkay = allOkay && oneOkay;
                }
            }

            return allOkay;
        }

        //Checks whatever the player has the shape available
        private static bool CheckAvaibility(Move move, Player player)
        {
            foreach (Piece piece in player.Pieces)
            {
                if (piece.Shape == move.Shape)
                {
                    return true;
                }
            }
            return false;
        }

        // Makes move and removes the piece and updates list of available moves for both players
        public void MakeMove(Move move, Board? board, Player player)
        {
            board?.Squares[move.X][move.Y].SetPiece(new PlacedPiece(move.Player, move.Shape, move.X, move.Y));

            foreach (Piece piece in player.Pieces)
            {
                if (piece.Shape == move.Shape)
                {
                    player.Pieces.Remove(piece);
                    break;
                }
            }

            logger?.Log(move);
            RemoveNotValidMoves(move, player);
        }

       
        /**
         * Removes moves of both players which are no longer valid because of given move by given player
         */
        public void RemoveNotValidMoves(Move move, Player player)
        {
            Dictionary<Piece.ShapeType, HashSet<Coordinates>> opponents_possibilities = player.Owner == Piece.PlayerID.PLAYER_ONE ?
                possibleTurnsPlayerTwo : possibleTurnsPlayerOne;

            Dictionary<Piece.ShapeType, HashSet<Coordinates>> current_players_possibilities = player.Owner == Piece.PlayerID.PLAYER_ONE ?
               possibleTurnsPlayerOne : possibleTurnsPlayerTwo;

            RemoveCoordinates(new Coordinates(move.X, move.Y), possibleTurnsPlayerOne);
            RemoveCoordinates(new Coordinates(move.X, move.Y), possibleTurnsPlayerTwo);

            RemoveArea(move.Shape, opponents_possibilities, move);
            RemoveColumn(move.Shape, opponents_possibilities, move);
            RemoveRow(move.Shape, opponents_possibilities, move);

            RemoveShape(move.Shape,current_players_possibilities, player);
        }

        /**
         * Checks whatever the player played their last piece of given shape and if so removes all moves with the piece
         */
        private static void RemoveShape(Piece.ShapeType shape, Dictionary<Piece.ShapeType, HashSet<Coordinates>> possibilities, Player player)
        {
            foreach (var piece in player.Pieces)
            {
                if (piece.Shape.Equals(shape))
                {
                    return;
                }
            }
            possibilities.Remove(shape);
        }

        /**
         * Removes all moves which would place given shape into same row as given move
         */
        private static void RemoveRow(Piece.ShapeType shape, Dictionary<Piece.ShapeType, HashSet<Coordinates>> possibilities, Move move)
        {
            if (possibilities.ContainsKey(shape))
            {
                List<Coordinates> coordinatesToRemove = new List<Coordinates>();
                foreach (Coordinates coordinates in possibilities[shape])
                {
                    if (coordinates.X == move.X)
                    {
                        coordinatesToRemove.Add(coordinates);
                    }
                }

                foreach (Coordinates coordinates in coordinatesToRemove)
                {
                    possibilities[shape].Remove(coordinates);
                }

                if (possibilities[shape].Count == 0)
                {
                    possibilities.Remove(shape);
                }
            }
        }

        /**
         * Removes all moves which would place given shape into same column as given move
         */
        private static void RemoveColumn(Piece.ShapeType shape, Dictionary<Piece.ShapeType, HashSet<Coordinates>> possibilities, Move move)
        {
            if (possibilities.ContainsKey(shape))
            {
                List<Coordinates> coordinatesToRemove = new List<Coordinates>();
                foreach (Coordinates coordinates in possibilities[shape])
                {
                    if (coordinates.Y == move.Y)
                    {
                        coordinatesToRemove.Add(coordinates);
                    }
                }


                foreach (Coordinates coordinates in coordinatesToRemove)
                {
                    possibilities[shape].Remove(coordinates);
                }

                if (possibilities[shape].Count == 0)
                {
                    possibilities.Remove(shape);
                }
            }
        }

        /**
         * Removes all moves which would place given shape into same 2 by 2 area as given move
         */
        private static void RemoveArea(Piece.ShapeType shape, Dictionary<Piece.ShapeType, HashSet<Coordinates>> possibilities, Move move)
        {
            if (possibilities.ContainsKey(shape))
            {
                List<Coordinates> coordinatesToRemove = new List<Coordinates>();
                foreach (Coordinates coordinates in possibilities[shape])
                {
                    if (coordinates.X / 2 == move.X / 2 && coordinates.Y / 2 == move.Y / 2)
                    {
                        coordinatesToRemove.Add(coordinates);
                    }
                }


                foreach (Coordinates coordinates in coordinatesToRemove)
                {
                    possibilities[shape].Remove(coordinates);
                }

                if (possibilities[shape].Count == 0)
                {
                    possibilities.Remove(shape);
                }
            }
        }

        /**
         * Removes all moves which would place any shape to same coordinates as move
         */
        private static void RemoveCoordinates(Coordinates coordinates, Dictionary<Piece.ShapeType, HashSet<Coordinates>> possibilities)
        {
            foreach (Piece.ShapeType shape in Enum.GetValues(typeof(Piece.ShapeType)))
            {
                if (!possibilities.ContainsKey(shape))
                {
                    continue;
                }

                foreach(Coordinates coordinates2 in possibilities[shape])
                {
                    if (coordinates.X == coordinates2.X && coordinates.Y == coordinates2.Y)
                    {
                        possibilities[shape].Remove(coordinates2);

                        if (possibilities[shape].Count == 0)
                        {
                            possibilities.Remove(shape);
                        }
                        break;
                    }
                }
            }

        }

        public Logger? Logger
        {
            get
            {
                return logger;
            }
        }

        /**
         * Checks whatever the move completed row, column or area 
         */
        public static bool CheckWin(Board board,Move move)
        {
            return CheckColumnForWin(move,board) || CheckRowForWin(move,board) || CheckAreaForWin(move, board);
        }

        /**
         * Checks whatever given player has lost because of no possible moves
         */
        public bool CheckLoss(Piece.PlayerID player)
        {
            if(player == Piece.PlayerID.PLAYER_ONE)
            {
                return possibleTurnsPlayerOne.Count == 0;
            }
            else
            {
                return possibleTurnsPlayerTwo.Count == 0;

            }
        }

        /**
         * Checks whatever the move completed column
         */
        private static bool CheckColumnForWin(Move move, Board board)
        {
            HashSet<Piece.ShapeType> shapes = new HashSet<Piece.ShapeType>();

            for (int i = 0; i < 4; i++)
            {
                if(board.Squares[move.X][i].Piece == null)
                {
                    return false;
                }

                if (shapes.Contains(board.Squares[move.X][i].Piece.Shape))
                {
                    return false;
                }

                shapes.Add(board.Squares[move.X][i].Piece.Shape);
            }

            return true;
        }

        /**
        * Checks whatever the move completed row
        */
        private static bool CheckRowForWin(Move move, Board board)
        {
            HashSet<Piece.ShapeType> shapes = new HashSet<Piece.ShapeType>();

            for (int i = 0; i < 4; i++)
            {
                if (board.Squares[i][move.Y].Piece == null)
                {
                    return false;
                }

                if (shapes.Contains(board.Squares[i][move.Y].Piece.Shape))
                {
                    return false;
                }

                shapes.Add(board.Squares[i][move.Y].Piece.Shape);
            }

            return true;
        }

        /**
        * Checks whatever the move completed 2 by 2 area
        */
        private static bool CheckAreaForWin(Move move, Board board)
        {
            HashSet<Piece.ShapeType> shapes = new HashSet<Piece.ShapeType>();

            int startOfAreaX = ((int)(move.X / 2)) * 2;
            int startOfAreaY = ((int)(move.Y / 2)) * 2;

            for (int x = startOfAreaX; x <= startOfAreaX + 1; x++)
            {
                for (int y = startOfAreaY; y <= startOfAreaY + 1; y++)
                {
                    if (board.Squares[x][y].Piece == null)
                    {
                        return false;
                    }

                    if (shapes.Contains(board.Squares[x][y].Piece.Shape))
                    {
                        return false;
                    }

                    shapes.Add(board.Squares[x][y].Piece.Shape);
                }
            }

            return true;
        }
    }
}
