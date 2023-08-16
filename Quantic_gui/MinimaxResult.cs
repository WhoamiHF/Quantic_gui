using Quantic_console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantic_gui
{
    /** 
     * Class used to store result of minimax and transfer it to lesser depth
     * Contains information about optimal score and move which is the first move needed to be made
     */
    class MinimaxResult
    {
        double score;
        Move? move;
        public MinimaxResult(double score, Move? move)
        {
            this.score = score;
            this.move = move;
        }

        public double Score
        {
            get
            {
                return score;
            }

            set
            {
                score = value;
            }
        }

        public Move? Move
        {
            get
            {
                return move;
            }

            set
            {
                move = value;
            }
        }
    }
}
