using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terralands.Systems.WeaponSystem;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using static System.Net.Mime.MediaTypeNames;

namespace Terralands
{
    internal static class Utils
    {
        public static Vector2 Abs(this Vector2 vector)
        {
            return new Vector2(Math.Abs(vector.X), Math.Abs(vector.Y));
        }

        public static Vector2 Int(this Vector2 vector)
        {
            return new Vector2((int)vector.X, (int)vector.Y);
        }

        public static WeaponPart FindSuitablePart(WeaponPart[] array, byte weaponPartID)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].partType == weaponPartID) return array[i];
            }
            return null;
        }

        public static Vector2? FindConnectingPoint(WeaponPart part, byte weaponPartID)
        {
            for (int i = 0; i < part.connectionPoints.Count; i++)
            {
                if (part.connectionPoints.Keys.ElementAt(i) == weaponPartID)
                {
                    return part.connectionPoints.Values.ElementAt(i);
                }
            }
            return null;
        }

        public static void DrawAssembledWeapon(SpriteBatch spriteBatch, AssembledWeapon weapon, Vector2 position, float rotation, bool flipped = false, bool UI = false)
        {
            spriteBatch.End();

            Effect weaponShader = GameShaders.Misc["TestShader"].Shader;
            weaponShader.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects);

            Texture2D weaponMaterialTexture1 = ModContent.Request<Texture2D>(weapon.weaponMaterial1.Texture).Value;
            weaponShader.Parameters["materialTexture"].SetValue(weaponMaterialTexture1);
            weaponShader.Parameters["materialTextureSize"].SetValue(weaponMaterialTexture1.Size());
            Texture2D weaponMaterialTexture2 = ModContent.Request<Texture2D>(weapon.weaponMaterial2.Texture).Value;
            weaponShader.Parameters["materialTexture2"].SetValue(weaponMaterialTexture2);
            weaponShader.Parameters["materialTexture2Size"].SetValue(weaponMaterialTexture2.Size());
            Texture2D weaponMaterialTexture3 = ModContent.Request<Texture2D>(weapon.weaponMaterial3.Texture).Value;
            weaponShader.Parameters["materialTexture3"].SetValue(weaponMaterialTexture3);
            weaponShader.Parameters["materialTexture3Size"].SetValue(weaponMaterialTexture3.Size());

            weaponShader.CurrentTechnique.Passes[0].Apply();

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, weaponShader, UI ? Main.UIScaleMatrix : Main.GameViewMatrix.TransformationMatrix);

            Vector2 drawOffset = Vector2.Zero;
            Vector2 negativedrawOffset = Vector2.Zero;
            Vector2 realOffset = Vector2.Zero;

            Vector2 weaponSize = Vector2.Zero;
            float properScale = 1f;

            Vector2 holdoutOffset = Vector2.Zero;
            Vector2 shootPosition = Vector2.Zero;

            WeaponPart[] weaponParts = new WeaponPart[] { };
            weaponParts = weapon.weaponParts;

            Vector2 holdoutoffset = new Vector2(weapon.holdoutOffset.X, flipped? 0 : weapon.holdoutOffset.Y).RotatedBy(rotation);

            for (int i = 0; i < weaponParts.Length; i++)
            {
                weaponParts[i].drawn = false;
            }

            for (int i = 0; i < weaponParts.Length; i++)
            {
                WeaponPart basePart = weaponParts[i];

                for (int k = 0; k < basePart.connectionPoints.Count; k++)
                {
                    WeaponPart connectingPart = Utils.FindSuitablePart(weaponParts, basePart.connectionPoints.Keys.ElementAt(k));
                    if (connectingPart != null && !connectingPart.drawn)
                    {
                        Asset<Texture2D> tex2 = ModContent.Request<Texture2D>(connectingPart.Texture);
                        Asset<Texture2D> tex = ModContent.Request<Texture2D>(basePart.Texture);

                        weaponShader.Parameters["materialMap"].SetValue(ModContent.Request<Texture2D>(connectingPart.MaterialTexture).Value);
                        weaponShader.Parameters["textureSize"].SetValue(tex2.Size());

                        Vector2? connectingPoint = Utils.FindConnectingPoint(basePart, connectingPart.partType) == null ? Vector2.Zero : Utils.FindConnectingPoint(basePart, connectingPart.partType) * (flipped ? -1 : 1);
                        Vector2? basePartOffset = Utils.FindConnectingPoint(connectingPart, basePart.partType) == null ? Vector2.Zero : Utils.FindConnectingPoint(connectingPart, basePart.partType) * (flipped ? -1 : 1);

                        connectingPoint = flipped ? new Vector2(connectingPoint.Value.X, connectingPoint.Value.Y - tex2.Height()) : connectingPoint;
                        basePartOffset = flipped ? new Vector2(basePartOffset.Value.X, basePartOffset.Value.Y - tex.Height()) : basePartOffset;

                        Vector2 partOffset = new Vector2(connectingPoint.Value.X - basePartOffset.Value.X, connectingPoint.Value.Y - basePartOffset.Value.Y) * (flipped ? -1 : 1) * weapon.Item.scale;

                        Vector2 setPosition = (weapon.Item.position + partOffset);
                        Vector2 distance = weapon.Item.position - setPosition;

                        spriteBatch.Draw(tex2.Value,
                            (position - Main.screenPosition + partOffset + distance + holdoutoffset).Int(),
                            tex2.Frame(),
                            Color.White,
                            rotation,
                            new Vector2(distance.X / weapon.Item.scale, distance.Y * (flipped ? -1 : 1) / weapon.Item.scale),
                            weapon.Item.scale,
                            flipped? SpriteEffects.FlipVertically : SpriteEffects.None,
                            1f);

                        connectingPart.drawn = true;

                        Vector2 pos = (Vector2)connectingPoint - (Vector2)basePartOffset;
                        if (pos.X > 0) drawOffset.X = MathHelper.Max(drawOffset.X, pos.X + tex2.Width());
                        if (pos.Y > 0) drawOffset.Y = MathHelper.Max(drawOffset.Y, pos.Y + tex2.Height());

                        if (pos.X < 0) negativedrawOffset.X = MathHelper.Min(negativedrawOffset.X, pos.X);
                        if (pos.Y < 0) negativedrawOffset.Y = MathHelper.Min(negativedrawOffset.Y, pos.Y);

                        realOffset = new Vector2(drawOffset.X + negativedrawOffset.X, (drawOffset.Y + negativedrawOffset.Y) / 2f);

                        weaponSize = drawOffset + negativedrawOffset.Abs();
                    }
                }

                if (!basePart.drawn)
                {
                    Asset<Texture2D> tex = ModContent.Request<Texture2D>(basePart.Texture);

                    spriteBatch.Draw(tex.Value,
                        (position - Main.screenPosition + holdoutoffset).Int(),
                        tex.Frame(),
                        Color.White,
                        rotation,
                        Vector2.Zero,
                        weapon.Item.scale,
                        flipped ? SpriteEffects.FlipVertically : SpriteEffects.None,
                        1f);
                    basePart.drawn = true;

                    drawOffset.X = MathHelper.Max(drawOffset.X, tex.Width());
                    drawOffset.Y = MathHelper.Max(drawOffset.Y, tex.Height());

                    realOffset = new Vector2(drawOffset.X + negativedrawOffset.X, (drawOffset.Y + negativedrawOffset.Y) / 2f);
                    weaponSize = drawOffset + negativedrawOffset.Abs();
                }
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, UI? Main.UIScaleMatrix : Main.GameViewMatrix.TransformationMatrix);
        }
    }
}
