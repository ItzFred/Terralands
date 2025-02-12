using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using ReLogic.Content;
using Terralands.Systems.WeaponSystem.WeaponParts.Gun.Grip;
using Terralands.Systems.WeaponSystem.WeaponParts.Gun.Body;
using System.Linq;
using Terralands.Systems.WeaponSystem.WeaponParts.Gun.Barrel;
using Terralands.Systems.WeaponSystem.WeaponParts.Gun.Magazine;
using Terralands.Systems.WeaponSystem.WeaponParts.Gun.Stock;
using System;
using Terraria.DataStructures;
using Microsoft.CodeAnalysis;
using Terraria.Graphics.Shaders;
using static System.Net.Mime.MediaTypeNames;
using Terralands.Systems.WeaponSystem.WeaponMaterials;
using Terraria.ModLoader.IO;

namespace Terralands.Systems.WeaponSystem
{
    internal class AssembledWeapon : ModItem
    {
        public override string Texture => $"Terraria/Images/Item_{ItemID.CopperShortsword}";

        public WeaponPart[] weaponParts = new WeaponPart[] {
            new VladofRifleBody(),
            new DahlBarrel(),
            new RoundGrip(),
            new MilitaryStock(),
            new TestGunMagazine()
        };

        public WeaponMaterial weaponMaterial1 = new Rust();
        public WeaponMaterial weaponMaterial2 = new Rust();
        public WeaponMaterial weaponMaterial3 = new Rust();

        public Vector2 drawOffset = Vector2.Zero;
        public Vector2 negativedrawOffset = Vector2.Zero;
        public Vector2 realOffset = Vector2.Zero;

        public Vector2 weaponSize = Vector2.Zero;
        public float properScale = 1f;

        public Vector2 holdoutOffset = Vector2.Zero;
        public Vector2 shootPosition = Vector2.Zero;

        public override void SaveData(TagCompound tag)
        {
            tag["WeaponMaterial1"] = WeaponMaterialID.GetType(weaponMaterial1);
            tag["WeaponMaterial2"] = WeaponMaterialID.GetType(weaponMaterial2);
            tag["WeaponMaterial3"] = WeaponMaterialID.GetType(weaponMaterial3);

            int[] weaponPartTypes = new int[] { };
            for (int i = 0; i < weaponParts.Length; i++)
            {
                weaponPartTypes = weaponPartTypes.Append(WeaponPartMadeID.GetType(weaponParts[i])).ToArray();
            }
            tag["WeaponParts"] = weaponPartTypes;
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey("WeaponMaterial1")) weaponMaterial1 = WeaponMaterialID.IDs[tag.Get<int>("WeaponMaterial1")];
            if (tag.ContainsKey("WeaponMaterial2")) weaponMaterial2 = WeaponMaterialID.IDs[tag.Get<int>("WeaponMaterial2")];
            if (tag.ContainsKey("WeaponMaterial3")) weaponMaterial3 = WeaponMaterialID.IDs[tag.Get<int>("WeaponMaterial3")];

