using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Items.Catacomb;
using Spooky.Content.Items.Minibiomes.Ocean;
using Spooky.Content.Items.Slingshots.Ammo;
using Spooky.Content.Projectiles.Slingshots;

namespace Spooky.Content.Items.Slingshots
{
    public class BigBoneSlingshot : ModItem
    {
		public static readonly SoundStyle UseSound = new("Spooky/Content/Sounds/SlingshotDraw", SoundType.Sound);

		public override void SetStaticDefaults()
        {
			ItemGlobal.IsSlingshot[Item.type] = true;
			ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<BigBoneStaff>();
        }

        public override void SetDefaults()
        {
            Item.damage = 160;
			Item.crit = 10;
			Item.DamageType = DamageClass.Ranged;
			Item.noMelee = true;
			Item.autoReuse = true;
			Item.noUseGraphic = true;
			Item.channel = true;
			Item.width = 24;
            Item.height = 34;
			Item.useTime = 45;
			Item.useAnimation = 45;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 4;
			Item.rare = ItemRarityID.Yellow;
			Item.value = Item.buyPrice(gold: 35);
			Item.UseSound = UseSound;
			Item.shoot = ModContent.ProjectileType<BigBoneSlingshotProj>();
			Item.useAmmo = ModContent.ItemType<MossyPebble>();
			Item.shootSpeed = 22f;
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
    }
}