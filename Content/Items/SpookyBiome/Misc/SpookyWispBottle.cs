using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Content.Projectiles.Pets;
using Spooky.Content.Buffs.Pets;

namespace Spooky.Content.Items.SpookyBiome.Misc
{
	public class SpookyWispBottle : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Spooky Wisp Jar");
			Tooltip.SetDefault("Summons a spooky wisp pet to provide light");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.Fish);
			Item.width = 20;
			Item.height = 30;
			Item.rare = ItemRarityID.Blue;
			Item.shoot = ModContent.ProjectileType<SpookyWisp>();
			Item.buffType = ModContent.BuffType<SpookyWispBuff>();
		}

		public override bool CanUseItem(Player player)
		{
			return player.miscEquips[1].IsAir;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.Bottle)
            .AddIngredient(ModContent.ItemType<HalloweenWispItem>(), 10)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
	}
}