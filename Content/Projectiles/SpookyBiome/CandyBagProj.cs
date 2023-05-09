using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

using Spooky.Core;
using Spooky.Content.Buffs.Minion;
using Spooky.Content.Items.SpookyBiome;

namespace Spooky.Content.Projectiles.SpookyBiome
{
    public class CandyBagProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            Main.projPet[Projectile.type] = true;
        }
        
        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 28;
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
            if (Projectile.frameCounter >= 16)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= 4)
                {
                    Projectile.frame = 0;
                }
            }

            Player player = Main.player[Projectile.owner];
            
            if (player.dead)
            {
                player.GetModPlayer<SpookyPlayer>().CandyBag = false;
            }

            if (player.GetModPlayer<SpookyPlayer>().CandyBag)
            {
                Projectile.timeLeft = 2;
                player.AddBuff(ModContent.BuffType<CandyBagBuff>(), 2, false);
            }

            if (!player.GetModPlayer<SpookyPlayer>().CandyBag)
            {
                Projectile.Kill();
            }

            Projectile.localAI[0]++;
            
            //movement
            if (Projectile.localAI[0] < 1200)
            {
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
                
                direction.X += (20 + num * 40) * player.direction;
                direction.Y -= 70f;
                float distanceTo = direction.Length();
                if (distanceTo > 10f && speed < 9f)
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
                    Projectile.direction = Main.player[Projectile.owner].direction;
                    Projectile.velocity *= (float)Math.Pow(0.9, 40.0 / 40);
                }

                Projectile.rotation = Projectile.velocity.X * 0.05f;

                if ((double)Math.Abs(Projectile.velocity.X) > 0.2)
                {
                    Projectile.spriteDirection = -Projectile.direction;
                    return;
                }
            }

            //slow down
            if (Projectile.localAI[0] >= 1000)
            {
                Projectile.velocity *= 0.98f;
            }

            //drop candy
            if (Projectile.localAI[0] >= 1200)
            {
                SoundEngine.PlaySound(SoundID.MaxMana, Projectile.Center);

                int[] Candies = new int[] { ModContent.ItemType<Candy1>(), ModContent.ItemType<Candy2>(), ModContent.ItemType<Candy3>() };
                int newItem = Item.NewItem(Projectile.GetSource_DropAsItem(), Projectile.Hitbox, Main.rand.Next(Candies));
                Main.item[newItem].noGrabDelay = 0;

                if (Main.netMode == NetmodeID.MultiplayerClient && newItem >= 0)
                {
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, newItem, 1f);
                }
                
                Projectile.localAI[0] = 0;
            }
        }
    }
}