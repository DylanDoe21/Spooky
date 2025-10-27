using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Content.Items.SpiderCave.Misc;
using Spooky.Content.Projectiles.SpiderCave;

namespace Spooky.Content.Items.SpiderCave
{
    public class MiteClaws : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 20;
			Item.DamageType = DamageClass.Melee;
			Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.width = 44;
            Item.height = 52;
            Item.useTime = 8;
			Item.useAnimation = 8;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 1;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(gold: 15);
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<MiteClawSlash>();
            Item.shootSpeed = 1f;
        }

        public override bool MeleePrefix() 
		{
			return true;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 75f;
            position += muzzleOffset;

			int newProj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            Main.projectile[newProj].rotation = Main.rand.Next(0, 361);
            Main.projectile[newProj].scale = Main.rand.NextFloat(1f, 1.5f);

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