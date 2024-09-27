using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;

using Spooky.Content.Items.Catacomb;

namespace Spooky.Content.Tiles.Catacomb.Furniture
{
	public class CatacombCrate2 : ModItem
    {
		public override void SetStaticDefaults() 
		{
			Item.ResearchUnlockCount = 5;
			ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<CatacombCrate>();
		}

		public override void SetDefaults() 
		{
            Item.DefaultToPlaceableTile(ModContent.TileType<CatacombCrate2Tile>());
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
			int[] catacombChestDrops = new int[] 
			{
				ModContent.ItemType<GlowBulb>(),
				ModContent.ItemType<OldRifle>(),
				ModContent.ItemType<FlameIdol>(),
				ModContent.ItemType<HunterSoulScepter>(),
				ModContent.ItemType<CrossCharm>()
			};

            itemLoot.Add(ItemDropRule.OneFromOptions(1, catacombChestDrops));

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
            itemLoot.Add(ItemDropRule.Common(ItemID.GoldCoin, 4, 1, 7));
		}
	}
}