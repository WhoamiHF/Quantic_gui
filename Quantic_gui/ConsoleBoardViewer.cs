using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantic_console
{
    /**
     * Class to provide user interface for game so it can be played in console. Inherits from boardViewer.
     */
    internal class ConsoleBoardViewer : BoardViewer
    {
        //Writes to console which player has won
        public override void ShowWin(Piece.PlayerID winner)
        {
            string message = winner == Piece.PlayerID.PLAYER_ONE ? "Player one has won!" : "Player two has won!";
            Console.WriteLine(message); 
        }

        //Draws the current state of grid to the console
        public override void ViewBoard(Board board)
        {
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    Console.Write(board.Squares[x][y].GetSymbol());
                }
                Console.WriteLine();
            }
        }

        //Writes list of pieces available of given player
        public override void ViewPlayerPieces(Player player)
        {
            foreach(Piece piece in player.Pieces)
            {
                Console.Write(piece.GetSymbol() + " ");
            }
            Console.WriteLine();
        }
    }
}
