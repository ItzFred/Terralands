using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terralands.Systems.WeaponSystem.WeaponMaterials;
using Terralands.Systems.WeaponSystem.WeaponParts.Gun.Barrel;
using Terralands.Systems.WeaponSystem.WeaponParts.Gun.Body;
using Terralands.Systems.WeaponSystem.WeaponParts.Gun.Grip;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using static System.Net.Mime.MediaTypeNames;

namespace Terralands.Systems.WeaponSystem
{
    internal class AssembledWeaponHeldProjectile : ModProjectile
    {

        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.timeLeft = 10000;
        }

        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.Ale}";

        public override bool ShouldUpdatePosition() => false;
        public override bool? CanHitNPC(NPC target) => false;
        public override bool CanHitPlayer(Player target) => false;
        public override bool CanHitPvp(Player target) => false;

        Player player;
        public override void AI()
        {
            player = Main.player[Projectile.owner];
            player.heldProj = Projectile.whoAmI;
            Projectile.Center = player.itemLocation.ToPoint().ToVector2();
            Projectile.rotation = player.itemRotation;
            Projectile.scale = player.HeldItem.scale;

            if (player.ItemAnimationEndingOrEnded) Projectile.Kill();
        }

        public WeaponPart[] weaponParts = new WeaponPart[] {
            new VladofPistolBody(),
            new BlockBarrel(),
            new RoundGrip(),
        };

        public WeaponMaterial weaponMaterial1 = new Rust();
        public WeaponMaterial weaponMaterial2 = new GratedMetal();
        public WeaponMaterial weaponMaterial3 = new Rust();

        public Vector2 drawOffset = Vector2.Zero;
        public Vector2 negativedrawOffset = Vector2.Zero;
        public Vector2 realOffset = Vector2.Zero;

        public Vector2 weaponSize = Vector2.Zero;
        public float properScale = 1f;

        public Vector2 holdoutOffset = Vector2.Zero;

        public override bool PreDraw(ref Color lightColor)
        {

            Main.spriteBatch.End();

            Effect weaponShader = GameShaders.Misc["WeaponSkinShader"].Shader;
            weaponShader.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects);
            weaponShader.Parameters["uWorldPosition"].SetValue(Projectile.position);

            Texture2D weaponMaterialTexture1 = ModContent.Request<Texture2D>(weaponMaterial1.Texture).Value;
            weaponShader.Parameters["materialTexture"].SetValue(weaponMaterialTexture1);
            weaponShader.Parameters["materialTextureSize"].SetValue(weaponMaterialTexture1.Size());
            weaponShader.Parameters["TileHorizontal1"].SetValue(weaponMaterial1.TileHorizontal);
            Texture2D weaponMaterialTexture2 = ModContent.Request<Texture2D>(weaponMaterial2.Texture).Value;
            weaponShader.Parameters["materialTexture2"].SetValue(weaponMaterialTexture2);
            weaponShader.Parameters["materialTexture2Size"].SetValue(weaponMaterialTexture2.Size());
            weaponShader.Parameters["TileHorizontal2"].SetValue(weaponMaterial2.TileHorizontal);
            Texture2D weaponMaterialTexture3 = ModContent.Request<Texture2D>(weaponMaterial3.Texture).Value;
            weaponShader.Parameters["materialTexture3"].SetValue(weaponMaterialTexture3);
            weaponShader.Parameters["materialTexture3Size"].SetValue(weaponMaterialTexture3.Size());
            weaponShader.Parameters["TileHorizontal3"].SetValue(weaponMaterial3.TileHorizontal);

            weaponShader.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, weaponShader, Main.GameViewMatrix.TransformationMatrix);
            
            float rotation = player.direction == 1 ? player.itemRotation : player.itemRotation + MathHelper.Pi;
            Vector2 holdoutoffset = new Vector2(holdoutOffset.X, player.direction == 1 ? holdoutOffset.Y : 0).RotatedBy(rotation);

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

                        Vector2? connectingPoint = Utils.FindConnectingPoint(basePart, connectingPart.partType) == null ? Vector2.Zero : Utils.FindConnectingPoint(basePart, connectingPart.partType) * player.direction;
                        Vector2? basePartOffset = Utils.FindConnectingPoint(connectingPart, basePart.partType) == null ? Vector2.Zero : Utils.FindConnectingPoint(connectingPart, basePart.partType) * player.direction;
                        connectingPoint = player.direction == 1 ? connectingPoint : new Vector2(connectingPoint.Value.X, connectingPoint.Value.Y - tex2.Height());
                        basePartOffset = player.direction == 1 ? basePartOffset : new Vector2(basePartOffset.Value.X, basePartOffset.Value.Y - tex.Height());

                        Vector2 partOffset = new Vector2(connectingPoint.Value.X - basePartOffset.Value.X, connectingPoint.Value.Y - basePartOffset.Value.Y) * player.direction * Projectile.scale;
                        Vector2 setPosition = (Projectile.Center + partOffset);
                        Vector2 distance = Projectile.Center - setPosition;

                        Main.spriteBatch.Draw(tex2.Value,
                            (player.MountedCenter.ToPoint().ToVector2() - Main.screenPosition + partOffset + distance + holdoutoffset),
                            tex2.Frame(),
                            Color.White,
                            rotation,
                            new Vector2(distance.X / Projectile.scale, distance.Y * player.direction / Projectile.scale),
                            Projectile.scale,
                            player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically,
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

                    weaponShader.Parameters["materialMap"].SetValue(ModContent.Request<Texture2D>(basePart.MaterialTexture).Value);
                    weaponShader.Parameters["textureSize"].SetValue(tex.Size());

                    Main.spriteBatch.Draw(tex.Value,
                        player.MountedCenter.ToPoint().ToVector2() - Main.screenPosition + holdoutoffset,
                        tex.Frame(),
                        Color.White,
                        rotation,
                        Vector2.Zero,
                        Projectile.scale,
                        player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically,
                        1f);
                    basePart.drawn = true;

                    drawOffset.X = MathHelper.Max(drawOffset.X, tex.Width());
                    drawOffset.Y = MathHelper.Max(drawOffset.Y, tex.Height());

                    realOffset = new Vector2(drawOffset.X + negativedrawOffset.X, (drawOffset.Y + negativedrawOffset.Y) / 2f);
                    weaponSize = drawOffset + negativedrawOffset.Abs();
                }
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}
