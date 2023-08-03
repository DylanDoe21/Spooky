using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Content.Items.Food;
using Spooky.Content.Items.Pets;
using Spooky.Content.Tiles.Banner.SpookyForest;

namespace Spooky.Content.NPCs.SpookyBiome
{
    public class FluffBatBig1 : ModNPC  
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;

            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Position = new Vector2(0f, -10f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = -30f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 55;
            NPC.damage = 18;
            NPC.width = 60;
			NPC.height = 38;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.8f;
            NPC.value = Item.buyPrice(0, 0, 0, 50);
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath4;
            NPC.aiStyle = 14;
			AIType = NPCID.GiantBat;
            AnimationType = NPCID.GiantBat;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<FluffBatGiantBanner>();
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyBiomeUg>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.FluffBatBig1"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyBiomeUg>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void AI()
		{
			NPC.spriteDirection = NPC.direction;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FuzzBatBlood>(), 20));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<VampireGummy>(), 100));
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
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/BigPurpleBatGore" + numGores).Type);
                }
            }
        }
    }

    public class FluffBatBig2 : FluffBatBig1  
    {
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.FluffBatBig2"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyBiomeUg>().ModBiomeBestiaryInfoElement)
			});
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
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/BigBlackBatGore" + numGores).Type);
                }
            }
        }
    }
}