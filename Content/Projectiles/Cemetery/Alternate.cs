using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Cemetery
{
    public class Alternate : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
        }
        
        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 52;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.timeLeft = 600;
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

            Projectile.spriteDirection = -player.direction;

            if (Projectile.timeLeft <= 255)
            {
                Projectile.alpha++;
            }
            
            //movement
            if (Projectile.localAI[0] < 1200)
            {
                float goToX = (player.Center.X + (35 * -player.direction)) - Projectile.Center.X;
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
            }

            //slow down any nearby non-boss enemy
            for (int i = 0; i < Main.maxNPCs; i++)
			{
                NPC NPC = Main.npc[i];

				if (NPC.active && NPC.CanBeChasedBy(this) && !NPC.friendly && !NPC.dontTakeDamage && !NPCID.Sets.CountsAsCritter[NPC.type] && NPC.Distance(Projectile.Center) <= 250f)
                {
                    if (!NPC.boss && NPC.type != NPCID.EaterofWorldsHead && NPC.type != NPCID.EaterofWorldsBody && NPC.type != NPCID.EaterofWorldsTail)
                    {
                        NPC.velocity *= player.GetModPlayer<SpookyPlayer>().AnalogHorrorTape ? 0.75f : 0.95f;
                    }
                }
            }
        }
    }
}