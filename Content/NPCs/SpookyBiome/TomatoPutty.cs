using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.Slingshots.Ammo;
using Spooky.Content.Tiles.Blooms;

namespace Spooky.Content.NPCs.SpookyBiome
{
	public class TomatoPutty1 : ModNPC
	{
        public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 2;
		}

		public override void SetDefaults()
		{
            NPC.lifeMax = 55;
            NPC.damage = 20;
            NPC.defense = 0;
            NPC.width = 34;
            NPC.height = 24;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.25f;
			NPC.value = Item.buyPrice(0, 0, 0, 25);
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.Item177;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = 1;
			AIType = NPCID.Crimslime;
			AnimationType = NPCID.Crimslime;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.TomatoPutty1"),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.BloodMoon,
				new BestiaryBackgroundOverlay("Spooky/Content/Biomes/SpookyBiomeBloodMoon_Background", Color.White)
			});
		}

		public override void AI()
		{
			NPC.spriteDirection = NPC.direction;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.Gel, 1, 1, 3));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<TomatoAmmo>(), 20, 25, 50));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<TomatoSeed>(), 120));
        }

		public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numDusts = 0; numDusts < 20; numDusts++)
                {
                    int DustGore = Dust.NewDust(NPC.Center, 1, 1, DustID.TintableDust, 0f, 0f, 100, Color.Red, 1.25f);
                    Main.dust[DustGore].velocity *= 1.2f;
                }
            }
		}
    }

	public class TomatoPutty2 : TomatoPutty1
	{
		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.TomatoPutty2"),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.BloodMoon,
				new BestiaryBackgroundOverlay("Spooky/Content/Biomes/SpookyBiomeBloodMoon_Background", Color.White)
			});
		}
	}

	public class TomatoPutty3 : TomatoPutty1
	{
		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.TomatoPutty3"),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.BloodMoon,
				new BestiaryBackgroundOverlay("Spooky/Content/Biomes/SpookyBiomeBloodMoon_Background", Color.White)
			});
		}
	}

    public class TomatoPutty4 : TomatoPutty1
	{
		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.TomatoPutty4"),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.BloodMoon,
				new BestiaryBackgroundOverlay("Spooky/Content/Biomes/SpookyBiomeBloodMoon_Background", Color.White)
			});
		}
	}
}