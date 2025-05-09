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
using Spooky.Content.Items.Catacomb;
using Spooky.Content.NPCs.Boss.Daffodil;

namespace Spooky.Content.Items.BossBags
{
	public class BossBagDaffodil : ModItem
	{
		public override void SetStaticDefaults()
        {
            ItemID.Sets.BossBag[Type] = true;
            ItemID.Sets.PreHardmodeLikeBossBag[Type] = false;
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
				ModContent.ItemType<DaffodilBlade>(),
                ModContent.ItemType<DaffodilBow>(), 
				ModContent.ItemType<DaffodilRod>(),
                ModContent.ItemType<DaffodilStaff>()
			};

            itemLoot.Add(ItemDropRule.OneFromOptions(1, MainItem));

			//farmer glove
            itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<FarmerGlove>(), 2));

			//boss mask
            itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<DaffodilMask>(), 7));

            //expert item
            itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<DaffodilHairpin>(), 1));

            //money
            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<DaffodilEye>()));
        }
	}
}