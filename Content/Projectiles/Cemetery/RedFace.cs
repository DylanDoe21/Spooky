using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Cemetery
{
    public class RedFace : ModProjectile
    {
        public int SaveDirection;
        public float SaveRotation;

        public override void SetDefaults()
        {
            Projectile.width = 88;
			Projectile.height = 56;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1000;
            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            NPC target = Main.npc[(int)Projectile.ai[1]];

            Projectile.ai[0]++;

            if (Projectile.ai[0] < 60)
            {
                Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y);
                float RotateX = target.Center.X - vector.X;
                float RotateY = target.Center.Y - vector.Y;
                Projectile.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;

                Projectile.direction = Projectile.spriteDirection = target.Center.X < Projectile.Center.X ? -1 : 1;

                Projectile.alpha -= 5;

                if (Projectile.alpha <= 0)
                {
                    Projectile.alpha = 0;
                }
            }
    
            if (Projectile.ai[0] == 60)
            {
                SaveDirection = Projectile.direction;
                SaveRotation = Projectile.rotation;

                double Velocity = Math.Atan2(target.Center.Y - Projectile.Center.Y, target.Center.X - Projectile.Center.X);
                Projectile.velocity = new Vector2((float)Math.Cos(Velocity), (float)Math.Sin(Velocity)) * 12;
            }

            if (Projectile.localAI[0] > 60)
            {
                Projectile.spriteDirection = SaveDirection;
                Projectile.rotation = SaveRotation;
            }

            if (Projectile.ai[0] > 75)
            {
                Projectile.alpha += 10;

                if (Projectile.alpha >= 255)
                {
                    Projectile.Kill();
                }
            }
        }
    }
}