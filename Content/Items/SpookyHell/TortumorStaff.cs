using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;

using Spooky.Content.Buffs.Minion;
using Spooky.Content.Projectiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell
{
	public class TortumorStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Gore Staff");
			Tooltip.SetDefault("Summons a tortumor to fight with you");
			Item.staff[Item.type] = true;
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
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
			Item.useStyle = ItemUseStyleID.Shoot;          
			Item.knockBack = 1;
			Item.rare = ItemRarityID.Green;  
			Item.value = Item.buyPrice(gold: 2);
			Item.UseSound = SoundID.Item70;     
			Item.buffType = ModContent.BuffType<TortumorMinionBuff>();
			Item.shoot = ModContent.ProjectileType<TortumorMinion>();
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) 
		{
			position = Main.MouseWorld;
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