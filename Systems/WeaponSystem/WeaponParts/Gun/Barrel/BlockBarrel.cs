using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Terralands.Systems.WeaponSystem.WeaponParts.Gun.Barrel
{
    internal class BlockBarrel : WeaponPart
    {
        public override byte partType => WeaponPartID.gunParts.barrel;

        public override Dictionary<byte, Vector2> connectionPoints => new Dictionary<byte, Vector2>()
        {
            { WeaponPartID.gunParts.body, new Vector2(8,8) }
        };
    }
}
