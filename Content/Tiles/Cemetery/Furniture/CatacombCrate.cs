using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Creative;

using Spooky.Core;
using Spooky.Content.Items.Catacomb;

namespace Spooky.Content.Tiles.Cemetery.Furniture
{
	public class CatacombCrate : ModItem
    {
		public override void SetStaticDefaults() 
		{
			Item.ResearchUnlockCount = 5;
		}

		public override void SetDefaults() 
		{
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
            Item.width = 34;
			Item.height = 34;
			Item.useTime = 15;
			Item.useAnimation = 15;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.maxStack = 9999;
			Item.rare = ItemRarityID.Green;  
			Item.value = Item.buyPrice(gold: 1);
			Item.createTile = ModContent.TileType<CatacombCrateTile>();
		}

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup) 
		{
			itemGroup = ContentSamples.CreativeHelper.ItemGroup.Crates;
		}

		public override bool CanRightClick() 
		{
			return true;
		}

		public override void ModifyItemLoot(ItemLoot itemLoot) 
		{
			//main items
			int[] catacombChestDrops = new int[] 
			{
				ModContent.ItemType<HarvesterScythe>(),
				ModContent.ItemType<GraveCrossbow>(),
				ModContent.ItemType<ThornStaff>(),
				ModContent.ItemType<NineTails>()
			};

            itemLoot.Add(ItemDropRule.OneFromOptions(1, catacombChestDrops));

            //drop vanilla ores
            IItemDropRule[] oreTypes = new IItemDropRule[] 
			{
				ItemDropRule.Common(ItemID.CopperOre, 1, 8, 15),
				ItemDropRule.Common(ItemID.TinOre, 1, 8, 15),
				ItemDropRule.Common(ItemID.IronOre, 1, 8, 15),
				ItemDropRule.Common(ItemID.LeadOre, 1, 8, 15),
				ItemDropRule.Common(ItemID.SilverOre, 1, 8, 15),
				ItemDropRule.Common(ItemID.TungstenOre, 1, 8, 15),
				ItemDropRule.Common(ItemID.GoldOre, 1, 8, 15),
				ItemDropRule.Common(ItemID.PlatinumOre, 1, 8, 15),
			};
			itemLoot.Add(new OneFromRulesRule(7, oreTypes));

			//drop vanilla bars
			IItemDropRule[] oreBars = new IItemDropRule[] 
			{
				ItemDropRule.Common(ItemID.IronBar, 1, 2, 10),
				ItemDropRule.Common(ItemID.LeadBar, 1, 2, 10),
				ItemDropRule.Common(ItemID.SilverBar, 1, 2, 10),
				ItemDropRule.Common(ItemID.TungstenBar, 1, 2, 10),
				ItemDropRule.Common(ItemID.GoldBar, 1, 2, 10),
				ItemDropRule.Common(ItemID.PlatinumBar, 1, 2, 10),
			};
			itemLoot.Add(new OneFromRulesRule(4, oreBars));

			//drop some potions
			IItemDropRule[] explorationPotions = new IItemDropRule[] 
			{
				ItemDropRule.Common(ItemID.ObsidianSkinPotion, 1, 2, 3),
				ItemDropRule.Common(ItemID.SpelunkerPotion, 1, 2, 3),
				ItemDropRule.Common(ItemID.HunterPotion, 1, 2, 3),
				ItemDropRule.Common(ItemID.GravitationPotion, 1, 2, 3),
				ItemDropRule.Common(ItemID.MiningPotion, 1, 2, 3),
				ItemDropRule.Common(ItemID.HeartreachPotion, 1, 2, 3),
			};
			itemLoot.Add(new OneFromRulesRule(4, explorationPotions));

			//healing and mana potions
			IItemDropRule[] resourcePotions = new IItemDropRule[] 
			{
				ItemDropRule.Common(ItemID.HealingPotion, 1, 5, 6),
				ItemDropRule.Common(ItemID.ManaPotion, 1, 5, 6),
			};

			itemLoot.Add(new OneFromRulesRule(2, resourcePotions));

			//fishing bait
			IItemDropRule[] highendBait = new IItemDropRule[] 
			{
				ItemDropRule.Common(ItemID.JourneymanBait, 1, 2, 6),
				ItemDropRule.Common(ItemID.MasterBait, 1, 2, 7),
			};
			itemLoot.Add(new OneFromRulesRule(2, highendBait));

			//coins
			itemLoot.Add(ItemDropRule.Common(ItemID.GoldCoin, 4, 1, 3));
		}
	}
}