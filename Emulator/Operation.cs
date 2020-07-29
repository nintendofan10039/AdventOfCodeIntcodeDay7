using System;
using System.Collections.Generic;
using System.Text;

namespace Emulator
{
        enum Operation
        {
            Add = 1,
            Multiply = 2,
            Input = 3,
            Output = 4,
            JumpIfNotZero = 5,
            JumpIfZero = 6,
            IsLessThan = 7,
            IsEqualTo = 8,
            Finished = 99
        }
}
