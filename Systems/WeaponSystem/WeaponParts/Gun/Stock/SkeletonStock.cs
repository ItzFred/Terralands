using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;
using Terralands.Systems.WeaponSystem;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader.Config;
using Terraria.ModLoader;

namespace Terralands.Systems.WeaponSystem.WeaponParts.Gun.Stock
{
    internal class SkeletonStock : WeaponPart
    {
        public override byte partType => WeaponPartID.gunParts.stock;

        public override Dictionary<byte, Vector2> connectionPoints => new Dictionary<byte, Vector2>()
        {
            { WeaponPartID.gunParts.body, new Vector2(22,6) },
        };
    }
}
