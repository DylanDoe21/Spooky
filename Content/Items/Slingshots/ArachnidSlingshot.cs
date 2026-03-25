using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Items.Slingshots.Ammo;
using Spooky.Content.Items.SpiderCave.Misc;
using Spooky.Content.Projectiles.Slingshots;
using Spooky.Content.Tiles.SpiderCave;

namespace Spooky.Content.Items.Slingshots
{
    public class ArachnidSlingshot : ModItem
    {
		public static readonly SoundStyle UseSound = new("Spooky/Content/Sounds/SlingshotDraw", SoundType.Sound);

		public override void SetStaticDefaults()
		{
			ItemGlobal.IsSlingshot[Item.type] = true;
		}

        public override void SetDefaults()
        {
            Item.damage = 85;
			Item.crit = 10;
			Item.DamageType = DamageClass.Ranged;
			Item.noMelee = true;
			Item.autoReuse = true;
			Item.noUseGraphic = true;
			Item.channel = true;
			Item.width = 22;
            Item.height = 32;
			Item.useTime = 32;
			Item.useAnimation = 32;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 4;
			Item.rare = ItemRarityID.Lime;
			Item.value = Item.buyPrice(gold: 35);
			Item.UseSound = UseSound;
			Item.shoot = ModContent.ProjectileType<ArachnidSlingshotProj>();
			Item.useAmmo = ModContent.ItemType<MossyPebble>();
			Item.shootSpeed = 13f;
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
			Projectile.NewProjectile(source, position.X, position.Y, 0, 0, Item.shoot, damage, knockback, player.whoAmI);

			return false;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<SpiderChitin>(), 20)
			.AddIngredient(ItemID.SpiderFang, 10)
			.AddIngredient(ItemID.ChlorophyteBar, 10)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
    }
}