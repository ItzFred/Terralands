using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;
using Terralands.Systems.WeaponSystem;

namespace Terralands.Systems.WeaponSystem.WeaponParts.Gun.Barrel
{
    internal class RailgunBarrel : WeaponPart
    {
        public override byte partType => WeaponPartID.gunParts.barrel;

        public override Dictionary<byte, Vector2> connectionPoints => new Dictionary<byte, Vector2>()
        {
            { WeaponPartID.gunParts.body, new Vector2(8,8) }
        };
    }
}
