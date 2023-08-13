using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

using Spooky.Content.Buffs.Minion;
using Spooky.Content.Projectiles.Sentient;
using Spooky.Content.Tiles.SpookyHell.Furniture;

namespace Spooky.Content.Items.SpookyHell.Sentient
{
    public class SentientImpStaff : ModItem, ICauldronOutput
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 42;
            Item.mana = 50;
			Item.DamageType = DamageClass.Summon;
			Item.autoReuse = true;
            Item.width = 58;
            Item.height = 56;
            Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 2;
            Item.rare = ModContent.RarityType<SentientRarity>();
            Item.value = Item.buyPrice(gold: 25);
            Item.UseSound = SoundID.Item82;
            Item.buffType = ModContent.BuffType<GrugBuff>();
			Item.shoot = ModContent.ProjectileType<Grug>();
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