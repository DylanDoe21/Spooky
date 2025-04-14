using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using System;
using System.Collections.Generic;

using Spooky.Content.Items.Minibiomes.Vegetable;

namespace Spooky.Content.NPCs.Minibiomes.Vegetable
{
    public class HoppingPotato1 : ModNPC  
    {
        public override void SetDefaults()
		{
            NPC.lifeMax = 65;
            NPC.damage = 25;
            NPC.defense = 5;
            NPC.width = 26;
			NPC.height = 44;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.75f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.HitSound = SoundID.NPCHit11;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = 26;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.VegetableBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.HoppingPotato1"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.VegetableBiome>().ModBiomeBestiaryInfoElement)
			});
        }

        public override void AI()
		{
			NPC.spriteDirection = NPC.direction;

			NPC.rotation += Math.Abs(NPC.velocity.X) * 0.1f * (float)NPC.direction;

			if (NPC.velocity.Y == 0)
			{
				NPC.velocity.X *= 0.5f;

				NPC.localAI[0]++;

				if (NPC.localAI[0] >= 60)
				{
					NPC.velocity.Y = Main.rand.Next(-6, -2);
				}
			}
		}

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                if (Main.netMode != NetmodeID.Server) 
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/HoppingPotato1Gore").Type);
                }
            }
        }
    }

    public class HoppingPotato2 : HoppingPotato1  
    {
        public override void SetDefaults()
		{
            NPC.lifeMax = 70;
            NPC.damage = 25;
            NPC.defense = 5;
            NPC.width = 26;
			NPC.height = 26;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.75f;
			NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.HitSound = SoundID.NPCHit11;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = 26;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.VegetableBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.HoppingPotato2"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.VegetableBiome>().ModBiomeBestiaryInfoElement)
			});
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                if (Main.netMode != NetmodeID.Server) 
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/HoppingPotato2Gore").Type);
                }
            }
        }
    }

    public class HoppingPotato3 : HoppingPotato1  
    {
        public override void SetDefaults()
		{
            NPC.lifeMax = 70;
            NPC.damage = 25;
            NPC.defense = 5;
            NPC.width = 30;
			NPC.height = 34;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.75f;
			NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.HitSound = SoundID.NPCHit11;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = 26;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.VegetableBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.HoppingPotato3"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.VegetableBiome>().ModBiomeBestiaryInfoElement)
			});
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                if (Main.netMode != NetmodeID.Server) 
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/HoppingPotato3Gore").Type);
                }
            }
        }
    }

    public class HoppingPotato4 : HoppingPotato1  
    {
        public override void SetDefaults()
		{
            NPC.lifeMax = 70;
            NPC.damage = 25;
            NPC.defense = 5;
            NPC.width = 32;
			NPC.height = 30;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.75f;
			NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.HitSound = SoundID.NPCHit11;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = 26;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.VegetableBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.HoppingPotato4"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.VegetableBiome>().ModBiomeBestiaryInfoElement)
			});
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                if (Main.netMode != NetmodeID.Server) 
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/HoppingPotato4Gore").Type);
                }
            }
        }
    }
}