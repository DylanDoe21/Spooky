using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Items.Blooms.Accessory;
using Spooky.Content.Items.BossBags.Accessory;
using Spooky.Content.Items.Cemetery;
using Spooky.Content.Items.Cemetery.Armor;
using Spooky.Content.Items.Costume;
using Spooky.Content.Items.Pets;
using Spooky.Content.Items.Slingshots;
using Spooky.Content.NPCs.Boss.SpookySpirit;

namespace Spooky.Content.Items.BossBags
{
	public class BossBagSpookySpirit : ModItem
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
                ModContent.ItemType<SpiritSword>(), 
                ModContent.ItemType<SpiritSlingshot>(), 
                ModContent.ItemType<SpiritHandStaff>(), 
                ModContent.ItemType<SpiritScroll>()
            };

			itemLoot.Add(ItemDropRule.OneFromOptions(1, MainItem));

            int[] ArmorPieces = new int[] 
            { 
                ModContent.ItemType<SpiritHorsemanHead>(), 
                ModContent.ItemType<SpiritHorsemanBody>(), 
                ModContent.ItemType<SpiritHorsemanLegs>()
            };

            itemLoot.Add(ItemDropRule.OneFromOptions(1, ArmorPieces));

            //mask
			itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<TheMask>(), 2));

            //chalupo pet
			itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<ChalupoPepper>(), 15));

            //boss mask
			itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<SpookySpiritMask>(), 7));

            //expert item
            itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<SpiritAmulet>(), 1));

            //money
            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<SpookySpirit>()));
        }
	}
}