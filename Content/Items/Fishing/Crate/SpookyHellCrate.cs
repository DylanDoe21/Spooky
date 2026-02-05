using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;

using Spooky.Content.Tiles.NoseTemple;
using Spooky.Content.Tiles.Painting;

namespace Spooky.Content.Items.Fishing.Crate
{
	public class SpookyHellCrate : ModItem
    {
		public override void SetStaticDefaults() 
		{
			Item.ResearchUnlockCount = 5;
			ItemID.Sets.IsFishingCrate[Item.type] = true;
		}

		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<SpookyHellCrateTile>());
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
			//nose temple blocks
			IItemDropRule[] TempleBricks = new IItemDropRule[]
			{
				ItemDropRule.Common(ModContent.ItemType<NoseTempleBrickGrayItem>(), 1, 20, 50),
				ItemDropRule.Common(ModContent.ItemType<NoseTempleBrickGreenItem>(), 1, 20, 50),
				ItemDropRule.Common(ModContent.ItemType<NoseTempleBrickPurpleItem>(), 1, 20, 50),
				ItemDropRule.Common(ModContent.ItemType<NoseTempleBrickRedItem>(), 1, 20, 50),
			};
			itemLoot.Add(new OneFromRulesRule(3, TempleBricks));

			//nose temple paintings
			int[] Paintings = new int[]
			{
				ModContent.ItemType<AmbushPaintingItem>(),
				ModContent.ItemType<BaxterPaintingItem>(),
				ModContent.ItemType<LuigiPaintingItem>(),
				ModContent.ItemType<MonalumboItem>(),
				ModContent.ItemType<RedMistPaintingItem>(),
				ModContent.ItemType<ShadowEyePaintingItem>()
			};
			itemLoot.Add(ItemDropRule.OneFromOptions(5, Paintings));

			//drop vanilla ores
			IItemDropRule[] oreTypes = new IItemDropRule[] 
			{
				ItemDropRule.Common(ItemID.DemoniteOre, 1, 8, 15),
				ItemDropRule.Common(ItemID.CrimtaneOre, 1, 8, 15),
			};
			itemLoot.Add(new OneFromRulesRule(7, oreTypes));

			//drop vanilla bars
			IItemDropRule[] oreBars = new IItemDropRule[] 
			{
				ItemDropRule.Common(ItemID.DemoniteBar, 1, 2, 10),
				ItemDropRule.Common(ItemID.CrimtaneBar, 1, 2, 10),
			};
			itemLoot.Add(new OneFromRulesRule(4, oreBars));

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
            itemLoot.Add(ItemDropRule.Common(ItemID.GoldCoin, 3, 1, 5));
		}
	}
}