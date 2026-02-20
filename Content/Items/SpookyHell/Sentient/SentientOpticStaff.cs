using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Buffs.Minion;
using Spooky.Content.Projectiles.Sentient;

namespace Spooky.Content.Items.SpookyHell.Sentient
{
    public class SentientOpticStaff : ModItem, ICauldronOutput
    {
        public override void SetDefaults()
        {
            Item.damage = 45;
            Item.mana = 25;
			Item.DamageType = DamageClass.Summon;
            Item.autoReuse = true;
			Item.noMelee = true;
			Item.width = 46;
			Item.height = 46;
            Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 3;
            Item.rare = ModContent.RarityType<SentientRarity>();
            Item.value = Item.buyPrice(gold: 30);
            Item.UseSound = SoundID.Item90;
            Item.buffType = ModContent.BuffType<SentientOpticStaffBuff>();
			Item.shoot = ModContent.ProjectileType<SentientOpticStaffMouth>();
            Item.shootSpeed = 0f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
            player.AddBuff(Item.buffType, 2);

            var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            projectile.ai[1] = player.ownedProjectileCounts[type]; //projectile count for the minion circle position
            projectile.originalDamage = Item.damage;

			return false;
		}
    }
}