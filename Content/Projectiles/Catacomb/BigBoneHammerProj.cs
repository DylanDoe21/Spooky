using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Catacomb
{
    public class BigBoneHammerProj : ModProjectile
    {
        public float Speed = 0.02f;
        public float TrailSize = 0;

        private List<Vector2> cache;
        private Trail trail;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Skull Smasher");
            Main.projFrames[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 82;
            Projectile.height = 82;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.netImportant = true;
            Projectile.ownerHitCheck = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 10000;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player owner = Main.player[Projectile.owner];

            if (owner.direction == 1)
            {
                Projectile.frame = 0;
            }
            else
            {
                Projectile.frame = 1;
            }

            Main.spriteBatch.End();
            Effect effect = ShaderLoader.GlowyTrail;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("Spooky/ShaderAssets/LightningTrail").Value); //trails texture image
            effect.Parameters["time"].SetValue((float)Main.timeForVisualEffects * 0.05f); //this affects something?
            effect.Parameters["repeats"].SetValue(1); //this is how many times the trail is drawn

            trail?.Render(effect);

            Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);

            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            int frameHeight = tex.Height / Main.projFrames[Projectile.type];
            Rectangle frame = new Rectangle(0, frameHeight * Projectile.frame, tex.Width, frameHeight);

            Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, Projectile.Center - new Vector2(0, 0) - Main.screenPosition, frame, 
            lightColor, Projectile.rotation, new Vector2(0, frameHeight), Projectile.scale, SpriteEffects.None, 0f);

            return false;
        }

        const int TrailLength = 12;

        private void ManageCaches()
        {
            Vector2 offset = (Projectile.rotation - 0.78f).ToRotationVector2() * Projectile.width;

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
                //use (* 1 - factor.X) at the end to make it fade at the beginning, or use (* factor.X) at the end to make it fade at the end
                return Color.Lerp(Color.Black, Color.Yellow, factor.X) * factor.X;
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center + Projectile.velocity;
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

            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1f;
                Projectile.rotation -= owner.direction == 1 ? MathHelper.PiOver2 : 0f;
            }

            if (Main.mouseRight)
            {
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

                Projectile.timeLeft = 10000;

                Projectile.localAI[0]++;

                if (Projectile.localAI[0] < 120)
                {
                    Speed += 0.003f;
                    TrailSize += 0.12f;
                }

                if (Projectile.localAI[0] == 120)
                {
                    SoundEngine.PlaySound(SoundID.Item35, Projectile.Center);
                }

                SetProjectilePosition(owner);

                SetOwnerAnimation(owner);
            }

            if (Projectile.localAI[0] >= 120 && Main.mouseRightRelease)
            {
                SoundEngine.PlaySound(SoundID.Item84, Projectile.Center);

                Vector2 ShootSpeed = Main.MouseWorld - Projectile.Center;
                ShootSpeed.Normalize();

                ShootSpeed.X *= 55;
                ShootSpeed.Y *= 55;
                        
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, 
                ShootSpeed.X, ShootSpeed.Y, ModContent.ProjectileType<BigBoneHammerProj2>(), Projectile.damage, 12f, Main.myPlayer, 0f, 0f);

                Projectile.Kill();
            }

            //kill projectile if you release before its charged
            if (Projectile.localAI[0] > 2 && Projectile.localAI[0] < 120 && Main.mouseRightRelease)
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