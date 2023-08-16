using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantic_console
{
    /**
     * Abstract class providing methods to view board, player pieces and message about win - console or gui descendants are implemented
     */
    internal abstract class BoardViewer
    {
        public BoardViewer() { }

        //View current state of the board
        public abstract void ViewBoard(Board board);

        //List available pieces of given player
        public abstract void ViewPlayerPieces(Player player);

        //Inform user that specified player has won
        public abstract void ShowWin(Piece.PlayerID winner);
    }
}
