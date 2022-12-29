using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Items.Cemetery
{
	public class SwampGooStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Swamp Goo Staff");
			Tooltip.SetDefault("Summons a goo monster sentry");
			Item.staff[Item.type] = true;
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 15;
			Item.mana = 12;          
			Item.DamageType = DamageClass.Summon;
			Item.noMelee = true;
			Item.autoReuse = true;       
			Item.width = 14;           
			Item.height = 32;         
			Item.useTime = 30;         
			Item.useAnimation = 30;         
			Item.useStyle = ItemUseStyleID.Shoot;          
			Item.knockBack = 1;
			Item.rare = ItemRarityID.Blue;  
			Item.value = Item.buyPrice(gold: 2);
			Item.UseSound = SoundID.Item70;
			//Item.shoot = ModContent.ProjectileType<SwampGooSentry>();
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) 
		{
			position = Main.MouseWorld;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
			projectile.originalDamage = Item.damage;

			return false;
		}
	}
}
