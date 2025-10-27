using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Minibiomes.Christmas
{
    public class SnakeHead : ModProjectile
    {
        bool SpawnedSegment = false;

        Vector2 RotatePosition;

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 16;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true; 
            Projectile.tileCollide = false;
            Projectile.timeLeft = 240;
            Projectile.penetrate = 3;
            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;
            
            if (!SpawnedSegment && Projectile.ai[0] >= 2)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), new Vector2((int)Projectile.Center.X + (Projectile.width / 2), (int)Projectile.Center.Y + (Projectile.height / 2)), 
                Vector2.Zero, ModContent.ProjectileType<SnakeBody>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai0: Projectile.whoAmI, ai1: 1);

                SpawnedSegment = true;
            }

            if (Projectile.timeLeft <= 60)
            {
                Projectile.alpha += 5;
            }

            Projectile.ai[0]++;
            if (Projectile.ai[0] == 1)
            {
                Projectile.localAI[0] = Main.rand.Next(35, 120);
            }
            if (Projectile.ai[0] == Projectile.localAI[0])
            {
                RotatePosition = new Vector2(Projectile.Center.X, Projectile.Center.Y + (Main.rand.NextBool() ? -15 : 15));
            }
            if (Projectile.ai[0] > Projectile.localAI[0])
            {
                double angle = Projectile.DirectionTo(RotatePosition).ToRotation() - Projectile.velocity.ToRotation();
                while (angle > Math.PI)
                {
                    angle -= 2.0 * Math.PI;
                }
                while (angle < -Math.PI)
                {
                    angle += 2.0 * Math.PI;
                }

                if (Math.Abs(angle) > Math.PI / 2)
                {
                    Projectile.localAI[1] = Math.Sign(angle);
                    Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 8;
                }

                Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(6f) * Projectile.localAI[1]);

                if (Projectile.ai[0] >= Projectile.localAI[0] + 60)
                {
                    Projectile.ai[0] = 28;
                }
            }

            if (Projectile.ai[0] < Projectile.localAI[0] && Projectile.ai[0] >= 28)
            {
                float WaveIntensity = 8f;
                float Wave = 10f;

                Projectile.ai[1]++;
                if (Projectile.ai[2] == 0)
                {
                    if (Projectile.ai[1] > Wave * 0.5f)
                    {
                        Projectile.ai[1] = 0;
                        Projectile.ai[2] = 1;
                    }
                    else
                    {
                        Vector2 perturbedSpeed = new Vector2(Projectile.velocity.X, Projectile.velocity.Y).RotatedBy(MathHelper.ToRadians(-WaveIntensity));
                        Projectile.velocity = perturbedSpeed;
                    }
                }
                else
                {
                    if (Projectile.ai[1] <= Wave)
                    {
                        Vector2 perturbedSpeed = new Vector2(Projectile.velocity.X, Projectile.velocity.Y).RotatedBy(MathHelper.ToRadians(WaveIntensity));
                        Projectile.velocity = perturbedSpeed;
                    }
                    else
                    {
                        Vector2 perturbedSpeed = new Vector2(Projectile.velocity.X, Projectile.velocity.Y).RotatedBy(MathHelper.ToRadians(-WaveIntensity));
                        Projectile.velocity = perturbedSpeed;
                    }
                    if (Projectile.ai[1] >= Wave * 2)
                    {
                        Projectile.ai[1] = 0;
                    }
                }
            }
        }

        public override void OnKill(int timeLeft)
		{
            foreach (var Proj in Main.ActiveProjectiles)
			{
                if ((Proj.type == ModContent.ProjectileType<SnakeBody>() || Proj.type == ModContent.ProjectileType<SnakeTail>()) && Proj.ai[0] == Projectile.whoAmI)
                {
                    Proj.Kill();
                }
            }
        }
    }
}