//Created by Fred :)

using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ID;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Terralands
{
	public class Terralands : Mod
	{
        public override void Load()
        {

            Asset<Effect> weaponSkinShader = ModContent.Request<Effect>("Terralands/Effects/WeaponSkinShader");
            GameShaders.Misc["WeaponSkinShader"] = new MiscShaderData(weaponSkinShader, "WeaponSkinShader");
        }
    }
}
