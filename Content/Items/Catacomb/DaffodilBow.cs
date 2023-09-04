using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Items.Catacomb.Misc;
using Spooky.Content.Items.SpookyBiome.Misc;
using Spooky.Content.Projectiles;
using Spooky.Content.Projectiles.Catacomb;

namespace Spooky.Content.Items.Catacomb
{
	public class DaffodilBow : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 45;
			Item.DamageType = DamageClass.Ranged;
			Item.noMelee = true;
			Item.autoReuse = false;
			Item.noUseGraphic = true;
			Item.channel = true;
			Item.width = 26;    
			Item.height = 38;         
			Item.useTime = 32;         
			Item.useAnimation = 32;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 2;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.buyPrice(gold: 15);
			Item.shoot = ModContent.ProjectileType<Blank>();
			Item.useAmmo = AmmoID.Arrow;
			Item.shootSpeed = 0f;
		}

		public override bool CanUseItem(Player player)
		{
			return player.ownedProjectileCounts[ModContent.ProjectileType<DaffodilBowProj>()] < 1;
		}
		
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			if (player.ownedProjectileCounts[ModContent.ProjectileType<DaffodilBowProj>()] < 1)
            {
				Projectile.NewProjectile(source, position.X, position.Y, 0, 0, ModContent.ProjectileType<DaffodilBowProj>(), damage, knockback, player.whoAmI, 0f, 0f);
			}

			return false;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<PlantChunk>(), 18)
			.AddIngredient(ModContent.ItemType<FlyBigItem>(), 5)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
	}
}
