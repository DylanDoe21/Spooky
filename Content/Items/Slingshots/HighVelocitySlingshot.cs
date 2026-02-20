using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Items.Slingshots.Ammo;
using Spooky.Content.Projectiles.Slingshots;

namespace Spooky.Content.Items.Slingshots
{
    public class HighVelocitySlingshot : ModItem
    {
		public static readonly SoundStyle UseSound = new("Spooky/Content/Sounds/SlingshotDraw", SoundType.Sound);

		public override void SetStaticDefaults()
		{
			ItemGlobal.IsSlingshot[Item.type] = true;
		}

        public override void SetDefaults()
        {
            Item.damage = 120;
			Item.crit = 50;
			Item.DamageType = DamageClass.Ranged;
			Item.noMelee = true;
			Item.autoReuse = false;
			Item.noUseGraphic = true;
			Item.channel = true;
			Item.width = 22;
            Item.height = 32;
			Item.useTime = 100;
			Item.useAnimation = 100;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 8;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(gold: 10);
			Item.UseSound = UseSound;
			Item.shoot = ModContent.ProjectileType<HighVelocitySlingshotProj>();
			Item.useAmmo = ModContent.ItemType<MossyPebble>();
			Item.shootSpeed = 40f;
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
	}
}