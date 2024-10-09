using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Items.SpookyBiome;
using Spooky.Content.Projectiles.Cemetery;

namespace Spooky.Content.Items.Cemetery
{
    public class SpiritSlingshot : ModItem
    {
		public static readonly SoundStyle UseSound = new("Spooky/Content/Sounds/SlingshotDraw", SoundType.Sound);

		public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<SpiritHandStaff>();
        }

        public override void SetDefaults()
        {
            Item.damage = 25;
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
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(gold: 3);
			Item.UseSound = UseSound;
			Item.shoot = ModContent.ProjectileType<SpiritSlingshotProj>();
			Item.useAmmo = ModContent.ItemType<MossyPebble>();
			Item.shootSpeed = 0f;
        }

		public override bool CanUseItem(Player player)
		{
			return player.ownedProjectileCounts[ModContent.ProjectileType<SpiritSlingshotProj>()] < 1;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Projectile.NewProjectile(source, position.X, position.Y, 0, 0, ModContent.ProjectileType<SpiritSlingshotProj>(), damage, knockback, player.whoAmI, 0f, 0f);

			return false;
		}
    }
}