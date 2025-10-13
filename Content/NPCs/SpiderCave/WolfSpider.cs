using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Content.Items.SpiderCave.Misc;

namespace Spooky.Content.NPCs.SpiderCave
{
    public class WolfSpider : ModNPC  
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 6;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Position = new Vector2(23f, 0f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = 0f
            };
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 600;
            NPC.damage = 25;
            NPC.defense = 5;
            NPC.width = 76;
			NPC.height = 76;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.HitSound = SoundID.NPCHit29 with { Pitch = -0.5f };
			NPC.DeathSound = SoundID.NPCDeath41 with { Pitch = -0.5f };
            NPC.aiStyle = 3;
			AIType = NPCID.GoblinScout;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpiderCaveBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.WolfSpider"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpiderCaveBiome>().ModBiomeBestiaryInfoElement)
			});
        }

        public override void FindFrame(int frameHeight)
        {   
            //running animation
            NPC.frameCounter++;
            if (NPC.frameCounter > 7 - (NPC.velocity.X > 0 ? NPC.velocity.X : -NPC.velocity.X))
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 6)
            {
                NPC.frame.Y = 0 * frameHeight;
            }

            //jumping frame
            if (NPC.velocity.Y < 0)
            {
                NPC.frame.Y = 2 * frameHeight;
            }

            //falling frame
            if (NPC.velocity.Y > 0)
            {
                NPC.frame.Y = 5 * frameHeight;
            }
        }

        public override void AI()
		{
			NPC.spriteDirection = NPC.direction;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SpiderChitin>(), 3, 1, 3));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 8; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server)
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/WolfSpiderGore" + numGores).Type);
                    }
                }
            }
        }
    }
}