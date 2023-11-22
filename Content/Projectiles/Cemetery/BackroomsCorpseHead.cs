using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Buffs.Debuff;
using Spooky.Content.Buffs.Minion;

namespace Spooky.Content.Projectiles.Cemetery
{
	public class BackroomsCorpseHead : ModProjectile
    {
        public override void SetStaticDefaults()
        {
			Main.projFrames[Projectile.type] = 3;
        }
        
        public override void SetDefaults()
        {
			Projectile.width = 20;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.timeLeft = 2;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= 3)
                {
                    Projectile.frame = 0;
                }
            }

            Player player = Main.player[Projectile.owner];

            if (player.dead)
            {
                player.GetModPlayer<SpookyPlayer>().BackroomsCorpse = false;
            }

            if (player.GetModPlayer<SpookyPlayer>().BackroomsCorpse)
            {
                Projectile.timeLeft = 2;
            }

            Projectile.spriteDirection = -player.direction;

			float goToX = (player.Center.X + (player.direction == -1 ? 35 : -35)) - Projectile.Center.X;
            float goToY = player.Center.Y - Projectile.Center.Y - 50;

            float speed = 0.08f;
            
            if (Vector2.Distance(Projectile.Center, player.Center) >= 140)
            {
                speed = 0.2f;
            }
            else
            {
                speed = 0.12f;
            }
            
            if (Projectile.velocity.X > speed)
            {
                Projectile.velocity.X *= 0.98f;
            }
            if (Projectile.velocity.Y > speed)
            {
                Projectile.velocity.Y *= 0.98f;
            }

            if (Projectile.velocity.X < goToX)
            {
                Projectile.velocity.X = Projectile.velocity.X + speed;
                if (Projectile.velocity.X < 0f && goToX > 0f)
                {
                    Projectile.velocity.X = Projectile.velocity.X + speed;
                }
            }
            else if (Projectile.velocity.X > goToX)
            {
                Projectile.velocity.X = Projectile.velocity.X - speed;
                if (Projectile.velocity.X > 0f && goToX < 0f)
                {
                    Projectile.velocity.X = Projectile.velocity.X - speed;
                }
            }
            if (Projectile.velocity.Y < goToY)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + speed;
                if (Projectile.velocity.Y < 0f && goToY > 0f)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + speed;
                    return;
                }
            }
            else if (Projectile.velocity.Y > goToY)
            {
                Projectile.velocity.Y = Projectile.velocity.Y - speed;
                if (Projectile.velocity.Y > 0f && goToY < 0f)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y - speed;
                    return;
                }
            }

            for (int k = 0; k < Main.maxNPCs; k++)
			{
				if (Main.npc[k].active && Main.npc[k].Distance(Projectile.Center) <= 275f)
                {
                    Main.npc[k].AddBuff(ModContent.BuffType<BackroomsDecay>(), 2);
                }
            }
		}
    }
}