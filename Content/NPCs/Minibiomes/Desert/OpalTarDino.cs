using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.Minibiomes.Desert;
using Spooky.Content.NPCs.Minibiomes.Desert.Projectiles;

namespace Spooky.Content.NPCs.Minibiomes.Desert
{
    public class OpalTarDino : ModNPC  
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 6;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
				Position = new Vector2(0f, 15f),
				PortraitPositionXOverride = 0f,
				PortraitPositionYOverride = 0f
			};
        }

        public override void SetDefaults()
		{
            NPC.lifeMax = 400;
            NPC.damage = 55;
            NPC.defense = 20;
            NPC.width = 82;
			NPC.height = 80;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 5, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.behindTiles = true;
            NPC.HitSound = SoundID.Item95 with { Volume = 0.8f, Pitch = 1f };
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.TarPitsBiome>().Type };
        }

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
			{
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.OpalTarDino"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.TarPitsBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;

            if (NPC.ai[0] <= 60)
            {
                if (NPC.frameCounter > 5)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 3)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }
            }
            else
            {
                if (NPC.frame.Y < frameHeight * 5)
                {
                    NPC.frame.Y = 4 * frameHeight;
                }

                if (NPC.frameCounter > 5)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 6)
                {
                    NPC.frame.Y = 4 * frameHeight;
                }
            }
        }
        
        public override void AI()
		{
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];
            
            bool PlayerLineOfSight = Collision.CanHitLine(player.Center - new Vector2(10, 10), 20, 20, NPC.position, NPC.width, NPC.height);
            if ((player.Distance(NPC.Center) <= 500f && PlayerLineOfSight) || NPC.ai[0] >= 60)
            {
                NPC.ai[0]++;
                if (NPC.ai[0] >= 60 && NPC.ai[0] % 10 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item54 with { Pitch = -1f }, NPC.Center);
                
                    NPC.ai[1]++;
                    if (NPC.ai[1] >= 5 && TotalNearbySpikedSlimes() < 5)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int TarSlime = NPC.NewNPC(null, (int)NPC.Bottom.X, (int)NPC.Bottom.Y, ModContent.NPCType<TarSlimeSpiked>());

                            if (Main.netMode == NetmodeID.Server)
                            {
                                NetMessage.SendData(MessageID.SyncNPC, number: TarSlime);
                            }
                        }

                        NPC.ai[1] = 0;
                    }
                    else
                    {
                        Vector2 ShootSpeed = new Vector2(0, Main.rand.Next(5, 11));
                        Vector2 newVelocity = ShootSpeed.RotatedByRandom(MathHelper.ToRadians(7));

                        NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Bottom, new Vector2(newVelocity.X, Main.rand.Next(5, 11)), ModContent.ProjectileType<OpalTarDinoGlob>(), NPC.damage, 4.5f, ai0: Main.rand.Next(0, 3));
                    }
                }

                if (NPC.ai[0] >= 120)
                {
                    NPC.ai[0] = 0;
                }
            }
        }

        public int TotalNearbySpikedSlimes()
		{
            int NpcCount = 0;

            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.type == ModContent.NPCType<TarSlimeSpiked>() && NPC.Distance(npc.Center) <= 600f)
				{
					NpcCount++;
				}
				else
				{
					continue;
				}
			}

			return NpcCount;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<TarCannon>(), 10));
		}

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numDusts = 0; numDusts < 25; numDusts++)
                {                                                                                  
                    int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Asphalt, 0f, -2f, 0, default, 1f);
                    Main.dust[dust].position.X += Main.rand.Next(-25, 25) * 0.05f - 1.5f;
                    Main.dust[dust].position.Y += Main.rand.Next(-25, 25) * 0.05f - 1.5f;
                }

                for (int numGores = 1; numGores <= 5; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/OpalTarDinoGore" + numGores).Type);
                    }
                }
            }
        }
    }
}