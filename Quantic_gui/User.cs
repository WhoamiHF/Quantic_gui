using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantic_console
{
    /**
     * Class representing user player. Inherits from abstract player. 
     * In console version instead of coming up with a move, it asks for user input.
     * 
     */
    internal class User : Player
    {
        public User(Piece.PlayerID player) : base(player)
        {
        }

        public User(List<Piece> pieces, Piece.PlayerID player) : base(pieces, player)
        {
        }

        public User(User other) : base(other)
        {
        }

        public override Player Copy()
        {
            User result = new User(this);

            return result;
        }

        /**
         * In console version this method is used to ask user for input move
         */
        public override Move? SelectMove(GameLogic gameLogic, Board board, Player current, Player other)
        {
            int x = -1;
            int y = -1;
            Console.WriteLine("Select x and y coordinate (0..3)");
            while (!int.TryParse(Console.ReadKey().KeyChar.ToString(), out x) ||
                !int.TryParse(Console.ReadKey().KeyChar.ToString(), out y) || x <0 || x >3 || y < 0 || y > 3)
            {
                Console.WriteLine("Select x and y coordinate (0..3)");
            }

            Console.WriteLine("\nSelect shape - c p r s");

            Piece.ShapeType shape = Piece.ShapeType.CUBE;
            char key = Console.ReadKey().KeyChar;
            switch (key)
            {
                case 'c':
                    shape = Piece.ShapeType.CUBE;
                    break;
                case 'p':
                    shape = Piece.ShapeType.PYRAMID;
                    break;
                case 'r':
                    shape = Piece.ShapeType.CYLINDER;
                    break;
                case 's':
                    shape = Piece.ShapeType.SPHERE;
                    break;
            }

            Console.WriteLine();
            return new Move(x, y, Owner, shape);
        }
    }
}
