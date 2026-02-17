using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Items.Cemetery;
using Spooky.Content.Items.Slingshots.Ammo;
using Spooky.Content.Projectiles.Slingshots;

namespace Spooky.Content.Items.Slingshots
{
    public class OpalSlingshot : ModItem
    {
		public static readonly SoundStyle UseSound = new("Spooky/Content/Sounds/SlingshotDraw", SoundType.Sound);

		public override void SetStaticDefaults()
        {
			ItemGlobal.IsSlingshot[Item.type] = true;
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<SpiritHandStaff>();
        }

        public override void SetDefaults()
        {
            Item.damage = 62;
			Item.crit = 10;
			Item.DamageType = DamageClass.Ranged;
			Item.noMelee = true;
			Item.autoReuse = false;
			Item.noUseGraphic = true;
			Item.channel = true;
			Item.width = 24;
            Item.height = 34;
			Item.useTime = 60;
			Item.useAnimation = 60;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 4;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.buyPrice(gold: 15);
			Item.UseSound = UseSound;
			Item.shoot = ModContent.ProjectileType<OpalSlingshotProj>();
			Item.useAmmo = ModContent.ItemType<MossyPebble>();
			Item.shootSpeed = 16f;
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