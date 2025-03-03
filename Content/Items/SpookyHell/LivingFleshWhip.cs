using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.Projectiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell
{
    public class LivingFleshWhip : ModItem
    {
        int numUses = 0;

        public override void SetDefaults()
        {
            Item.damage = 82;
			Item.DamageType = DamageClass.SummonMeleeSpeed;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.width = 50;
            Item.height = 66;
			Item.useTime = 22;
			Item.useAnimation = 22;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 2;
			Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.buyPrice(gold: 15);
			Item.UseSound = SoundID.Item152;
			Item.shoot = ModContent.ProjectileType<LivingFleshWhipProjRed>();
			Item.shootSpeed = 5f;
        }

        public override bool MeleePrefix() 
		{
			return true;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            numUses++;

            if (numUses > 1)
            {
                numUses = 0;
            }

            type = numUses == 0 ? ModContent.ProjectileType<LivingFleshWhipProjRed>() : ModContent.ProjectileType<LivingFleshWhipProjBlue>();

            Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI);
			
			return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<FleshWhip>(), 1)
			.AddIngredient(ModContent.ItemType<ArteryPiece>(), 15)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
    }
}