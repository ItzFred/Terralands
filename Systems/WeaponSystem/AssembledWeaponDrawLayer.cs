using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using ReLogic.Content;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terralands.Systems.WeaponSystem
{
    internal class AssembledWeaponDrawLayer : PlayerDrawLayer
    {
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            if (drawInfo.drawPlayer.HeldItem?.type == ModContent.ItemType<AssembledWeapon>() && drawInfo.drawPlayer.itemAnimation > 0) return true;
            return false;
        }
        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.HeldItem);

        public Vector2 drawOffset = Vector2.Zero;
        public Vector2 negativedrawOffset = Vector2.Zero;
        public Vector2 realOffset = Vector2.Zero;

        public Vector2 weaponSize = Vector2.Zero;
        protected override void Draw(ref PlayerDrawSet drawInfo)
        {

            //WeaponPart[] weaponParts = new WeaponPart[] { };
            //if (drawInfo.drawPlayer.HeldItem.ModItem is AssembledWeapon weapon)
            //{
            //    Main.spriteBatch.End();

            //    Effect weaponShader = GameShaders.Misc["TestShader"].Shader;
            //    weaponShader.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects);

            //    Texture2D weaponMaterialTexture1 = ModContent.Request<Texture2D>(weapon.weaponMaterial1.Texture).Value;
            //    weaponShader.Parameters["materialTexture"].SetValue(weaponMaterialTexture1);
            //    weaponShader.Parameters["materialTextureSize"].SetValue(weaponMaterialTexture1.Size());
            //    Texture2D weaponMaterialTexture2 = ModContent.Request<Texture2D>(weapon.weaponMaterial2.Texture).Value;
            //    weaponShader.Parameters["materialTexture2"].SetValue(weaponMaterialTexture2);
            //    weaponShader.Parameters["materialTexture2Size"].SetValue(weaponMaterialTexture2.Size());
            //    Texture2D weaponMaterialTexture3 = ModContent.Request<Texture2D>(weapon.weaponMaterial3.Texture).Value;
            //    weaponShader.Parameters["materialTexture3"].SetValue(weaponMaterialTexture3);
            //    weaponShader.Parameters["materialTexture3Size"].SetValue(weaponMaterialTexture3.Size());

            //    weaponShader.CurrentTechnique.Passes[0].Apply();

            //    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, weaponShader, Main.GameViewMatrix.TransformationMatrix);

            //    weaponParts = weapon.weaponParts;
            //    float rotation = drawInfo.drawPlayer.direction == 1 ? drawInfo.drawPlayer.itemRotation : drawInfo.drawPlayer.itemRotation + MathHelper.Pi;

            //    Vector2 holdoutoffset = new Vector2(weapon.holdoutOffset.X, drawInfo.drawPlayer.direction == 1 ? weapon.holdoutOffset.Y : 0).RotatedBy(rotation);

            //    for (int i = 0; i < weaponParts.Length; i++)
            //    {
            //        weaponParts[i].drawn = false;
            //    }

            //    for (int i = 0; i < weaponParts.Length; i++)
            //    {
            //        WeaponPart basePart = weaponParts[i];

            //        for (int k = 0; k < basePart.connectionPoints.Count; k++)
            //        {
            //            WeaponPart connectingPart = Utils.FindSuitablePart(weaponParts, basePart.connectionPoints.Keys.ElementAt(k));
            //            if (connectingPart != null && !connectingPart.drawn)
            //            {
            //                Asset<Texture2D> tex2 = ModContent.Request<Texture2D>(connectingPart.Texture);
            //                Asset<Texture2D> tex = ModContent.Request<Texture2D>(basePart.Texture);

            //                weaponShader.Parameters["materialMap"].SetValue(ModContent.Request<Texture2D>(connectingPart.MaterialTexture).Value);
            //                weaponShader.Parameters["textureSize"].SetValue(tex2.Size());

            //                Vector2? connectingPoint = Utils.FindConnectingPoint(basePart, connectingPart.partType) == null ? Vector2.Zero : Utils.FindConnectingPoint(basePart, connectingPart.partType) * drawInfo.drawPlayer.direction;
            //                Vector2? basePartOffset = Utils.FindConnectingPoint(connectingPart, basePart.partType) == null ? Vector2.Zero : Utils.FindConnectingPoint(connectingPart, basePart.partType) * drawInfo.drawPlayer.direction;

            //                connectingPoint = drawInfo.drawPlayer.direction == 1 ? connectingPoint : new Vector2(connectingPoint.Value.X, connectingPoint.Value.Y - tex2.Height());
            //                basePartOffset = drawInfo.drawPlayer.direction == 1 ? basePartOffset : new Vector2(basePartOffset.Value.X, basePartOffset.Value.Y - tex.Height());

            //                Vector2 partOffset = new Vector2(connectingPoint.Value.X - basePartOffset.Value.X, connectingPoint.Value.Y - basePartOffset.Value.Y) * drawInfo.drawPlayer.direction * weapon.Item.scale;

            //                Vector2 setPosition = (weapon.Item.position + partOffset);
            //                Vector2 distance = weapon.Item.position - setPosition;

            //                Main.spriteBatch.Draw(tex2.Value,
            //                    (drawInfo.drawPlayer.MountedCenter - Main.screenPosition + partOffset + distance + holdoutoffset).Int(),
            //                    tex2.Frame(),
            //                    Color.White,
            //                    rotation,
            //                    new Vector2(distance.X / weapon.Item.scale, distance.Y * drawInfo.drawPlayer.direction / weapon.Item.scale),
            //                    weapon.Item.scale,
            //                    drawInfo.drawPlayer.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically,
            //                    1f);
            //                //DrawData value = new DrawData(tex2.Value,
            //                //    (drawInfo.drawPlayer.MountedCenter - Main.screenPosition + partOffset + distance + holdoutoffset).Int(),
            //                //    tex2.Frame(),
            //                //    Color.White,
            //                //    rotation,
            //                //    new Vector2(distance.X / weapon.Item.scale, distance.Y * drawInfo.drawPlayer.direction / weapon.Item.scale),
            //                //    weapon.Item.scale,
            //                //    drawInfo.drawPlayer.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically,
            //                //    1f);
            //                //drawInfo.DrawDataCache.Add(value);
            //                connectingPart.drawn = true;

            //                Vector2 pos = (Vector2)connectingPoint - (Vector2)basePartOffset;
            //                if (pos.X > 0) drawOffset.X = MathHelper.Max(drawOffset.X, pos.X + tex2.Width());
            //                if (pos.Y > 0) drawOffset.Y = MathHelper.Max(drawOffset.Y, pos.Y + tex2.Height());

            //                if (pos.X < 0) negativedrawOffset.X = MathHelper.Min(negativedrawOffset.X, pos.X);
            //                if (pos.Y < 0) negativedrawOffset.Y = MathHelper.Min(negativedrawOffset.Y, pos.Y);

            //                realOffset = new Vector2(drawOffset.X + negativedrawOffset.X, (drawOffset.Y + negativedrawOffset.Y) / 2f);

            //                weaponSize = drawOffset + negativedrawOffset.Abs();
            //            }
            //        }

            //        if (!basePart.drawn)
            //        {
            //            Asset<Texture2D> tex = ModContent.Request<Texture2D>(basePart.Texture);

            //            weaponShader.Parameters["materialMap"].SetValue(ModContent.Request<Texture2D>(basePart.MaterialTexture).Value);

            //            Main.spriteBatch.Draw(tex.Value,
            //                (drawInfo.drawPlayer.MountedCenter - Main.screenPosition + holdoutoffset).Int(),
            //                tex.Frame(),
            //                Color.White,
            //                rotation,
            //                Vector2.Zero,
            //                weapon.Item.scale,
            //                drawInfo.drawPlayer.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically,
            //                1f);
            //            //DrawData value = new DrawData(tex.Value,
            //            //    (drawInfo.drawPlayer.MountedCenter - Main.screenPosition + holdoutoffset).Int(),
            //            //    tex.Frame(),
            //            //    Color.White,
            //            //    rotation,
            //            //    Vector2.Zero,
            //            //    weapon.Item.scale,
            //            //    drawInfo.drawPlayer.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically,
            //            //    1f);
            //            //drawInfo.DrawDataCache.Add(value);
            //            basePart.drawn = true;

            //            drawOffset.X = MathHelper.Max(drawOffset.X, tex.Width());
            //            drawOffset.Y = MathHelper.Max(drawOffset.Y, tex.Height());

            //            realOffset = new Vector2(drawOffset.X + negativedrawOffset.X, (drawOffset.Y + negativedrawOffset.Y) / 2f);

            //            weaponSize = drawOffset + negativedrawOffset.Abs();
            //        }
            //    }

            //    Main.spriteBatch.End();
            //    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            //}
        }
    }
}
