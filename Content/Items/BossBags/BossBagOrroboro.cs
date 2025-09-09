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
using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.NPCs.Boss.Orroboro;

namespace Spooky.Content.Items.BossBags
{
	public class BossBagOrroboro : ModItem
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
			//weapon
			int[] MainItem = new int[] 
			{ 
				ModContent.ItemType<EyeFlail>(), 
				ModContent.ItemType<Scycler>(), 
				ModContent.ItemType<EyeRocketLauncher>(),
				ModContent.ItemType<MouthFlamethrower>(), 
				ModContent.ItemType<LeechStaff>(), 
				ModContent.ItemType<LeechWhip>() 
			};

            itemLoot.Add(ItemDropRule.FewFromOptions(2, 1, MainItem));

            //material
            itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<ArteryPiece>(), 1, 20, 35));

			//boss masks
			itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<OrroMask>(), 7));
			itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<BoroMask>(), 7));

            //expert item
            itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<OrroboroEmbryo>(), 1));

            //money
            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<OrroHeadP1>()));
        }
	}
}