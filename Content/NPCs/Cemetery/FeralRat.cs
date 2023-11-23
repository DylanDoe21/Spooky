using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Content.Buffs.Debuff;

namespace Spooky.Content.NPCs.Cemetery
{
    public class FeralRat1 : ModNPC  
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 5;

            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Position = new Vector2(10f, 0f),
                PortraitPositionXOverride = 10f,
                PortraitPositionYOverride = 0f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 80;
            NPC.damage = 25;
            NPC.width = 32;
			NPC.height = 32;
            NPC.npcSlots = 1f;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.value = Item.buyPrice(0, 0, 0, 50);
            NPC.HitSound = SoundID.NPCHit6;
			NPC.DeathSound = SoundID.NPCDeath4;
            NPC.aiStyle = 26;
            AIType = NPCID.Wolf;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CemeteryBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.FeralRat1"),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.BloodMoon,
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CemeteryBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void FindFrame(int frameHeight)
        {   
            //running animation
            NPC.frameCounter += 1;
            if (NPC.frameCounter > 9 - (NPC.velocity.X > 0 ? NPC.velocity.X : -NPC.velocity.X))
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 5)
            {
                NPC.frame.Y = 0 * frameHeight;
            }

            //jumping frame
            if (NPC.velocity.Y < 0)
            {
                NPC.frame.Y = 1 * frameHeight;
            }

            //falling frame
            if (NPC.velocity.Y > 0)
            {
                NPC.frame.Y = 2 * frameHeight;
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
                for (int numGores = 1; numGores <= 4; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/FeralRatGore" + numGores).Type);
                    }
                }
            }
        }
    }

    public class FeralRat2 : FeralRat1  
    {
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.FeralRat2"),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.BloodMoon,
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CemeteryBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 4; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/FeralRatBrownGore" + numGores).Type);
                    }
                }
            }
        }
    }
}