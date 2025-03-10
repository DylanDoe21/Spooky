using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles;
using Spooky.Content.Projectiles.Cemetery;

namespace Spooky.Content.Items.Cemetery
{
    public class SpiritScroll : ModItem
    {
		public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<SpiritSword>();
        }

        public override void SetDefaults()
        {
            Item.damage = 20;
			Item.mana = 12;
			Item.DamageType = DamageClass.Summon;
			Item.noMelee = true;
			Item.useTurn = false;
			Item.autoReuse = false;
			Item.noUseGraphic = true;
			Item.channel = true;
			Item.width = 52;
            Item.height = 56;
			Item.useTime = 45;
			Item.useAnimation = 45;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 4;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(gold: 3);
			Item.UseSound = SoundID.Item117;
			Item.shoot = ModContent.ProjectileType<SpiritScrollHoldout>();
			Item.shootSpeed = 0f;
        }
		
		public override bool CanUseItem(Player player)
		{
			return player.ownedProjectileCounts[ModContent.ProjectileType<SpiritScrollHoldout>()] < 1;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Projectile.NewProjectile(source, position.X, position.Y, 0, 0, ModContent.ProjectileType<SpiritScrollHoldout>(), damage, knockback, player.whoAmI);

			return false;
		}
    }
}