using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;

using Spooky.Content.Tiles.Minibiomes.Christmas;

namespace Spooky.Content.Items.Minibiomes.Christmas
{
	public class KrampusRewardPresent1 : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 3;
		}

		public override void SetDefaults()
		{
			Item.width = 44;
			Item.height = 44;
			Item.consumable = true;
			Item.rare = ItemRarityID.Quest;
			Item.maxStack = 9999;
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
			//accessory drops
			int[] Accessories = new int[]
			{
				ModContent.ItemType<KrampusBricks>(),
				ModContent.ItemType<KrampusChimney>(),
				ModContent.ItemType<KrampusJumpShoe>(),
				ModContent.ItemType<KrampusResolution>(),
				ModContent.ItemType<KrampusShapeBox>()
			};

			itemLoot.Add(ItemDropRule.FewFromOptions(2, 1, Accessories));

			//accessory drops
			int[] Weapons = new int[]
			{
				ModContent.ItemType<CursedDoll>(),
				ModContent.ItemType<JackPack>(),
				ModContent.ItemType<JacksBag>(),
				ModContent.ItemType<MarbleJar>(),
				ModContent.ItemType<RobotOven>(),
				ModContent.ItemType<SnakeTrombone>(),
				ModContent.ItemType<SnowBag>(),
				ModContent.ItemType<StockingStaff>(),
			};

			itemLoot.Add(ItemDropRule.FewFromOptions(3, 1, Weapons));

			//bricks
			IItemDropRule[] KrampusBricks = new IItemDropRule[]
			{
				ItemDropRule.Common(ModContent.ItemType<ChristmasBrickBlueItem>(), 1, 250, 500),
				ItemDropRule.Common(ModContent.ItemType<ChristmasBrickGreenItem>(), 1, 250, 500),
				ItemDropRule.Common(ModContent.ItemType<ChristmasBrickRedItem>(), 1, 250, 500),
			};
			itemLoot.Add(new OneFromRulesRule(1, KrampusBricks));

			//slabs
			IItemDropRule[] KrampusSlabs = new IItemDropRule[]
			{
				ItemDropRule.Common(ModContent.ItemType<ChristmasSlabBlueItem>(), 1, 250, 500),
				ItemDropRule.Common(ModContent.ItemType<ChristmasSlabGreenItem>(), 1, 250, 500),
				ItemDropRule.Common(ModContent.ItemType<ChristmasSlabRedItem>(), 1, 250, 500),
			};
			itemLoot.Add(new OneFromRulesRule(1, KrampusSlabs));

			//yuletide wood
			itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<ChristmasWoodItem>(), 1, 250, 500));

			//carpets
			itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<ChristmasCarpetItem>(), 1, 100, 150));

			//windows
			itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<ChristmasWindowItem>(), 1, 70, 140));

			//gold coins
			itemLoot.Add(ItemDropRule.NotScalingWithLuck(ItemID.GoldCoin, 1, 15, 30));
		}
	}

	public class KrampusRewardPresent2 : KrampusRewardPresent1
	{
	}

	public class KrampusRewardPresent3 : KrampusRewardPresent1
	{
	}
}