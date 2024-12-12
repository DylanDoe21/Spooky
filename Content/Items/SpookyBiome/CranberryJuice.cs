using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Items.SpookyBiome
{
	public class CranberryJuice : ModItem
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
            Item.rare = ItemRarityID.LightRed;
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

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<CranberryJelly>(), 1)
            .AddIngredient(ItemID.PixieDust, 3)
            .AddIngredient(ItemID.CrystalShard, 1)
            .AddTile(TileID.Bottles)
            .Register();
        }
    }
}
