using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Items.SpiderCave.Misc;
using Spooky.Content.Projectiles.SpiderCave;

namespace Spooky.Content.Items.SpiderCave
{
    public class MiteTome : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 50;
            Item.mana = 20;
			Item.DamageType = DamageClass.Magic;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.width = 34;
            Item.height = 38;
            Item.useTime = 40;
			Item.useAnimation = 40;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 3;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(gold: 15);
            Item.UseSound = SoundID.Item43;
            Item.shoot = ModContent.ProjectileType<MiteTomeLightning>();
            Item.shootSpeed = 5f;
        }

        public override Vector2? HoldoutOffset()
		{
			return new Vector2(-2, 0);
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
            for (int i = 0; i <= 2; i++)
            {
                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(35));

                Projectile.NewProjectile(source, position, newVelocity, type, damage, knockback, player.whoAmI, newVelocity.ToRotation());
            }

			return false;
		}

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<MiteMandibles>(), 18)
            .AddRecipeGroup("SpookyMod:AdamantiteBars", 8)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
    }
}