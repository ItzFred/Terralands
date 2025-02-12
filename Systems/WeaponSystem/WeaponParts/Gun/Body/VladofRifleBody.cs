using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;
using Terralands.Systems.WeaponSystem;

namespace Terralands.Systems.WeaponSystem.WeaponParts.Gun.Body
{
    internal class VladofRifleBody : WeaponPart
    {
        public override byte partType => WeaponPartID.gunParts.body;

        public override Dictionary<byte, Vector2> connectionPoints => new Dictionary<byte, Vector2>()
        {
            { WeaponPartID.gunParts.grip, new Vector2(4,10) },
            { WeaponPartID.gunParts.barrel, new Vector2(32,6) },
            { WeaponPartID.gunParts.magazine, new Vector2(20,12) },
            { WeaponPartID.gunParts.stock, new Vector2(4,6) }
        };
    }
}
