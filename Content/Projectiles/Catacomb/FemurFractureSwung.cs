using Terraria;
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

        bool runOnce = true;
		Vector2[] trailLength = new Vector2[12];

        private static Asset<Texture2D> ProjTexture;
        private static Asset<Texture2D> TrailTexture;

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
            if (runOnce)
			{
				return false;
			}

            if (Projectile.ai[0] > 15)
            {
                TrailTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/TrailCircle");

                Vector2 drawOrigin = new Vector2(TrailTexture.Width() * 0.5f, TrailTexture.Height() * 0.5f);
                Vector2 offset = (Projectile.rotation - 0.78f).ToRotationVector2() * (Projectile.width - 50);
                Vector2 previousPosition = Projectile.Center + offset;

                for (int k = 0; k < trailLength.Length; k++)
                {
                    float scale = Projectile.scale * (trailLength.Length - k) / (float)trailLength.Length;
                    scale *= 1f + TrailSize;

                    Color color = Color.Lerp(Color.DarkGray, (Projectile.ai[0] >= 240 ? Color.Lime : Color.Gray), scale);

                    if (trailLength[k] == Vector2.Zero)
                    {
                        return false;
                    }

                    Vector2 drawPos = trailLength[k] - Main.screenPosition;
                    Vector2 currentPos = trailLength[k];
                    Vector2 betweenPositions = previousPosition - currentPos;

                    float max = betweenPositions.Length() / (4 * scale);

                    for (int i = 0; i < max; i++)
                    {
                        drawPos = previousPosition + -betweenPositions * (i / max) - Main.screenPosition;

                        //gives the projectile after images a shaking effect
                        float x = Main.rand.Next(-1, 2) * scale;
                        float y = Main.rand.Next(-1, 2) * scale;

                        Main.spriteBatch.Draw(TrailTexture.Value, drawPos + new Vector2(x, y), null, color, Projectile.rotation, drawOrigin, scale * 0.45f, SpriteEffects.None, 0f);
                    }

                    previousPosition = currentPos;
                }
            }

            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            int frameHeight = ProjTexture.Height() / Main.projFrames[Projectile.type];
            Rectangle frame = new Rectangle(0, frameHeight * Projectile.frame, ProjTexture.Width(), frameHeight);

            Main.spriteBatch.Draw(ProjTexture.Value, Projectile.Center - Main.screenPosition, frame, lightColor, Projectile.rotation, new Vector2(0, frameHeight), Projectile.scale, SpriteEffects.None, 0f);

            return false;
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

            if (runOnce)
			{
				for (int i = 0; i < trailLength.Length; i++)
				{
					trailLength[i] = Vector2.Zero;
				}
				runOnce = false;
			}

            Vector2 offset = (Projectile.rotation - 0.78f).ToRotationVector2() * (Projectile.width - 50);
			Vector2 current = Projectile.Center + offset;
			for (int i = 0; i < trailLength.Length; i++)
			{
				Vector2 previousPosition = trailLength[i];
				trailLength[i] = current;
				current = previousPosition;
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
                    TrailSize += 0.005f;
                }

                //play sound when fully charged
                if (Projectile.ai[0] == 120)
                {
                    SoundEngine.PlaySound(SoundID.DD2_DarkMageHealImpact with { Volume = SoundID.DD2_DarkMageHealImpact.Volume * 100f }, Projectile.Center);
                }

                //play different sound when super charged
                if (Projectile.ai[0] == 240)
                {
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