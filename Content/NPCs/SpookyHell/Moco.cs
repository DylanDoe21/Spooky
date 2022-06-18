using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

using Spooky.Content.NPCs.SpookyHell.Projectiles;
using Spooky.Content.Items.SpookyHell;
using Spooky.Content.Items.BossBags.Pets;

namespace Spooky.Content.NPCs.SpookyHell
{
    public class Moco : ModNPC
    {
        int moveSpeed = 0;
		int moveSpeedY = 0;
		float HomeY = 150f;
        
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Moco");
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.TrailCacheLength[NPC.type] = 7;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 1500;
            NPC.damage = 50;
            NPC.defense = 10;
            NPC.width = 110;
			NPC.height = 96;
			NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 10, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit44;
			NPC.DeathSound = SoundID.NPCDeath47;
            NPC.aiStyle = -1;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 1;
            if (NPC.frameCounter > 3)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0.0;
            }
            if (NPC.frame.Y >= frameHeight * 4)
            {
                NPC.frame.Y = 0;
            }
        }

        public static readonly SoundStyle MocoSneeze = new SoundStyle("Spooky/Content/Sounds/MocoSneeze", SoundType.Sound);
        public override void AI()
        {
            NPC.netUpdate = true;

            NPC.spriteDirection = NPC.direction;
            
            int Damage = Main.expertMode ? 22 : 35;

            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

            NPC.ai[0]++;

            if (NPC.ai[0] < 500)
            {
                NPC.rotation = NPC.velocity.X * 0.04f;
            }
            
            if (NPC.ai[0] < 360)
            {
                NPC.defense = 0;
                if (NPC.Center.X >= player.Center.X && moveSpeed >= -45) // flies to players x position
                {
                    moveSpeed--;
                }

                if (NPC.Center.X <= player.Center.X && moveSpeed <= 45)
                {
                    moveSpeed++;
                }

                NPC.velocity.X = moveSpeed * 0.1f;

                if (NPC.Center.Y >= player.Center.Y - HomeY && moveSpeedY >= -45) //Flies to players Y position
                {
                    moveSpeedY--;
                    HomeY = 125f;
                }

                if (NPC.Center.Y <= player.Center.Y - HomeY && moveSpeedY <= 45)
                {
                    moveSpeedY++;
                }

                NPC.velocity.Y = moveSpeedY * 0.22f;

                if (Main.rand.Next(200) == 0)
                {
                    HomeY = -25f;
                }
            }

            //go diagonal of the player
            if (NPC.ai[0] >= 360 && NPC.ai[0] < 420)
            {
                NPC.direction = Math.Sign(player.Center.X - NPC.Center.X);	
                Vector2 GoTo = player.Center;
                NPC.spriteDirection = NPC.direction;
                GoTo.X += (NPC.Center.X < player.Center.X) ? -270 : 270;
                GoTo.Y -= 270;

                float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 12, 25);
                NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
            }

            //slow down right before charge to stop weird movement
            if (NPC.ai[0] == 420)
            {
                NPC.velocity *= 0;
            }

            //Charge at the player
            if (NPC.ai[0] == 422)
            {
                SoundEngine.PlaySound(SoundID.Roar, NPC.position);

                Vector2 direction1 = Main.player[NPC.target].Center - NPC.Center;
                direction1.Normalize();
                        
                direction1.X = direction1.X * 22;
                direction1.Y = direction1.Y * 0;  
                NPC.velocity.X = direction1.X;
                NPC.velocity.Y = direction1.Y;
            }
            
            if (NPC.ai[0] >= 420 && NPC.ai[0] < 480)
            {
                NPC.velocity *= 0.98f;

                if (Main.rand.Next(8) == 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, 0, 5,
                        ModContent.ProjectileType<MocoBooger>(), Damage, 1, Main.myPlayer, 0, 0);
                    }
                }
            }

            //EoC rotation for when he sneezes
            if (NPC.ai[0] > 500)
            {
                NPC.velocity *= 0.98f;

                Vector2 vector92 = new Vector2(NPC.Center.X, NPC.Center.Y);
                float num740 = Main.player[NPC.target].Center.X - vector92.X;
                float num741 = Main.player[NPC.target].Center.Y - vector92.Y;
                NPC.rotation = (float)Math.Atan2((double)num741, (double)num740) + 4.71f;
            }

            //Sneeze attack
            if (NPC.ai[0] == 600)
            {
                //play cute sneeze sound
                SoundEngine.PlaySound(MocoSneeze, NPC.position);

                Vector2 ShootSpeed = Main.player[NPC.target].Center - NPC.Center;
                ShootSpeed.Normalize();
                ShootSpeed.X *= 6.5f;
                ShootSpeed.Y *= 6.5f;

                int amountOfProjectiles = Main.rand.Next(3, 6);
                for (int i = 0; i < amountOfProjectiles; ++i)
                {			
                    float Spread = (float)Main.rand.Next(-100, 100) * 0.01f;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, ShootSpeed.X + Spread, ShootSpeed.Y + Spread, 
                        ModContent.ProjectileType<MocoBooger>(), Damage, 1, Main.myPlayer, 0, 0);
                    }
                }

                //charge opposite of the player to simulate recoil
                Vector2 ChargeDirection = Main.player[NPC.target].Center - NPC.Center;
                ChargeDirection.Normalize();
                        
                ChargeDirection.X = ChargeDirection.X * -8;
                ChargeDirection.Y = ChargeDirection.Y * -8;  
                NPC.velocity.X = ChargeDirection.X;
                NPC.velocity.Y = ChargeDirection.Y;
            }

            //loop ai
            if (NPC.ai[0] >= 700)
            {
                NPC.ai[0] = 0;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MonsterChunk>(), Main.rand.Next(3, 6)));

            if (Main.rand.Next(20) == 0)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SusTissue>(), 1));
            }
        }

        public override void HitEffect(int hitDirection, double damage)
		{
			if (NPC.life <= 0)
			{
                int Damage = Main.expertMode ? 22 : 35;

                Vector2 vel = new Vector2(2f, 0f).RotatedByRandom(2 * Math.PI);
                for (int i = 0; i < 4; i++)
                {
                    Vector2 speed = vel.RotatedBy(2 * Math.PI / 4 * (i + Main.rand.NextDouble() - 0.5));
                    Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, speed, ModContent.ProjectileType<ToxicSpit>(), Damage, 0f, Main.myPlayer, 0, 0);
                }
            
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Spooky/MocoGore1").Type);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Spooky/MocoGore2").Type);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Spooky/MocoGore3").Type);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Spooky/MocoGore4").Type);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Spooky/MocoGore5").Type);
            }
        }
    }
}