using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using System.IO;
using System.Collections.Generic;

using Spooky.Content.Items.Food;

namespace Spooky.Content.NPCs.Catacomb.Layer2
{
    public class RollFlower : ModNPC  
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 5;
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
            NPC.lifeMax = 180;
            NPC.damage = 40;
            NPC.defense = 22;
            NPC.width = 54;
			NPC.height = 56;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.5f;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.HitSound = SoundID.Grass;
			NPC.DeathSound = SoundID.NPCDeath1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CatacombBiome2>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.RollFlower"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome2>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void FindFrame(int frameHeight)
        {   
            NPC.frameCounter += 1;

            //running animation
            if (NPC.frameCounter > 6)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0.0;
            }
            if (NPC.frame.Y >= frameHeight * 4)
            {
                NPC.frame.Y = 0 * frameHeight;
            }

            //jumping/rolling frame
            if ((NPC.velocity.Y > 0 || NPC.velocity.Y < 0) || NPC.localAI[0] >= 420)
            {
                NPC.frame.Y = 4 * frameHeight;
            }
        }
        
        public override void AI()
		{
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

			NPC.spriteDirection = NPC.direction;

            NPC.localAI[0]++;

            if (NPC.localAI[0] < 420)
            {
                NPC.rotation = 0;
                NPC.aiStyle = 3;
			    AIType = NPCID.GoblinWarrior;
            }

            if (NPC.localAI[0] >= 420)
            {
                NPC.rotation += 0.45f * (float)NPC.direction;
                NPC.velocity.X = NPC.velocity.X * 1.05f;

                if (NPC.velocity.X >= 8)
                {
                    NPC.velocity.X = 8;
                }

                if (NPC.velocity.X <= -8)
                {
                    NPC.velocity.X = -8;
                }

                NPC.aiStyle = 26;
			    AIType = NPCID.Tumbleweed;
            }

            if (NPC.localAI[0] >= 620)
            {
                NPC.localAI[0] = 0;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CandyCorn>(), 100));
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
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/RollFlowerGore" + numGores).Type);
                }
            }
        }
    }
}