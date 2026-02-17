using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.Minibiomes.Desert;
using Spooky.Content.Tiles.Minibiomes.Desert;

namespace Spooky.Content.Items.Minibiomes.Desert
{
    public class CactusClub : ModItem
    {
        int numUses = -1;

        public override void SetDefaults()
        {
            Item.damage = 25;
			Item.DamageType = DamageClass.Melee;
			Item.noUseGraphic = true;
			Item.autoReuse = true;
			Item.noMelee = true;
			Item.channel = true;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 25;
			Item.useAnimation = 25;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 6;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 1);
            Item.UseSound = SoundID.Item7;
            Item.shoot = ModContent.ProjectileType<CactusClubProj>();
            Item.shootSpeed = 12f;
        }

        public override bool MeleePrefix() 
		{
			return true;
		}

        public override bool CanUseItem(Player player)
		{
			return player.ownedProjectileCounts[Item.shoot] <= 0;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			numUses++;
			if (numUses > 1)
            {
                numUses = 0;
            }

			Projectile.NewProjectileDirect(source, position + (velocity * 20) + (velocity.RotatedBy(-1.57f * player.direction) * 20), Vector2.Zero, type, damage, knockback, player.whoAmI, numUses == 0 ? 0 : 1);
			
			return false;
		}

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<TarPitCactusBlockItem>(), 20)
            .AddRecipeGroup(RecipeGroupID.IronBar, 12)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }
}