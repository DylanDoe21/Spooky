using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.Projectiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell
{
    public class LivingFleshStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 55;
			Item.mana = 10;
			Item.DamageType = DamageClass.Magic;
            Item.noMelee = true;
			Item.autoReuse = true;
            Item.channel = true;
			Item.width = 68;
            Item.height = 62;
			Item.useTime = 30;         
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 2;
			Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.buyPrice(gold: 15);
			Item.UseSound = SoundID.Item17;     
			Item.shoot = ModContent.ProjectileType<ControllableEyeBig>();
			Item.shootSpeed = 10f;
        }

        public override bool CanUseItem(Player player)
		{
			return player.ownedProjectileCounts[Item.shoot] < 8;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 45f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }

            Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI, 0f, 0f);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<FleshStaff>(), 1)
			.AddIngredient(ModContent.ItemType<ArteryPiece>(), 15)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
    }
}