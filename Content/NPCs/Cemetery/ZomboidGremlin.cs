using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using System.Collections.Generic;

using Spooky.Content.Items.Food;

namespace Spooky.Content.NPCs.Cemetery
{
    public class ZomboidGremlin : ModNPC  
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 5;
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 25;
            NPC.damage = 15;
            NPC.defense = 0;
            NPC.width = 28;
			NPC.height = 40;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 1f;
            NPC.value = Item.buyPrice(0, 0, 0, 20);
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath2 with { Pitch = 0.45f };
            NPC.aiStyle = 3;
            AIType = NPCID.DesertGhoul;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CemeteryBiome>().Type };
		}

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.ZomboidGremlin"),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CemeteryBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void FindFrame(int frameHeight)
        {
            //running animation
            NPC.frameCounter++;
            if (NPC.frameCounter > 3)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 4)
            {
                NPC.frame.Y = 0 * frameHeight;
            }

            //jumping frame
            if (NPC.velocity.Y > 0 || NPC.velocity.Y < 0)
            {
                NPC.frame.Y = 4 * frameHeight;
            }
        }
        
        public override void AI()
		{
            NPC.spriteDirection = NPC.direction;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FrankenMarshmallow>(), 100));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 4; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/ZomboidGremlinGore" + numGores).Type);
                    }
                }
            }
        }
    }
}