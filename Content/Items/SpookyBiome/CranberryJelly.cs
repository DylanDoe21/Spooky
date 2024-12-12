using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Items.SpookyBiome
{
	public class CranberryJelly : ModItem
	{
        public override void SetStaticDefaults() 
        {
			Item.ResearchUnlockCount = 30;
		}

		public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 40;
            Item.consumable = true;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.Item3;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.maxStack = 9999;
			Item.scale = 0.75f;
        }

        public override bool CanUseItem(Player player)
        {
            return true;
        }

		public override bool? UseItem(Player player)
		{
			return true;
		}
    }
}
