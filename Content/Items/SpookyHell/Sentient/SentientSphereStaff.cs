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
    public class SentientSphereStaff : ModItem, ICauldronOutput
    {
        public override void SetDefaults()
        {
            Item.damage = 68;
            Item.mana = 20;
			Item.DamageType = DamageClass.Summon;
            Item.noMelee = true;
			Item.autoReuse = true;
			Item.width = 68;
			Item.height = 70;
            Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 3;
            Item.rare = ModContent.RarityType<SentientRarity>();
            Item.value = Item.buyPrice(gold: 60);
            Item.UseSound = SoundID.Item82;
            Item.buffType = ModContent.BuffType<WingedBiomassBuff>();
			Item.shoot = ModContent.ProjectileType<WingedBiomass>();
            Item.shootSpeed = 12f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
            player.AddBuff(Item.buffType, 2);

            var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
            projectile.originalDamage = Item.damage;

			return false;
		}
    }
}