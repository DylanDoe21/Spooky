using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.Projectiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell
{
    public class LivingFleshAxe : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 85;
            Item.crit = 7;
			Item.DamageType = DamageClass.Melee;
            Item.noUseGraphic = true;
			Item.autoReuse = true;
			Item.noMelee = true;
			Item.channel = true;
            Item.width = 90;
            Item.height = 86;
            Item.useTime = 25;
			Item.useAnimation = 25;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 7;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.buyPrice(gold: 15);
            Item.UseSound = SoundID.DD2_MonkStaffSwing;
            Item.shoot = ModContent.ProjectileType<LivingFleshAxeProj>();
            Item.shootSpeed = 12f;
        }

        public override bool MeleePrefix() 
		{
			return true;
		}

        public override bool CanUseItem(Player player)
		{
			return player.ownedProjectileCounts[ModContent.ProjectileType<LivingFleshAxeProj>()] <= 0;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Projectile.NewProjectileDirect(source, position + (velocity * 20) + (velocity.RotatedBy(-1.57f * player.direction) * 20), Vector2.Zero, type, damage, knockback, player.whoAmI, 0);
			
			return false;
		}

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<FleshAxe>(), 1)
			.AddIngredient(ModContent.ItemType<ArteryPiece>(), 15)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
    }
}