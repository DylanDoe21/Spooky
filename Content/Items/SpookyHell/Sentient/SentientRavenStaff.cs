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
    public class SentientRavenStaff : ModItem, ICauldronOutput
    {
        public override void SetDefaults()
        {
            Item.damage = 150;
            Item.mana = 50;
			Item.DamageType = DamageClass.Summon;
            Item.autoReuse = true;
			Item.noMelee = true;
			Item.width = 44;
			Item.height = 72;
            Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 3;
            Item.rare = ModContent.RarityType<SentientRarity>();
            Item.value = Item.buyPrice(gold: 60);
            Item.UseSound = SoundID.Item90;
            Item.buffType = ModContent.BuffType<BillyBuff>();
			Item.shoot = ModContent.ProjectileType<Billy>();
            Item.shootSpeed = 12f;
        }

        public override bool CanUseItem(Player player)
		{
			if (player.ownedProjectileCounts[Item.shoot] > 0 && player.altFunctionUse != 2) 
			{
				return false;
			}

			return true;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
            if (player.ownedProjectileCounts[Item.shoot] < 1)
            {
                player.AddBuff(Item.buffType, 2);

                var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
                projectile.originalDamage = Item.damage;
            }

			return false;
		}
    }
}