using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terralands.Systems.WeaponSystem
{
    internal static class WeaponPartID
    {
        public static GunPartID gunParts = new GunPartID();
    }

    internal class GunPartID
    {
        public byte body = 0;
        public byte grip = 1;
        public byte magazine = 2;
        public byte stock = 3;
        public byte barrel = 4;
        public byte scope = 5;
    }
}
