using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Slingshots
{
    public class EyeWormHead1 : ModProjectile
    {
        bool SpawnedSegment = false;

        Vector2 RotatePosition;

        public override void SetStaticDefaults()
        {
            ProjectileGlobal.IsSlingshotAmmoProj[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 16;
            Projectile.DamageType = DamageClass.Ranged;
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
            
            if (!SpawnedSegment)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), new Vector2((int)Projectile.Center.X + (Projectile.width / 2), (int)Projectile.Center.Y + (Projectile.height / 2)), 
                Vector2.Zero, ModContent.ProjectileType<EyeWormBody>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai0: Projectile.whoAmI, ai1: 1);

                SpawnedSegment = true;
            }

            if (Projectile.timeLeft <= 60)
            {
                Projectile.alpha += 5;
            }

            float WaveIntensity = 10f;
            float Wave = 8f;

            Projectile.ai[0]++;
            if (Projectile.ai[1] == 0)
            {
                if (Projectile.ai[0] > Wave * 0.5f)
                {
                    Projectile.ai[0] = 0;
                    Projectile.ai[1] = 1;
                }
                else
                {
                    Vector2 perturbedSpeed = new Vector2(Projectile.velocity.X, Projectile.velocity.Y).RotatedBy(MathHelper.ToRadians(-WaveIntensity));
                    Projectile.velocity = perturbedSpeed;
                }
            }
            else
            {
                if (Projectile.ai[0] <= Wave)
                {
                    Vector2 perturbedSpeed = new Vector2(Projectile.velocity.X, Projectile.velocity.Y).RotatedBy(MathHelper.ToRadians(WaveIntensity));
                    Projectile.velocity = perturbedSpeed;
                }
                else
                {
                    Vector2 perturbedSpeed = new Vector2(Projectile.velocity.X, Projectile.velocity.Y).RotatedBy(MathHelper.ToRadians(-WaveIntensity));
                    Projectile.velocity = perturbedSpeed;
                }
                if (Projectile.ai[0] >= Wave * 2)
                {
                    Projectile.ai[0] = 0;
                }
            }
        }

        public override void OnKill(int timeLeft)
		{
            foreach (var Proj in Main.ActiveProjectiles)
			{
                if ((Proj.type == ModContent.ProjectileType<EyeWormBody>() || Proj.type == ModContent.ProjectileType<EyeWormTail>()) && Proj.ai[0] == Projectile.whoAmI)
                {
                    Proj.Kill();
                }
            }
        }
    }

    public class EyeWormHead2 : EyeWormHead1
    {
        bool SpawnedSegment = false;

        Vector2 RotatePosition;

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;
            
            if (!SpawnedSegment)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), new Vector2((int)Projectile.Center.X, (int)Projectile.Center.Y), 
                Vector2.Zero, ModContent.ProjectileType<EyeWormBody>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai0: Projectile.whoAmI, ai1: 1);

                SpawnedSegment = true;
            }

            if (Projectile.timeLeft <= 60)
            {
                Projectile.alpha += 5;
            }

            float WaveIntensity = 10f;
            float Wave = 8f;

            Projectile.ai[0]++;
            if (Projectile.ai[1] == 0)
            {
                if (Projectile.ai[0] > Wave * 0.5f)
                {
                    Projectile.ai[0] = 0;
                    Projectile.ai[1] = 1;
                }
                else
                {
                    Vector2 perturbedSpeed = new Vector2(Projectile.velocity.X, Projectile.velocity.Y).RotatedBy(MathHelper.ToRadians(WaveIntensity));
                    Projectile.velocity = perturbedSpeed;
                }
            }
            else
            {
                if (Projectile.ai[0] <= Wave)
                {
                    Vector2 perturbedSpeed = new Vector2(Projectile.velocity.X, Projectile.velocity.Y).RotatedBy(MathHelper.ToRadians(-WaveIntensity));
                    Projectile.velocity = perturbedSpeed;
                }
                else
                {
                    Vector2 perturbedSpeed = new Vector2(Projectile.velocity.X, Projectile.velocity.Y).RotatedBy(MathHelper.ToRadians(WaveIntensity));
                    Projectile.velocity = perturbedSpeed;
                }
                if (Projectile.ai[0] >= Wave * 2)
                {
                    Projectile.ai[0] = 0;
                }
            }
        }
    }
}