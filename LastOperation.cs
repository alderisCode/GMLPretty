using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GMLPretty
{
    public enum LastOperation : int
    {
        None = 0,
        StartNode = 1,
        EndNode = 2,
        Declaration = 3,
        Value = 4,
        WhiteSpace = 5,
        EmptyNode = 6,
        Comment= 7,
        Other = 100
    }
}
