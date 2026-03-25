using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Content.Items.Catacomb.Misc;
using Spooky.Content.Items.Food;

namespace Spooky.Content.NPCs.Catacomb.Layer2
{
	public class LilySlime1Big : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 2;
		}

		public override void SetDefaults()
		{
            NPC.lifeMax = 180;
            NPC.damage = 35;
            NPC.defense = 5;
            NPC.width = 50;
            NPC.height = 40;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.5f;
			NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.Grass;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = 1;
			AIType = NPCID.HoppinJack;
			AnimationType = NPCID.HoppinJack;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CatacombBiome2>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.LilySlime1Big"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome2>().ModBiomeBestiaryInfoElement)
			});
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CatacombChestKeyLower>(), 50));
            npcLoot.Add(ItemDropRule.Common(ItemID.Gel, 1, 1, 3));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CandyCorn>(), 100));
        }

		public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 3; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/LilyLeafGore" + Main.rand.Next(1, 3)).Type);
                    }
                }
            }
		}
    }

	public class LilySlime2Big : LilySlime1Big
	{
		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.LilySlime2Big"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome2>().ModBiomeBestiaryInfoElement)
			});
		}
	}

	public class LilySlime1Small : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 2;
		}

		public override void SetDefaults()
		{
            NPC.lifeMax = 90;
            NPC.damage = 30;
            NPC.defense = 5;
            NPC.width = 30;
            NPC.height = 26;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.5f;
			NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.Grass;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = 1;
			AIType = NPCID.HoppinJack;
			AnimationType = NPCID.HoppinJack;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CatacombBiome2>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.LilySlime1Small"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome2>().ModBiomeBestiaryInfoElement)
			});
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CatacombChestKeyLower>(), 50));
            npcLoot.Add(ItemDropRule.Common(ItemID.Gel, 1, 1, 3));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CandyCorn>(), 100));
        }

		public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 3; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/LilyLeafGoreSmall" + Main.rand.Next(1, 3)).Type);
                    }
                }
            }
		}
	}

	public class LilySlime2Small : LilySlime1Small
	{
		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.LilySlime2Small"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome2>().ModBiomeBestiaryInfoElement)
			});
		}
	}
}