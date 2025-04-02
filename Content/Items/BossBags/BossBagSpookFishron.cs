using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Items.BossBags.Accessory;
using Spooky.Content.Items.SpookyBiome;
using Spooky.Content.Items.Costume;
using Spooky.Content.NPCs.Boss.SpookFishron;

namespace Spooky.Content.Items.BossBags
{
	public class BossBagSpookFishron : ModItem
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
            //weapon drops
            int[] MainItem = new int[] 
            { 
                ModContent.ItemType<SpookFishronFlail>(), 
                ModContent.ItemType<SpookFishronBow>(), 
                ModContent.ItemType<SpookFishronTome>(), 
                ModContent.ItemType<SpookFishronGun>(),
                ModContent.ItemType<SpookFishronStaff>()
            };

			itemLoot.Add(ItemDropRule.OneFromOptions(1, MainItem));

            //wings
			itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<SpookFishronWings>(), 10));

            //boss mask
			itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<SpookFishronMask>(), 7));

            //expert item
            itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<PumpkinSpiceLatte>(), 1));

            //money
            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<SpookFishron>()));
        }
	}
}