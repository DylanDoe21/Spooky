using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Items.Slingshots.Ammo;
using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.Projectiles.Slingshots;

namespace Spooky.Content.Items.Slingshots
{
    public class StickyHand : ModItem
    {
		public static readonly SoundStyle UseSound = new("Spooky/Content/Sounds/SlingshotDraw", SoundType.Sound);

		public override void SetStaticDefaults()
        {
			ItemGlobal.IsSlingshot[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 65;
			Item.crit = 10;
			Item.DamageType = DamageClass.Ranged;
			Item.noMelee = true;
			Item.autoReuse = false;
			Item.noUseGraphic = true;
			Item.channel = true;
			Item.width = 24;
            Item.height = 34;
			Item.useTime = 45;
			Item.useAnimation = 45;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 6;
			Item.rare = ItemRarityID.Orange;
           	Item.value = Item.buyPrice(gold: 15);
			Item.UseSound = UseSound;
			Item.shoot = ModContent.ProjectileType<StickyHandProj>();
			Item.useAmmo = ModContent.ItemType<MossyPebble>();
			Item.shootSpeed = 25f;
        }

		public override bool CanConsumeAmmo(Item ammo, Player player)
		{
			return player.ItemUsesThisAnimation != 0;
		}

		public override bool CanUseItem(Player player)
		{
			return player.ownedProjectileCounts[Item.shoot] < 1;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Projectile.NewProjectile(source, position.X, position.Y, 0, 0, Item.shoot, damage, knockback, player.whoAmI, 0f, 0f);

			return false;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddRecipeGroup("SpookyMod:DemoniteBars", 5)
			.AddIngredient(ModContent.ItemType<SnotGlob>(), 12)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
    }
}