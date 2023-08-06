using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Content.Dusts;
using Spooky.Content.Items.Food;
using Spooky.Content.Items.Catacomb;
using Spooky.Content.Items.Costume;

namespace Spooky.Content.NPCs.Catacomb.Layer1
{
    public class ZomboidPyromancer : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 10;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 150;
            NPC.damage = 20;
            NPC.defense = 0;
            NPC.width = 46;
			NPC.height = 56;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.5f;
            NPC.value = Item.buyPrice(0, 0, 1, 75);
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath2;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CatacombBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.ZomboidPyromancer"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Catacomb/Layer1/ZomboidPyromancerFlames").Value;

            var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int numEffect = 0; numEffect < 5; numEffect++)
            {
                float shakeX = Main.rand.Next(-2, 2);
			    float shakeY = Main.rand.Next(-2, 2);

                Main.EntitySpriteDraw(tex, NPC.Center - Main.screenPosition + new Vector2(0 + shakeX, NPC.gfxOffY + 4 + shakeY), 
                NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0.5f);
            }
        }

        public override void FindFrame(int frameHeight)
        {
            //running animation
            NPC.frameCounter += 1;

            if (NPC.localAI[0] == 0)
            {
                //use regular walking anim when in walking state
                if (NPC.localAI[1] < 60)
                {
                    if (NPC.frameCounter > 10)
                    {
                        NPC.frame.Y = NPC.frame.Y + frameHeight;
                        NPC.frameCounter = 0.0;
                    }
                    if (NPC.frame.Y >= frameHeight * 4)
                    {
                        NPC.frame.Y = 0 * frameHeight;
                    }

                    //frame when falling/jumping
                    if (NPC.velocity.Y > 0 || NPC.velocity.Y < 0)
                    {
                        NPC.frame.Y = 2 * frameHeight;
                    }
                }
                //use casting animation during casting ai
                if (NPC.localAI[1] >= 60)
                {
                    if (NPC.frame.Y < frameHeight * 5)
                    {
                        NPC.frame.Y = 4 * frameHeight;
                    }

                    if (NPC.frameCounter > 4)
                    {
                        NPC.frame.Y = NPC.frame.Y + frameHeight;
                        NPC.frameCounter = 0.0;
                    }
                    if (NPC.frame.Y >= frameHeight * 7)
                    {
                        NPC.frame.Y = 5 * frameHeight;
                    }
                }
            }
            else
            {
                if (NPC.frame.Y < frameHeight * 8)
                {
                    NPC.frame.Y = 7 * frameHeight;
                }

                if (NPC.frameCounter > 4)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0.0;
                }
                if (NPC.frame.Y >= frameHeight * 10)
                {
                    NPC.frame.Y = 7 * frameHeight;
                }
            }
        }
        
        public override void AI()
		{
            Player player = Main.player[NPC.target];

            NPC.spriteDirection = NPC.direction;

            int Damage = Main.masterMode ? 40 / 3 : Main.expertMode ? 30 / 2 : 20;

            switch ((int)NPC.localAI[0])
            {
                case 0:
                {
                    if (player.Distance(NPC.Center) <= 150f || NPC.localAI[1] >= 60)
                    {
                        NPC.localAI[1]++;
                    }

                    if (NPC.localAI[1] < 60)
                    {
                        NPC.aiStyle = 3;
                        AIType = NPCID.Crab;
                    }

                    if (NPC.localAI[1] >= 60)
                    {
                        NPC.aiStyle = 0;

                        int MaxDusts = Main.rand.Next(3, 8);
                        for (int numDusts = 0; numDusts < MaxDusts; numDusts++)
                        {
                            Vector2 dustPos = (Vector2.One * new Vector2((float)NPC.width / 3f, (float)NPC.height / 3f) * Main.rand.NextFloat(1.25f, 1.75f)).RotatedBy((double)((float)(numDusts - (MaxDusts / 2 - 1)) * 6.28318548f / (float)MaxDusts), default(Vector2)) + NPC.Center;
                            Vector2 velocity = dustPos - NPC.Center;
                            int dustEffect = Dust.NewDust(dustPos + velocity, 0, 0, ModContent.DustType<GlowyDust>(), velocity.X * 2f, velocity.Y * 2f, 100, default, 1f);
                            Main.dust[dustEffect].color = Color.Orange;
                            Main.dust[dustEffect].scale = 0.1f;
                            Main.dust[dustEffect].noGravity = true;
                            Main.dust[dustEffect].noLight = false;
                            Main.dust[dustEffect].velocity = Vector2.Normalize(velocity) * Main.rand.NextFloat(-2f, -1f);
                            Main.dust[dustEffect].fadeIn = 1.3f;
                        }

                        //explode
                        if (NPC.localAI[1] == 115)
                        {
                            SoundEngine.PlaySound(SoundID.NPCDeath14, NPC.Center);

                            for (int numDust = 0; numDust < 35; numDust++)
                            {                                                                                  
                                int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.InfernoFork, 0f, -2f, 0, default, 1.5f);
                                Main.dust[dust].velocity.X *= Main.rand.NextFloat(-12f, 12f);
                                Main.dust[dust].velocity.Y *= Main.rand.NextFloat(-12f, 12f);
                                Main.dust[dust].scale = Main.rand.NextFloat(1f, 2f);
                                Main.dust[dust].noGravity = true;
                            }

                            NPC.localAI[1] = 0;
                            NPC.localAI[0]++;
                        }
                    }

                    break;
                }

                case 1:
                {
                    NPC.aiStyle = 3;
                    AIType = NPCID.DesertGhoul;

                    NPC.AddBuff(BuffID.OnFire3, 2);

                    NPC.localAI[1]++;

                    if (NPC.localAI[1] % 60 == 20)
                    {
                        for (int numProjectiles = 0; numProjectiles < 2; numProjectiles++)
                        {
                            int[] Types = new int[] { ProjectileID.GreekFire1, ProjectileID.GreekFire2, ProjectileID.GreekFire3 };

                            Vector2 Speed = new Vector2(2f, 0f).RotatedByRandom(2 * Math.PI);

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Speed, Main.rand.Next(Types), Damage, 0f, NPC.target, 0, 0);
                        }

                        NPC.localAI[1] = 0;
                    }

                    break;
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FlameIdol>(), 30));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ZomboidPyromancerHood>(), 5));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FrankenMarshmallow>(), 50));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            //dont run on multiplayer
			if (Main.netMode == NetmodeID.Server) 
            {
				return;
			}

			if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 3; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/ZomboidPyromancerGore" + numGores).Type);
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/ZomboidPyromancerCloth" + numGores).Type);
                    }
                }
            }
        }
    }
}