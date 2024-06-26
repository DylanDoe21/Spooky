﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Catacomb
{
    public class FemurFractureSwung : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/Catacomb/FemurFractureProj";

        public float Speed = 0.02f;
        public float TrailSize = 0;

        float SaveKnockback;
        bool SavedKnockback = false;

        private List<Vector2> cache;
        private Trail trail;

        private static Asset<Texture2D> ProjTexture;

        public override void SetDefaults()
        {
            Projectile.width = 120;
            Projectile.height = 120;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.netImportant = true;
            Projectile.ownerHitCheck = true;
            Projectile.tileCollide = false;
            Projectile.knockBack = 0;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 10000;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[0] > 10)
            {
                Main.spriteBatch.End();
                Effect effect = ShaderLoader.GlowyTrail;

                Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
                Matrix view = Main.GameViewMatrix.ZoomMatrix;
                Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

                effect.Parameters["transformMatrix"].SetValue(world * view * projection);
                effect.Parameters["sampleTexture"].SetValue(ShaderLoader.ShadowTrail.Value);
                effect.Parameters["time"].SetValue((float)Main.timeForVisualEffects * 0.05f);
                effect.Parameters["repeats"].SetValue(1);

                trail?.Render(effect);

                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            }

            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            int frameHeight = ProjTexture.Height() / Main.projFrames[Projectile.type];
            Rectangle frame = new Rectangle(0, frameHeight * Projectile.frame, ProjTexture.Width(), frameHeight);

            Main.spriteBatch.Draw(ProjTexture.Value, Projectile.Center - new Vector2(0, 0) - Main.screenPosition, frame, 
            lightColor, Projectile.rotation, new Vector2(0, frameHeight), Projectile.scale, SpriteEffects.None, 0f);

            return false;
        }

        const int TrailLength = 12;

        private void ManageCaches()
        {
            Vector2 offset = (Projectile.rotation - 0.78f).ToRotationVector2() * (Projectile.width - 48);

            if (cache == null)
            {
                cache = new List<Vector2>();
                for (int i = 0; i < TrailLength; i++)
                {
                    cache.Add(Projectile.Center + offset);
                }
            }

            cache.Add(Projectile.Center + offset);

            while (cache.Count > TrailLength)
            {
                cache.RemoveAt(0);
            }
        }

        private void ManageTrail()
        {
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, TrailLength, new TriangularTip(4), factor => TrailSize * factor, factor =>
            {
                return Color.Lerp(Color.DarkGray, (Projectile.ai[0] >= 240 ? Color.Lime : Color.Gray), factor.X) * factor.X;
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center + Projectile.velocity;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player owner = Main.player[Projectile.owner];

            //since this projectile is weird and only knocks enemies back in one direction, manually handle knockback here
            Vector2 Knockback = owner.Center - target.Center;
            Knockback.Normalize();
            Knockback *= SaveKnockback * 2;

            if (target.knockBackResist > 0f)
            {
                target.velocity = -Knockback * target.knockBackResist;
            }
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            if (!Main.dedServ)
            {
                ManageCaches();
                ManageTrail();
            }

            if (!owner.active || owner.dead)
            {
                Projectile.Kill();
            }

            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[0] = 1f;
                Projectile.rotation -= owner.direction == 1 ? MathHelper.PiOver2 : 0f;
            }

            if (!SavedKnockback)
            {
                SaveKnockback = Projectile.knockBack;
                SavedKnockback = true;
            }
            else
            {
                Projectile.knockBack = 0;
            }

            if (Main.mouseLeft)
            {
                //set time left super high since this projectile will always die manually
                Projectile.timeLeft = 10000;

                //set the player arm and projectile rotation depending on which direction you're facing
                if (owner.direction == 1)
                {
                    owner.itemRotation = Projectile.rotation + 2.14f;
                    owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, owner.itemRotation + 2.14f);
                }
                else
                {
                    owner.itemRotation = Projectile.rotation + 2.14f;
                    owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, owner.itemRotation + 2.14f);
                }

                Projectile.ai[0]++;

                //increase swing speed and the trail size as you swing it
                if (Projectile.ai[0] < 120)
                {
                    Speed += 0.0025f;
                    TrailSize += 0.08f;
                }

                //play sound when fully charged
                if (Projectile.ai[0] == 120)
                {
                    SoundEngine.PlaySound(SoundID.DD2_DarkMageHealImpact with { Volume = SoundID.DD2_DarkMageHealImpact.Volume * 100f }, Projectile.Center);
                }

                //play different sound when super charged
                if (Projectile.ai[0] == 240)
                {
                    TrailSize += 2f;

                    SoundEngine.PlaySound(SoundID.DD2_DarkMageSummonSkeleton with { Volume = SoundID.DD2_DarkMageSummonSkeleton.Volume * 3.5f }, Projectile.Center);
                }

                SetProjectilePosition(owner);

                SetOwnerAnimation(owner);
            }

            //when you release right click when the hammer is charged, throw it
            if (Projectile.ai[0] >= 120 && Projectile.ai[0] < 240 && Main.mouseLeftRelease)
            {
                SoundEngine.PlaySound(SoundID.Item84, Projectile.Center);

                Vector2 ShootSpeed = Main.MouseWorld - Projectile.Center;
                ShootSpeed.Normalize();
                ShootSpeed *= 45;
                        
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, ShootSpeed, ModContent.ProjectileType<FemurFractureProj>(), Projectile.damage, 12f, Main.myPlayer, 0f, 0f);

                Projectile.Kill();
            }

            //throw the super charged hammer, but faster and set its ai to shoot skulls
            if (Projectile.ai[0] >= 240 && Main.mouseLeftRelease)
            {
                SoundEngine.PlaySound(SoundID.Item84, Projectile.Center);

                Vector2 ShootSpeed = Main.MouseWorld - Projectile.Center;
                ShootSpeed.Normalize();
                ShootSpeed *= 55;
                        
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, ShootSpeed, ModContent.ProjectileType<FemurFractureProj>(), Projectile.damage, 12f, Main.myPlayer, 0f, 1f);

                Projectile.Kill();
            }

            //kill this projectile if you release right click before its charged
            if (Projectile.ai[0] > 2 && Projectile.ai[0] < 120 && Main.mouseLeftRelease)
            {
                Projectile.Kill();
            }
        }

        public void SetProjectilePosition(Player owner)
        {
            Vector2 rotatedPoint = owner.RotatedRelativePoint(owner.MountedCenter);

            float animationModifier = (float)owner.itemAnimation / owner.itemAnimationMax * 2f - 1f;

            Projectile.rotation -= animationModifier * Speed * owner.direction;

            Projectile.velocity *= 0f;

            Projectile.Center = owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, owner.itemRotation + 3.14f);
        }

        private void SetOwnerAnimation(Player owner)
        {
            owner.heldProj = Projectile.whoAmI;

            float animationModifier = (float)owner.itemAnimation / owner.itemAnimationMax * 2f - 1f;

            owner.itemRotation = (Math.Abs(animationModifier) - 0.5f) * -owner.direction * 3.5f - owner.direction * 0.3f;
        }
	}
}