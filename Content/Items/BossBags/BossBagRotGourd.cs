using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Items.Blooms.Accessory;
using Spooky.Content.Items.BossBags.Accessory;
using Spooky.Content.Items.Costume;
using Spooky.Content.Items.SpiderCave.Misc;
using Spooky.Content.Items.SpookyBiome.Misc;
using Spooky.Content.NPCs.Boss.RotGourd;

namespace Spooky.Content.Items.BossBags
{
	public class BossBagRotGourd : ModItem
	{
		public override void SetStaticDefaults()
        {
            ItemID.Sets.BossBag[Type] = true;
            ItemID.Sets.PreHardmodeLikeBossBag[Type] = true;
            Item.ResearchUnlockCount = 3;
        }

		public override void SetDefaults()
        {
			Item.width = 34;
			Item.height = 38;
			Item.consumable = true;
			Item.expert = true;
			Item.rare = ItemRarityID.Expert;
			Item.maxStack = 9999;
		}

		public override bool CanRightClick() 
		{
			return true;
		}

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossBags;
		}

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            //material
            itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<RottenChunk>(), 1, 20, 35));

			//pumpkin carving kit
			itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<PumpkinCarvingKit>(), 1));

			//wormy
			itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<Wormy>(), 2));

			//spider grotto compass
			itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<GrottoCompass>(), 1));

			//boss mask
			itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<RotGourdMask>(), 7));

            //expert item
            itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<FlyCharm>(), 1));

            //money
            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<RotGourd>()));
        }
	}
}