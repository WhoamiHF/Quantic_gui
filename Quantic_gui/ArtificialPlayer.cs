using Quantic_gui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantic_console
{
    /**
     * Class for computer player, contains logic for finding the
     * optimal move. The optimal move is found using minimax
     * algorithm and is sped up by alpha beta prunning and using
     * multi-thread programming
     */ 
    internal class ArtificialPlayer : Player
    {
        //Cancellation token is used to stop computation of optimal move
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private Logger? _logger;

        /**
         * Constructor for artificial player, gets pieces and info 
         * whatever is first or second player
         */
        public ArtificialPlayer(List<Piece> pieces, Piece.PlayerID player,Logger? logger) : base(pieces, player)
        {
            _logger = logger;
        }

        /**
         * Constructor for artificial player, gets info 
         * whatever is first or second player and reference to logger instance
         */
        public ArtificialPlayer(Piece.PlayerID player,Logger? logger) : base(player)
        {
            _logger = logger;
        }


        /**
         * Copy constructor for artificial player;
         */
        public ArtificialPlayer(ArtificialPlayer other) : base(other)
        {
            _logger = other._logger;
        }

        /**
         * Method to get deep copy of this instance (used during minimax) 
         * by using copy constructor
         */
        public override Player Copy()
        {
            ArtificialPlayer result = new ArtificialPlayer(this);
           
            return result;
        }

        /**
         * Main function to get optimal move. Gets gameLogic which
         * tracks which moves can be played, board with current game state
         * and instance of player whose turn currently is (to get available pieces)
         */
        public override Move SelectMove(GameLogic gameLogic, Board board,Player current, Player other)
        {
            HashSet<Piece.ShapeType> shapes = GetUsedShapes(board);

            return MinimaxDepthZero(gameLogic, board,current,other,shapes,-1000,1000).Result.Move!;
        }

        /**
         * Gets board and returns set of shapes found on board's squares
         * used during determination which shapes to consider during minimax
         */
        private static HashSet<Piece.ShapeType> GetUsedShapes(Board board)
        {
            HashSet<Piece.ShapeType> shapes = new HashSet<Piece.ShapeType>();
            for(int i = 0; i < 4; i++)
            {
                for(int j = 0; j < 4; j++)
                {
                    if(board.Squares[i][j].Piece != null)
                    {
                        shapes.Add(board.Squares[i][j].Piece.Shape);
                    }
                }
            }
            return shapes;
        }


        /**
         * Gets set containing used shapes and determines which shapes to consider playing.
         * The idea is that either one of already present shapes or a new one can be played and
         * there is no need to distinguish between new shapes
         */
        private static HashSet<Piece.ShapeType> DetermineWhichShapesToConsider(HashSet<Piece.ShapeType> usedShapes)
        {
            if(usedShapes.Count == 4)
            {
                return usedShapes;
            }

            if (!usedShapes.Contains(Piece.ShapeType.PYRAMID))
            {
                usedShapes.Add(Piece.ShapeType.PYRAMID);
                return usedShapes;
            }

            if (!usedShapes.Contains(Piece.ShapeType.CUBE))
            {
                usedShapes.Add(Piece.ShapeType.CUBE);
                return usedShapes;
            }

            if (!usedShapes.Contains(Piece.ShapeType.CYLINDER))
            {
                usedShapes.Add(Piece.ShapeType.CYLINDER);
                return usedShapes;
            }

            if (!usedShapes.Contains(Piece.ShapeType.SPHERE))
            {
                usedShapes.Add(Piece.ShapeType.SPHERE);
                return usedShapes;
            }

            return usedShapes;
        }

        /**
         * First level of minimax, different by using threads to sped up
         * the computation by using parallel computation via threads
         */
        private async Task<MinimaxResult> MinimaxDepthZero(GameLogic gameLogic, Board board,
            Player currentPlayer, Player otherPlayer, HashSet<Piece.ShapeType> usedShapes,
            double alpha, double beta)
        {
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();

            List<Move> moves = gameLogic.GetCurrentPossibleMoves(this, DetermineWhichShapesToConsider(usedShapes));

            MinimaxResult result = new MinimaxResult(-1000, null);
            MinimaxInfo alphaBeta = new MinimaxInfo(alpha, beta);
     
            int finishedThreads = 0;
            Console.WriteLine("number of moves" + moves.Count);

            List<Task> tasks = new List<Task>();

            foreach (Move move in moves)
            {
                tasks.Add(Task.Run(() =>
                {
                    WorkerMinimax(gameLogic, currentPlayer,
                    otherPlayer, board, move, usedShapes, alphaBeta, result);
                    lock (moves)
                    {
                        finishedThreads++;
                    }
                }));            
            }

            await Task.WhenAll(tasks);

            return result;
        }

        /**
         * This function is called by MinimaxDepthZero for each move in 
         * different worker thread. Gets move which is starter move
         * and explores the state space after the moves performation
         */
        private void WorkerMinimax(GameLogic gameLogic, Player currentPlayer,
            Player otherPlayer, Board board, Move move, HashSet<Piece.ShapeType> usedShapes, MinimaxInfo alphaBeta, MinimaxResult result)
        {
            if (_cancellationTokenSource.Token.IsCancellationRequested)
            {
                return;
            }

            double score = EvaluateMove(gameLogic, currentPlayer, otherPlayer, board, move, 0,
                usedShapes, alphaBeta.Alpha, alphaBeta.Beta);;

            _logger?.Log("\t" +score +" " + move);
            lock (result)
            {
                if (score > result.Score || result.Move == null)
                {
                    result.Score = score;
                    result.Move = move;
                    Console.WriteLine(move);
                }
            }

            double max = 1000;
            lock (_cancellationTokenSource)
            {
                if (result.Score == max)
                {
                    Console.WriteLine("cancellation because of max!" + move);
                    _cancellationTokenSource.Cancel();
                }

                lock (alphaBeta)
                {
                    alphaBeta.Alpha = Math.Max(alphaBeta.Alpha, result.Score);
                    if (alphaBeta.Beta <= alphaBeta.Alpha)
                    {
                        //Console.WriteLine("prunning depth: " + depth);
                        Console.WriteLine("cancellation because of alphaBeta!" + move);
                        _cancellationTokenSource.Cancel();
                    }
                }
            }
        }

        /**
         * Called from WorkerMinimax, performs minimax without any more thread branching
         */
        private MinimaxResult MiniMax(GameLogic gameLogic,Board board, int depth, 
            Player currentPlayer, Player otherPlayer,HashSet<Piece.ShapeType> usedShapes,
            double alpha, double beta)
        {

            if(depth == 4)
            {
                return new MinimaxResult(EvaluatePosition(gameLogic, currentPlayer, otherPlayer),null);
            }

            List<Move> moves = gameLogic.GetCurrentPossibleMoves(currentPlayer,DetermineWhichShapesToConsider(usedShapes));

            double bestScore = depth % 2 == 0 ? -1000 : 1000;

            Move? bestMove = null;
            int i = 0;
            foreach (Move move in moves)
            {
             
                double score = EvaluateMove(gameLogic, currentPlayer, otherPlayer, board, move, depth,usedShapes,alpha,beta);
                i++;

                if ((score < bestScore && depth % 2 == 1) ||(score > bestScore && depth % 2 == 0) || bestMove == null)
                {
                    bestScore = score;
                    bestMove = move; 
                }

                double max = depth % 2 == 0 ? 1000 : -1000;
                if (bestScore == max)
                {
                    break;
                }

                if (depth % 2 == 0)
                {
                    alpha = Math.Max(alpha, bestScore);
                }
                else
                {
                    beta = Math.Min(beta, bestScore);
                }

                if (beta <= alpha)
                {
                    //Console.WriteLine("prunning depth: " + depth);
                    break;
                }

            }
            return new MinimaxResult(bestScore,bestMove!);
        }

        /**
         * Gives evaluation of move by checking whatever the player has won,
         * playing the move and continuing with minimax
         */
        private double EvaluateMove(GameLogic gameLogic,Player currentPlayer, Player otherPlayer, 
            Board board, Move move, int depth,HashSet<Piece.ShapeType> usedShapes, double alpha, double beta)
        {
            GameLogic logicCopy = new GameLogic(gameLogic);
            Player currentPlayerCopy = currentPlayer.Copy();
            Player otherPlayerCopy = otherPlayer.Copy();
            Board boardCopy = board.Copy();

            logicCopy.MakeMove(move, boardCopy, currentPlayerCopy);

            if (GameLogic.CheckWin(boardCopy, move))
            {
                return depth % 2 == 0 ? 1000 : -1000;
            }

            HashSet<Piece.ShapeType> usedShapesUpdated = new HashSet<Piece.ShapeType>();

            foreach (Piece.ShapeType shape in usedShapes)
            {
                usedShapesUpdated.Add(shape);
            }


            if (!usedShapesUpdated.Contains(move.Shape))
            {
                usedShapesUpdated.Add(move.Shape);
            }

            double score = MiniMax(logicCopy, boardCopy, depth + 1, otherPlayerCopy,
                currentPlayerCopy, usedShapesUpdated, alpha, beta).Score;
            return score;
        }


        /**
         * Evaluates current board - finds how many move each player can make and 
         * returns the difference (from current player view)
         */
        private static double EvaluatePosition(GameLogic gameLogic,Player currentPlayer,Player otherPlayer)
        {
            return gameLogic.GetCurrentPossibleMoves(currentPlayer,null).Count - gameLogic.GetCurrentPossibleMoves(otherPlayer,null).Count;
        }
    }
}
