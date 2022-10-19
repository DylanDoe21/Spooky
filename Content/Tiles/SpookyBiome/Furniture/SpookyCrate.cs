using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Creative;

using Spooky.Content.Items.SpookyBiome;
using Spooky.Content.Items.Vinyl;

namespace Spooky.Content.Tiles.SpookyBiome.Furniture
{
	public class SpookyCrate : ModItem
    {
		public override void SetStaticDefaults() 
		{
			DisplayName.SetDefault("Spooky Wood Crate");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 10;
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
			Item.useStyle = 1;
			Item.maxStack = 99;
			Item.rare = ItemRarityID.Green;  
			Item.value = Item.buyPrice(gold: 1);
			Item.createTile = ModContent.TileType<SpookyCrateTile>();
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
			int[] spookyChestDrops = new int[] 
			{
				ModContent.ItemType<ToiletPaper>(),
				ModContent.ItemType<LeafBlower>(),
				ModContent.ItemType<NecromancyTome>(),
				ModContent.ItemType<CreepyCandle>(),
				ModContent.ItemType<CandyBag>()
			};
			itemLoot.Add(ItemDropRule.OneFromOptionsNotScalingWithLuck(1, spookyChestDrops));

			//rarely drop one of the vinyl discs
			int[] vinylDiscs = new int[] 
			{
				ModContent.ItemType<VinylAlley>(),
				ModContent.ItemType<VinylLazy>(),
				ModContent.ItemType<VinylMysterious>(),
				ModContent.ItemType<VinylSleepy>()
			};
			itemLoot.Add(ItemDropRule.OneFromOptionsNotScalingWithLuck(45, vinylDiscs));

			itemLoot.Add(ItemDropRule.Common(ItemID.GoldCoin, 4, 5, 13));

			IItemDropRule[] oreTypes = new IItemDropRule[] 
			{
				ItemDropRule.Common(ItemID.CopperOre, 1, 30, 50),
				ItemDropRule.Common(ItemID.TinOre, 1, 30, 50),
				ItemDropRule.Common(ItemID.IronOre, 1, 30, 50),
				ItemDropRule.Common(ItemID.LeadOre, 1, 30, 50),
				ItemDropRule.Common(ItemID.SilverOre, 1, 30, 50),
				ItemDropRule.Common(ItemID.TungstenOre, 1, 30, 50),
				ItemDropRule.Common(ItemID.GoldOre, 1, 30, 50),
				ItemDropRule.Common(ItemID.PlatinumOre, 1, 30, 50),
			};
			itemLoot.Add(new OneFromRulesRule(7, oreTypes));

			IItemDropRule[] oreBars = new IItemDropRule[] 
			{
				ItemDropRule.Common(ItemID.IronBar, 1, 10, 21),
				ItemDropRule.Common(ItemID.LeadBar, 1, 10, 21),
				ItemDropRule.Common(ItemID.SilverBar, 1, 10, 21),
				ItemDropRule.Common(ItemID.TungstenBar, 1, 10, 21),
				ItemDropRule.Common(ItemID.GoldBar, 1, 10, 21),
				ItemDropRule.Common(ItemID.PlatinumBar, 1, 10, 21),
			};
			itemLoot.Add(new OneFromRulesRule(4, oreBars));

			IItemDropRule[] explorationPotions = new IItemDropRule[] 
			{
				ItemDropRule.Common(ItemID.ObsidianSkinPotion, 1, 2, 5),
				ItemDropRule.Common(ItemID.SpelunkerPotion, 1, 2, 5),
				ItemDropRule.Common(ItemID.HunterPotion, 1, 2, 5),
				ItemDropRule.Common(ItemID.GravitationPotion, 1, 2, 5),
				ItemDropRule.Common(ItemID.MiningPotion, 1, 2, 5),
				ItemDropRule.Common(ItemID.HeartreachPotion, 1, 2, 5),
			};
			itemLoot.Add(new OneFromRulesRule(4, explorationPotions));

			IItemDropRule[] resourcePotions = new IItemDropRule[] 
			{
				ItemDropRule.Common(ItemID.HealingPotion, 1, 5, 18),
				ItemDropRule.Common(ItemID.ManaPotion, 1, 5, 18),
			};

			itemLoot.Add(new OneFromRulesRule(2, resourcePotions));

			IItemDropRule[] highendBait = new IItemDropRule[] 
			{
				ItemDropRule.Common(ItemID.JourneymanBait, 1, 2, 7),
				ItemDropRule.Common(ItemID.MasterBait, 1, 2, 7),
			};
			itemLoot.Add(new OneFromRulesRule(2, highendBait));
		}
	}
}