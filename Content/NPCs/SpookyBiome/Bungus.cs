using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using System.Collections.Generic;

using Spooky.Content.Items.SpookyBiome;

namespace Spooky.Content.NPCs.SpookyBiome
{
    public class Bungus : ModNPC  
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 8;
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 35;
            NPC.damage = 20;
            NPC.defense = 5;
            NPC.width = 46;
			NPC.height = 48;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.75f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = 3;
			AIType = NPCID.GoblinScout;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyBiomeUg>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.Bungus"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyBiomeUg>().ModBiomeBestiaryInfoElement)
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
            if (NPC.frame.Y >= frameHeight * 8)
            {
                NPC.frame.Y = 0 * frameHeight;
            }

            //jumping frame
            if (NPC.velocity.Y > 0 || NPC.velocity.Y < 0)
            {
                NPC.frame.Y = 1 * frameHeight;
            }
        }
        
        public override void AI()
		{
			NPC.spriteDirection = NPC.direction;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BustlingGlowshroom>(), 30));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 3; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/BungusGore" + numGores).Type);
                    }
                }
            }
        }
    }
}