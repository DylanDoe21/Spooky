using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Buffs.Minion;
using Spooky.Content.Projectiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell
{
	public class BoogerStaff : ModItem
	{
		public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<BoogerFlail>();
			Item.staff[Item.type] = true;
        }

		public override void SetDefaults()
		{
			Item.damage = 25;
			Item.mana = 20;          
			Item.DamageType = DamageClass.Summon;
			Item.noMelee = true;
			Item.autoReuse = true;       
			Item.width = 42;           
			Item.height = 40;         
			Item.useTime = 35;         
			Item.useAnimation = 35;         
			Item.useStyle = ItemUseStyleID.Swing;          
			Item.knockBack = 1;
			Item.rare = ItemRarityID.Orange;  
			Item.value = Item.buyPrice(gold: 15);
			Item.UseSound = SoundID.Item78;     
			Item.buffType = ModContent.BuffType<NoseMinionBuff>();
			Item.shoot = ModContent.ProjectileType<NoseMinion>();
			Item.shootSpeed = 3f;
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
