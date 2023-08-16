using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quantic_gui
{
    /**
     * class used for storing alpha and beta values enabling passing reference to all threads on nullth depth in minimax
     */
    class MinimaxInfo
    {
        double alpha;
        double beta;
        public MinimaxInfo(double alpha, double beta)
        {
            this.alpha = alpha;
            this.beta = beta;
        }

        public double Alpha
        {
            get
            {
                return alpha;
            }
            set
            {
                alpha = value;
            }
        }
        public double Beta
        {
            get
            {
                return beta;
            }
            set
            {
                beta = value;
            }
        }
    }
}
