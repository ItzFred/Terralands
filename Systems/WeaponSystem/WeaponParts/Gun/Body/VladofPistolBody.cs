using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;
using Terralands.Systems.WeaponSystem;

namespace Terralands.Systems.WeaponSystem.WeaponParts.Gun.Body
{
    internal class VladofPistolBody : WeaponPart
    {
        public override byte partType => WeaponPartID.gunParts.body;

        public override Dictionary<byte, Vector2> connectionPoints => new Dictionary<byte, Vector2>()
        {
            { WeaponPartID.gunParts.grip, new Vector2(10,14) },
            { WeaponPartID.gunParts.barrel, new Vector2(22,10) },
            { WeaponPartID.gunParts.magazine, new Vector2(20,14) },
            { WeaponPartID.gunParts.stock, new Vector2(6,10) }
        };
    }
}
