using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;
using Terralands.Systems.WeaponSystem;

namespace Terralands.Systems.WeaponSystem.WeaponParts.Gun.Body
{
    internal class VladofSMGBody : WeaponPart
    {
        public override byte partType => WeaponPartID.gunParts.body;

        public override Dictionary<byte, Vector2> connectionPoints => new Dictionary<byte, Vector2>()
        {
            { WeaponPartID.gunParts.grip, new Vector2(8,12) },
            { WeaponPartID.gunParts.barrel, new Vector2(22,6) },
            { WeaponPartID.gunParts.magazine, new Vector2(16,12) },
            { WeaponPartID.gunParts.stock, new Vector2(4,8) }
        };
    }
}
