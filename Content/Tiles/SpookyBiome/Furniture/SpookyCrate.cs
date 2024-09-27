using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Creative;

using Spooky.Core;
using Spooky.Content.Items.SpookyBiome;

namespace Spooky.Content.Tiles.SpookyBiome.Furniture
{
	public class SpookyCrate : ModItem
    {
		public override void SetStaticDefaults() 
		{
			Item.ResearchUnlockCount = 5;
		}

		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<SpookyCrateTile>());
            Item.width = 34;
			Item.height = 34;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(gold: 1);
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
			int[] spookyChestDrops = new int[] 
			{
				ModContent.ItemType<ToiletPaper>(),
				ModContent.ItemType<LeafBlower>(),
				ModContent.ItemType<NecromancyTome>(),
				ModContent.ItemType<CreepyCandle>(),
				ModContent.ItemType<CandyBag>()
			};

            itemLoot.Add(ItemDropRule.OneFromOptions(1, spookyChestDrops));

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

			//goodie bags
			itemLoot.Add(ItemDropRule.Common(ItemID.GoodieBag, 2, 1, 2));

			//coins
			itemLoot.Add(ItemDropRule.Common(ItemID.GoldCoin, 4, 1, 3));
		}
	}
}