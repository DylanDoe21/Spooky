using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.SpiderCave
{
    public class DaddyLongLegs : ModNPC  
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 7;
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 100;
            NPC.damage = 25;
            NPC.defense = 5;
            NPC.width = 58;
			NPC.height = 72;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.5f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.HitSound = SoundID.NPCHit29;
			NPC.DeathSound = SoundID.NPCDeath35;
            NPC.aiStyle = 3;
			AIType = NPCID.GoblinScout;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpiderCaveBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.DaddyLongLegs"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpiderCaveBiome>().ModBiomeBestiaryInfoElement)
			});
        }

        public override void FindFrame(int frameHeight)
        {   
            //running animation
            NPC.frameCounter += 1;
            if (NPC.frameCounter > 5 - (NPC.velocity.X > 0 ? NPC.velocity.X : -NPC.velocity.X))
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 7)
            {
                NPC.frame.Y = 0 * frameHeight;
            }

            //jumping frames
            if (NPC.velocity.Y < 0 || NPC.velocity.Y > 0)
            {
                NPC.frame.Y = 4 * frameHeight;
            }
        }

        public override void AI()
		{
			NPC.spriteDirection = NPC.direction;
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 2; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/DaddyLongLegsGore" + numGores).Type);
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/DaddyLongLegsArmGore").Type);
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/DaddyLongLegsLegGore").Type);
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/DaddyLongLegsFootGore").Type);
                    }
                }
            }
        }
    }
}