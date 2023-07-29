using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System.IO;
using System.Collections.Generic;

using Spooky.Content.Items.Food;
using Spooky.Content.Items.Costume;

namespace Spooky.Content.NPCs.Catacomb.Layer1
{
    public class ZomboidNecromancer : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 7;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 100;
            NPC.damage = 22;
            NPC.defense = 5;
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
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.ZomboidNecromancer"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;

            if (player.InModBiome(ModContent.GetInstance<Biomes.CatacombBiome>()))
            {
                return 8f;
            }

            return 0f;
        }

        public override void FindFrame(int frameHeight)
        {
            //running animation
            NPC.frameCounter += 1;

            //use regular walking anim when in walking state
            if (NPC.localAI[0] <= 420)
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
            if (NPC.localAI[0] > 420)
            {
                if (NPC.frame.Y < frameHeight * 5)
                {
                    NPC.frame.Y = 4 * frameHeight;
                }

                if (NPC.frameCounter > 10)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0.0;
                }
                if (NPC.frame.Y >= frameHeight * 7)
                {
                    NPC.frame.Y = 6 * frameHeight;
                }
            }
        }
        
        public override void AI()
		{
            Player player = Main.player[NPC.target];

            NPC.spriteDirection = NPC.direction;

            NPC.localAI[0]++;

            if (NPC.localAI[0] <= 420)
            {
                NPC.aiStyle = 3;
                AIType = NPCID.Crab;
            }

            if (NPC.localAI[0] > 420)
            {
                NPC.aiStyle = 0;

                if (NPC.localAI[0] == 480 || NPC.localAI[0] == 500)
                {
                    SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
                }
            }

            if (NPC.localAI[0] >= 560)
            {
                NPC.localAI[0] = 0;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ZomboidNecromancerHood>(), 5));
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
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/ZomboidNecromancerGore" + numGores).Type);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/ZomboidNecromancerCloth" + numGores).Type);
                }
            }
        }
    }
}