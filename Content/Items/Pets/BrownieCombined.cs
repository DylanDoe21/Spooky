using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Buffs.Pets;
using Spooky.Content.Items.SpookyHell;
using Spooky.Content.Projectiles.Pets;
using Spooky.Content.Tiles.SpookyHell.Furniture;

namespace Spooky.Content.Items.Pets
{
	public class BrownieCombined : ModItem
	{
		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.Fish);
			Item.width = 22;
			Item.height = 28;
			Item.rare = ModContent.RarityType<SentientRarity>();
			Item.shoot = ModContent.ProjectileType<ColumboPetDisplay>();
			Item.buffType = ModContent.BuffType<ColumboPetCombinedBuff>();
		}

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
			if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(Item.buffType, 2, true);
            }
        }

		public override void AddRecipes()
        {
            CreateRecipe()
			.AddIngredient(ModContent.ItemType<BrownieOrange>(), 1)
			.AddIngredient(ModContent.ItemType<BrownieGhost>(), 1)
			.AddIngredient(ModContent.ItemType<BrownieBone>(), 1)
			.AddIngredient(ModContent.ItemType<BrownieOrganic>(), 1)
			.AddIngredient(ModContent.ItemType<SentientHeart>(), 1)
            .AddTile(ModContent.TileType<Cauldron>())
            .Register();
        }
	}
}