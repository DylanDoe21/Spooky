using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Items.BossBags.Accessory;
using Spooky.Content.Items.Costume;
using Spooky.Content.Items.SpookyHell;
using Spooky.Content.NPCs.Boss.Moco;

namespace Spooky.Content.Items.BossBags
{
	public class BossBagMoco : ModItem
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
				ModContent.ItemType<BoogerFlail>(), 
				ModContent.ItemType<BoogerBlaster>(), 
				ModContent.ItemType<BoogerStaff>() 
			};

            itemLoot.Add(ItemDropRule.OneFromOptions(1, MainItem));

			//boss mask
            itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<MocoMask>(), 7));

            //expert item
            itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<MocoNose>(), 1));

            //money
            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<Moco>()));
        }
	}
}