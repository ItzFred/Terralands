using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;
using Terralands.Systems.WeaponSystem;

namespace Terralands.Systems.WeaponSystem.WeaponParts.Gun.Magazine
{
    internal class TestGunMagazine : WeaponPart
    {
        public override byte partType => WeaponPartID.gunParts.magazine;

        public override Dictionary<byte, Vector2> connectionPoints => new Dictionary<byte, Vector2>()
        {
            { WeaponPartID.gunParts.body, new Vector2(6,0) },
        };
    }
}
