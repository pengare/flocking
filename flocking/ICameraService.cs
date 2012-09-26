using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;


namespace flocking
{
    interface ICameraService
    {
        Matrix ViewMatrix { get; }
        Matrix ProjectMatrix { get; }
    }
}
