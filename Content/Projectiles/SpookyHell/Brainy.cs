using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;
using Spooky.Content.Buffs.Minion;

namespace Spooky.Content.Projectiles.SpookyHell
{
    public class Brainy : ModProjectile
    {
        float ScaleAmount = 0.05f;
        int ScaleTimerLimit = 10;

        public static readonly SoundStyle BrainySound1 = new("Spooky/Content/Sounds/Brainy1", SoundType.Sound);
        public static readonly SoundStyle BrainySound2 = new("Spooky/Content/Sounds/Brainy2", SoundType.Sound);
        public static readonly SoundStyle BrainyConvulseSound = new("Spooky/Content/Sounds/BrainyConvulse", SoundType.Sound);

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brainy");
            Main.projFrames[Projectile.type] = 7;
            Main.projPet[Projectile.type] = true;
        }
        
        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 46;
            Projectile.minion = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.timeLeft = 2;
            Projectile.penetrate = -1;
            Projectile.minionSlots = 0;
            Projectile.aiStyle = -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

            Color color = new Color(127 - Projectile.alpha, 127 - Projectile.alpha, 127 - Projectile.alpha, 0).MultiplyRGBA(Color.DeepPink);

            Vector2 drawOrigin = new(tex.Width * 0.5f, Projectile.height * 0.5f);

            for (int numEffect = 0; numEffect < 3; numEffect++)
            {
                var effects = Projectile.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                Color newColor = color;
                newColor = Projectile.GetAlpha(newColor);
                newColor *= 1f;
                Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) + (numEffect / 3 * 6.28318548f + Projectile.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(0, Projectile.gfxOffY) - Projectile.velocity * numEffect;
                Rectangle rectangle = new(0, tex.Height / Main.projFrames[Projectile.type] * Projectile.frame, tex.Width, tex.Height / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(tex, vector, rectangle, newColor, Projectile.rotation, drawOrigin, Projectile.scale * 1.1f, effects, 0);
            }

            return true;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] <= 3510)
            {
                Projectile.frameCounter++;
                if (Projectile.frameCounter >= 6)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                    if (Projectile.frame >= 6)
                    {
                        Projectile.frame = 0;
                    }
                }
            }
            else
            {
                Projectile.frame = 6;
            }

            Projectile.spriteDirection = -Projectile.direction;

            Player player = Main.player[Projectile.owner];

			if (player.dead)
			{
				player.GetModPlayer<SpookyPlayer>().Brainy = false;
			}

			if (player.GetModPlayer<SpookyPlayer>().Brainy)
			{
				Projectile.timeLeft = 2;
			}

            //actual minion exploding ai
            Projectile.localAI[0]++;

            if (Projectile.localAI[0] < 3510)
            {
                if (Main.rand.Next(750) == 0)
                {
                    if (Main.rand.Next(2) == 0)
                    {
                        SoundEngine.PlaySound(BrainySound1, Projectile.Center);
                    }
                    else
                    {
                        SoundEngine.PlaySound(BrainySound2, Projectile.Center);
                    }
                }
            }

            if (Projectile.localAI[0] == 3510)
            {
                SoundEngine.PlaySound(BrainyConvulseSound, Projectile.Center);
            }

            if (Projectile.localAI[0] >= 3510 && Projectile.localAI[0] < 3600)
            {
                if (Projectile.localAI[0] == 3510 || Projectile.localAI[0] == 3540 || Projectile.localAI[0] == 3570)
                {
                    ScaleAmount += 0.05f;
                    ScaleTimerLimit -= 3;
                }

                Projectile.localAI[1]++;
                if (Projectile.localAI[1] < ScaleTimerLimit)
                {
                    Projectile.scale -= ScaleAmount;
                }
                if (Projectile.localAI[1] >= ScaleTimerLimit)
                {
                    Projectile.scale += ScaleAmount;
                }

                if (Projectile.localAI[1] > ScaleTimerLimit * 2)
                {
                    Projectile.localAI[1] = 0;
                    Projectile.scale = 1f;
                }
            }
            else
            {   
                Projectile.localAI[1] = 0;
                Projectile.scale = 1f;
            }

            if (Projectile.localAI[0] >= 3600)
            {
                for (int k = 0; k < Main.projectile.Length; k++)
                {
                    if (Main.projectile[k].active && Main.projectile[k].minion && Main.projectile[k].type != ModContent.ProjectileType<Brainy>()) 
                    {
                        SoundEngine.PlaySound(SoundID.Item96, Main.projectile[k].Center);

                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Main.projectile[k].Center.X, Main.projectile[k].Center.Y, 
                        0, 0, ModContent.ProjectileType<BrainyExplosion>(), Projectile.damage + (Main.projectile[k].damage / 2), 0f, Main.myPlayer, 0, 0);

                        Main.projectile[k].Kill();
                    }
                }

                ScaleAmount = 0.05f;
                ScaleTimerLimit = 10;

                Projectile.localAI[0] = 0;
            }
        
            //movement stuff
            if (!Collision.CanHitLine(Projectile.Center, 1, 1, player.Center, 1, 1))
            {
                Projectile.ai[0] = 1f;
            }

            float speed = 8f;
            if (Projectile.ai[0] == 1f)
            {
                speed = 15f;
            }

            Vector2 center = Projectile.Center;
            Vector2 direction = player.Center - center;
            Projectile.ai[1] = 3600f;
            Projectile.netUpdate = true;
            int num = 1;
            for (int k = 0; k < Projectile.whoAmI; k++)
            {
                if (Main.projectile[k].active && Main.projectile[k].owner == Projectile.owner && Main.projectile[k].type == Projectile.type)
                {
                    num++;
                }
            }
            
            direction.Y -= 70f;
            float distanceTo = direction.Length();
            if (distanceTo > 200f && speed < 9f)
            {
                speed = 9f;
            }
            if (distanceTo < 100f && Projectile.ai[0] == 1f && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                Projectile.ai[0] = 0f;
                Projectile.netUpdate = true;
            }
            if (distanceTo > 2000f)
            {
                Projectile.Center = player.Center;
            }
            if (distanceTo > 48f)
            {
                direction.Normalize();
                direction *= speed;
                float temp = 40 / 2f;
                Projectile.velocity = (Projectile.velocity * temp + direction) / (temp + 1);
            }
            else
            {
                Projectile.velocity *= (float)Math.Pow(0.9, 40.0 / 40);
            }

            Projectile.rotation = Projectile.velocity.X * 0.05f;

            if ((double)Math.Abs(Projectile.velocity.X) > 0.2)
            {
                Projectile.spriteDirection = -Projectile.direction;
                return;
            }
        }
    }
}