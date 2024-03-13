using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Projectiles.Cemetery;

namespace Spooky.Content.Items.Cemetery
{
    public class SpiritSword : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<SpiritSlingshot>();
        }

        public override void SetDefaults()
        {
            Item.damage = 22;
			Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
			Item.autoReuse = true;
            Item.width = 66;
            Item.height = 66;
            Item.useTime = 32;
			Item.useAnimation = 32;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 4;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 3);
			Item.UseSound = SoundID.DD2_MonkStaffSwing;
            Item.shoot = ModContent.ProjectileType<SpiritSwordSlash>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			int Slash = Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<SpiritSwordSlash>(), damage, knockback, player.whoAmI, player.direction * player.gravDir, player.itemAnimationMax);
			Main.projectile[Slash].scale *= Item.scale;

            return false;
		}
    }
}