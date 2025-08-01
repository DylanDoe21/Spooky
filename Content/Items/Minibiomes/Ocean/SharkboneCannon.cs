using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.Minibiomes.Ocean;

namespace Spooky.Content.Items.Minibiomes.Ocean
{
	public class SharkboneCannon : ModItem
	{
		public override void SetDefaults()
        {
			Item.damage = 25;
			Item.DamageType = DamageClass.Ranged;
			Item.noMelee = true;
			Item.autoReuse = true;
			Item.noUseGraphic = true;
			Item.channel = true;
			Item.width = 42;
			Item.height = 30;
			Item.useTime = 35;
			Item.useAnimation = 35;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 4;
            Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(gold: 1, silver: 50);
			Item.UseSound = SoundID.Item1;
			Item.shoot = ModContent.ProjectileType<SharkboneCannonProj>();
			Item.shootSpeed = 0f;
        }

        public override bool CanUseItem(Player player)
		{
			return player.ownedProjectileCounts[ModContent.ProjectileType<SharkboneCannonProj>()] < 1;
		}
		
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Projectile.NewProjectile(source, position.X, position.Y, 0, 0, ModContent.ProjectileType<SharkboneCannonProj>(), damage, knockback, player.whoAmI);

			return false;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<FishboneChunk>(), 20)
			.AddRecipeGroup(RecipeGroupID.IronBar, 10)
            .AddTile(TileID.Anvils)
            .Register();
        }
	}
}
