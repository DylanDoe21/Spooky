using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.GameContent.Drawing;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles.SpiderCave
{
    public class MiteTomeLightning : ModProjectile
    {
        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 100;
            Projectile.timeLeft = 120;
            Projectile.scale = 0.25f;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 2;
            Projectile.aiStyle = -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Rectangle rectangle = ProjTexture.Value.Bounds;
            Vector2 origin2 = rectangle.Size() / 2f;
            for (int i = 1; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero || Projectile.oldPos[i - 1] == Projectile.oldPos[i])
                    continue;

                Color color = Color.Lerp(new Color(90, 0, 213), new Color(91, 35, 119), i / (float)Projectile.oldPos.Length) * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length);
                Vector2 offset = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
                int length = (int)offset.Length();
                float scale = Projectile.scale * (float)Math.Sin(i / MathHelper.Pi) * 2;
                offset.Normalize();
                const int step = 3;
                for (int j = 0; j < length; j += step)
                {
                    Vector2 value5 = Projectile.oldPos[i] + offset * j;
                    Main.EntitySpriteDraw(ProjTexture.Value, value5 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), rectangle, color, Projectile.rotation, origin2, scale, SpriteEffects.FlipHorizontally, 0);
                }
            }

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) 
		{
            ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.NightsEdge, new ParticleOrchestraSettings
            {
                PositionInWorld = target.Center
            });

            if (Main.rand.NextBool())
            {
                Vector2 ProjectilePosition = Projectile.Center + new Vector2(1, 0).RotatedByRandom(360);
                Vector2 Velocity = Projectile.Center - ProjectilePosition;
                Velocity.Normalize();
                Velocity *= 2f;

                int newMite = Projectile.NewProjectile(Projectile.GetSource_Death(), target.Center, Velocity, ModContent.ProjectileType<MiteProjectile>(), Projectile.damage / 2, 0, Projectile.owner);
                Main.projectile[newMite].DamageType = DamageClass.Magic;
                Main.projectile[newMite].ai[0] = Main.rand.Next(3, 5);
                Main.projectile[newMite].ai[2] = 0;
                Main.projectile[newMite].penetrate = 2;
            }
		}

		public override void AI()
        {
            Projectile.frameCounter = Projectile.frameCounter + 1;

            if (Projectile.frameCounter < Projectile.extraUpdates * 2)
            {
                return;
            }

            Projectile.frameCounter = 0;
            float num1 = Projectile.velocity.Length();
            UnifiedRandom unifiedRandom = new((int)Projectile.ai[1]);
            int num2 = 0;
            Vector2 spinningpoint = -Vector2.UnitY;
            Vector2 rotationVector2;
            int num3;

            do
            {
                int num4 = unifiedRandom.Next();
                Projectile.ai[1] = num4;
                rotationVector2 = ((float)(num4 % 100 / 100.0 * 6.28318548202515)).ToRotationVector2();
                if (rotationVector2.Y > 0.0)
                    rotationVector2.Y--;
                bool flag = false;
                if (rotationVector2.Y > -0.0199999995529652)
                    flag = true;
                if (rotationVector2.X * (double)(Projectile.extraUpdates + 1) * 2.0 * num1 + Projectile.localAI[0] > 40.0)
                    flag = true;
                if (rotationVector2.X * (double)(Projectile.extraUpdates + 1) * 2.0 * num1 + Projectile.localAI[0] < -40.0)
                    flag = true;
                if (flag)
                {
                    num3 = num2;
                    num2 = num3 + 1;
                }
                else
                    goto label_3460;
            }

            while (num3 < 100);
            Projectile.velocity = Vector2.Zero;
            Projectile.localAI[1] = 1f;
            goto label_3461;
        label_3460:
            spinningpoint = rotationVector2;
        label_3461:

            if (Projectile.velocity == Vector2.Zero || Projectile.velocity.Length() < 4f)
            {
                Projectile.velocity = Vector2.UnitX.RotatedBy(Projectile.ai[0]).RotatedByRandom(Math.PI / 4) * 25;
                Projectile.ai[1] = Main.rand.Next(100);
                
                return;
            }

            Projectile.localAI[0] += (float)(spinningpoint.X * (double)(Projectile.extraUpdates + 1) * 2.0) * num1;
            Projectile.velocity = spinningpoint.RotatedBy(Projectile.ai[0] + 1.57079637050629, new Vector2()) * num1;
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.570796f;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            for (int index = 0; index < Projectile.oldPos.Length && (Projectile.oldPos[index].X != 0.0 || Projectile.oldPos[index].Y != 0.0); ++index)
            {
                Rectangle myRect = projHitbox;
                myRect.X = (int)Projectile.oldPos[index].X;
                myRect.Y = (int)Projectile.oldPos[index].Y;

                if (myRect.Intersects(targetHitbox))
                {
                    return true;
                }
            }

            return false;
        }
    }
}