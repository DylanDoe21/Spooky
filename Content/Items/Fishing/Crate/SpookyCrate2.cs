using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Creative;

using Spooky.Core;
using Spooky.Content.Items.SpookyBiome;

namespace Spooky.Content.Items.Fishing.Crate
{
	public class SpookyCrate2 : ModItem
    {
		public override void SetStaticDefaults() 
		{
			Item.ResearchUnlockCount = 5;
			ItemID.Sets.IsFishingCrate[Item.type] = true;
			ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<SpookyCrate>();
		}

		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<SpookyCrate2Tile>());
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
				ModContent.ItemType<CandyBag>(),
				ModContent.ItemType<AutumnLeaf>(),
				ModContent.ItemType<EggCarton>(),
				ModContent.ItemType<WarlockWalkers>(),
				ModContent.ItemType<MoldJar>()
			};

            itemLoot.Add(ItemDropRule.OneFromOptions(1, spookyChestDrops));

			//drop vanilla bars
			IItemDropRule[] oreBars = new IItemDropRule[] 
			{
				ItemDropRule.Common(ItemID.CobaltBar, 1, 2, 10),
				ItemDropRule.Common(ItemID.PalladiumBar, 1, 2, 10),
				ItemDropRule.Common(ItemID.MythrilBar, 1, 2, 10),
				ItemDropRule.Common(ItemID.OrichalcumBar, 1, 2, 10),
				ItemDropRule.Common(ItemID.AdamantiteBar, 1, 2, 5),
				ItemDropRule.Common(ItemID.TitaniumBar, 1, 2, 5),
			};
			itemLoot.Add(new OneFromRulesRule(2, oreBars));

			//drop some potions
			IItemDropRule[] explorationPotions = new IItemDropRule[] 
			{
				ItemDropRule.Common(ItemID.PotionOfReturn, 1, 2, 3),
				ItemDropRule.Common(ItemID.AmmoReservationPotion, 1, 2, 3),
				ItemDropRule.Common(ItemID.EndurancePotion, 1, 2, 3),
				ItemDropRule.Common(ItemID.BiomeSightPotion, 1, 2, 3),
				ItemDropRule.Common(ItemID.LuckPotion, 1, 2, 3),
				ItemDropRule.Common(ItemID.RagePotion, 1, 2, 3),
			};
			itemLoot.Add(new OneFromRulesRule(2, explorationPotions));

			//healing and mana potions
			IItemDropRule[] resourcePotions = new IItemDropRule[] 
			{
				ItemDropRule.Common(ItemID.HealingPotion, 1, 5, 6),
				ItemDropRule.Common(ItemID.ManaPotion, 1, 5, 6),
			};

			itemLoot.Add(new OneFromRulesRule(1, resourcePotions));

			//fishing bait
			IItemDropRule[] highendBait = new IItemDropRule[] 
			{
				ItemDropRule.Common(ItemID.JourneymanBait, 1, 2, 6),
				ItemDropRule.Common(ItemID.MasterBait, 1, 2, 7),
			};
			itemLoot.Add(new OneFromRulesRule(2, highendBait));

			//goodie bags
			itemLoot.Add(ItemDropRule.Common(ItemID.GoodieBag, 2, 2, 4));

			//coins
            itemLoot.Add(ItemDropRule.Common(ItemID.GoldCoin, 3, 1, 5));
		}
	}
}