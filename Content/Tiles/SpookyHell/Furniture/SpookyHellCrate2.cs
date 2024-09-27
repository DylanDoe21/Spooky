using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;

using Spooky.Content.Tiles.NoseTemple;
using Spooky.Content.Tiles.NoseTemple.Painting;

namespace Spooky.Content.Tiles.SpookyHell.Furniture
{
	public class SpookyHellCrate2 : ModItem
    {
		public override void SetStaticDefaults() 
		{
			Item.ResearchUnlockCount = 5;
			ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<SpookyHellCrate>();
		}

		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<SpookyHellCrate2Tile>());
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
            itemLoot.Add(ItemDropRule.Common(ItemID.GoldCoin, 4, 1, 7));
		}
	}
}