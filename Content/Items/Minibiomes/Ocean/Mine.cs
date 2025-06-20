using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Projectiles.Minibiomes.Ocean;

namespace Spooky.Content.Items.Minibiomes.Ocean
{
	public class Mine : ModItem
	{
		public override void SetDefaults()
        {
            Item.useTurn = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.autoReuse = true;
			Item.consumable = true;
            Item.width = 44;
            Item.height = 42;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 2;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(gold: 1);
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<MineProj>();
            Item.shootSpeed = 25f;
        }

		public override bool CanUseItem(Player player)
		{
			return player.ownedProjectileCounts[ModContent.ProjectileType<MineProj>()] <= 0;
		}

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<MineDynamite>(), 1)
            .AddIngredient(ModContent.ItemType<MineMetalPlates>(), 1)
            .AddIngredient(ModContent.ItemType<MinePressureSensor>(), 1)
            .AddIngredient(ModContent.ItemType<MineTimer>(), 1)
            .AddIngredient(ItemID.HallowedBar, 15)
            .AddIngredient(ItemID.Wire, 20)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
	}
}
