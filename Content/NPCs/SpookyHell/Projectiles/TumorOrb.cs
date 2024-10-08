using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

namespace Spooky.Content.NPCs.SpookyHell.Projectiles
{
	public class TumorOrb : ModProjectile
	{
        int OffsetX = Main.rand.Next(-50, 50);
        int OffsetY = Main.rand.Next(-50, 50);

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
        }

		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 16;
            Projectile.hostile = true;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 360;
            Projectile.aiStyle = -1;
		}

		public override void AI()
		{
            Projectile.rotation += 0.2f * (float)Projectile.direction;

            if (Projectile.spriteDirection == -1)
            {
                Projectile.rotation += MathHelper.Pi;
            }

            Projectile.frame = (int)Projectile.ai[2];

            Projectile.ai[0]++;

            if (Projectile.ai[0] < 180)
            {
                int index1 = (int)Projectile.ai[1];
                
                if (Main.npc[index1].active && (Main.npc[index1].type == ModContent.NPCType<TortumorFleshy>() || Main.npc[index1].type == ModContent.NPCType<TortumorGiant>())) 
                {
                    float goToX = Main.npc[index1].Center.X + OffsetX - Projectile.Center.X;
                    float goToY = Main.npc[index1].Center.Y + OffsetY - Projectile.Center.Y;

                    float speed = 0.12f;
                    
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
                else
                {
                    Projectile.Kill();
                }
            }
            
            if (Projectile.ai[0] == 180)
            {
                Player player = Main.player[Main.myPlayer];

                double angle = Math.Atan2(player.Center.Y - Projectile.Center.Y, player.Center.X - Projectile.Center.X);
                Projectile.velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 12;

                Projectile.tileCollide = true;
            }

            if (Projectile.ai[0] >= 180)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + 0.025f;
            }
		}

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath19, Projectile.Center);

            for (int numDust = 0; numDust < 20; numDust++)
            {
                int DustGore = Dust.NewDust(new Vector2(Projectile.Center.X, Projectile.Center.Y), 
                Projectile.width, Projectile.height, DustID.Blood, 0f, 0f, 100, default, 2f);

                Main.dust[DustGore].scale *= Main.rand.NextFloat(1f, 2f);
                Main.dust[DustGore].velocity *= 3f;
                Main.dust[DustGore].noGravity = true;

                if (Main.rand.NextBool(2))
                {
                    Main.dust[DustGore].scale = 0.5f;
                    Main.dust[DustGore].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
        }
    }
}