using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.Minibiomes.Desert;
using Spooky.Content.Tiles.Minibiomes.Desert;

namespace Spooky.Content.Items.Minibiomes.Desert
{
    public class CactusGlove : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 5;
            Item.mana = 5;
			Item.DamageType = DamageClass.Magic;
            Item.autoReuse = true;
            Item.noMelee = true;
			Item.noUseGraphic = true;
            Item.width = 28;
            Item.height = 30;
            Item.useTime = 5;
			Item.useAnimation = 5;
			Item.useStyle = ItemUseStyleID.Rapier;
			Item.knockBack = 1;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 1);
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<CactusGloveProj>();
            Item.shootSpeed = 3.5f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(18));

			Projectile.NewProjectile(source, position, newVelocity, type, damage, knockback, player.whoAmI);
            int Needle = Projectile.NewProjectile(source, position, newVelocity * 2, ModContent.ProjectileType<CactusNeedle>(), damage, knockback, player.whoAmI);
            Main.projectile[Needle].DamageType = DamageClass.Magic;
			
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