            if (tag.ContainsKey("WeaponParts"))
            {
                int[] loadedWeaponPartTypes = tag.Get<int[]>("WeaponParts");
                weaponParts = new WeaponPart[] { };
                for (int i = 0; i < loadedWeaponPartTypes.Length; i++)
                {
                    weaponParts = weaponParts.Append(WeaponPartMadeID.IDs[loadedWeaponPartTypes[i]]).ToArray();
                };
            }
        }
        public override void SetDefaults()
        {
            holdoutOffset = new Vector2(0, -12);
            Item.damage = 50;
            Item.useTime = 14;
            Item.useAnimation = 14;
            Item.scale = 1f;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 10f;
            Item.noUseGraphic = true;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {           
            Item.Size = weaponSize;
            base.Update(ref gravity, ref maxFallSpeed);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectileDirect(source, position + (shootPosition * -player.direction).RotatedBy(velocity.ToRotation()), velocity, type, damage, knockback, player.whoAmI);

            Projectile p = Projectile.NewProjectileDirect(source, position, velocity, ModContent.ProjectileType<AssembledWeaponHeldProjectile>(), 0, 0, player.whoAmI);
            if (p.ModProjectile is AssembledWeaponHeldProjectile heldproj)
            {
                heldproj.weaponMaterial1 = weaponMaterial1;
                heldproj.weaponMaterial2 = weaponMaterial2;
                heldproj.weaponMaterial3 = weaponMaterial3;
                heldproj.weaponParts = weaponParts;
                heldproj.holdoutOffset = holdoutOffset;
            }

            return false;
        }
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            spriteBatch.End();

            Effect weaponShader = GameShaders.Misc["WeaponSkinShader"].Shader;
            weaponShader.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects);

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

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, DepthStencilState.None, RasterizerState.CullCounterClockwise, weaponShader, Main.UIScaleMatrix);

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
                        Vector2? connectingPoint = Utils.FindConnectingPoint(basePart, connectingPart.partType) == null? Vector2.Zero : Utils.FindConnectingPoint(basePart, connectingPart.partType);
                        Vector2? basePartOffset = Utils.FindConnectingPoint(connectingPart, basePart.partType) == null ? Vector2.Zero : Utils.FindConnectingPoint(connectingPart, basePart.partType);

                        Asset<Texture2D> tex2 = ModContent.Request<Texture2D>(connectingPart.Texture);
                        weaponShader.Parameters["materialMap"].SetValue(ModContent.Request<Texture2D>(connectingPart.MaterialTexture).Value);
                        weaponShader.Parameters["textureSize"].SetValue(tex2.Size());

                        spriteBatch.Draw(tex2.Value,
                            position + ((Vector2)connectingPoint - (Vector2)basePartOffset - (realOffset / 2f)) * scale * properScale,
                            tex2.Frame(),
                            Color.White,
                            0f,
                            Vector2.Zero,
                            scale * properScale,
                            SpriteEffects.None,
                            1f);
                        connectingPart.drawn = true;

                        weaponShader.Parameters["uWorldPosition"].SetValue(position + ((Vector2)connectingPoint - (Vector2)basePartOffset - (realOffset / 2f)) * scale * properScale);

                        Vector2 pos = (Vector2)connectingPoint - (Vector2)basePartOffset;
                        if (pos.X > 0) drawOffset.X = MathHelper.Max(drawOffset.X, pos.X + tex2.Width());
                        if (pos.Y > 0) drawOffset.Y = MathHelper.Max(drawOffset.Y, pos.Y + tex2.Height());

                        if (pos.X < 0) negativedrawOffset.X = MathHelper.Min(negativedrawOffset.X, pos.X);
                        if (pos.Y < 0) negativedrawOffset.Y = MathHelper.Min(negativedrawOffset.Y, pos.Y);

                        realOffset = new Vector2(drawOffset.X + negativedrawOffset.X, (drawOffset.Y + negativedrawOffset.Y) /2f);

                        weaponSize = drawOffset + negativedrawOffset.Abs();

                        Vector2 setPosition = (Item.position + ((Vector2)connectingPoint - (Vector2)basePartOffset) * scale);
                        Vector2 distance = Item.position - setPosition;

                        if (connectingPart.partType == WeaponPartID.gunParts.grip)
                        {

                            holdoutOffset = distance;
                        }

                        if (connectingPart.partType == WeaponPartID.gunParts.barrel)
                        {
                            shootPosition = connectingPoint.Value;
                        }
                    }
                }

                if (!basePart.drawn)
                {
                    Asset<Texture2D> tex = ModContent.Request<Texture2D>(basePart.Texture);

                    weaponShader.Parameters["materialMap"].SetValue(ModContent.Request<Texture2D>(basePart.MaterialTexture).Value);
                    weaponShader.Parameters["textureSize"].SetValue(tex.Size());

                    spriteBatch.Draw(tex.Value,
                        position - (realOffset / 2f) * scale * properScale,
                        tex.Frame(),
                        Color.White,
                        0f,
                        Vector2.Zero,
                        scale * properScale,
                        SpriteEffects.None,
                        1f);
                    basePart.drawn = true;

                    weaponShader.Parameters["uWorldPosition"].SetValue(position - (realOffset / 2f) * scale * properScale);

                    drawOffset.X = MathHelper.Max(drawOffset.X, tex.Width());
                    drawOffset.Y = MathHelper.Max(drawOffset.Y, tex.Height());

                    realOffset = new Vector2(drawOffset.X + negativedrawOffset.X, (drawOffset.Y + negativedrawOffset.Y) / 2f);

                    weaponSize = drawOffset + negativedrawOffset.Abs();
                }
            }

            float biggestsize = MathHelper.Max(weaponSize.X, weaponSize.Y);
            properScale = MathHelper.Clamp( 40 / biggestsize, 0.2f, 1f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            spriteBatch.End();

            Effect weaponShader = GameShaders.Misc["WeaponSkinShader"].Shader;
            weaponShader.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects);

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

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, weaponShader, Main.GameViewMatrix.TransformationMatrix);

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
                        Vector2? connectingPoint = Utils.FindConnectingPoint(basePart, connectingPart.partType) == null ? Vector2.Zero : Utils.FindConnectingPoint(basePart, connectingPart.partType);
                        Vector2? basePartOffset = Utils.FindConnectingPoint(connectingPart, basePart.partType) == null ? Vector2.Zero : Utils.FindConnectingPoint(connectingPart, basePart.partType);

                        Vector2 setPosition = (Item.position + ((Vector2)connectingPoint - (Vector2)basePartOffset) * scale);
                        Vector2 distance = Item.position - setPosition;

                        Asset<Texture2D> tex2 = ModContent.Request<Texture2D>(connectingPart.Texture);
                        weaponShader.Parameters["materialMap"].SetValue(ModContent.Request<Texture2D>(connectingPart.MaterialTexture).Value);
                        weaponShader.Parameters["textureSize"].SetValue(tex2.Size());

                        spriteBatch.Draw(tex2.Value,
                            (Item.position + ((Vector2)connectingPoint - (Vector2)basePartOffset) * scale) - Main.screenPosition + distance - negativedrawOffset,
                            tex2.Frame(),
                            Color.White,
                            rotation,
                            distance,
                            scale,
                            SpriteEffects.None,
                            1f);
                        connectingPart.drawn = true;

                        weaponShader.Parameters["uWorldPosition"].SetValue(Item.position + ((connectingPoint.Value - basePartOffset.Value) * scale) - Main.screenPosition + distance - negativedrawOffset);
                    }

                }

                if (!basePart.drawn)
                {
                    Asset<Texture2D> tex = ModContent.Request<Texture2D>(basePart.Texture);

                    weaponShader.Parameters["materialMap"].SetValue(ModContent.Request<Texture2D>(basePart.MaterialTexture).Value);
                    weaponShader.Parameters["textureSize"].SetValue(tex.Size());

                    spriteBatch.Draw(tex.Value,
                        Item.position - Main.screenPosition - negativedrawOffset,
                        tex.Frame(),
                        Color.White,
                        rotation,
                        Vector2.Zero,
                        scale,
                        SpriteEffects.None,
                        1f);
                    basePart.drawn = true;

                    weaponShader.Parameters["uWorldPosition"].SetValue(Item.position - Main.screenPosition - negativedrawOffset);
                }
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}
