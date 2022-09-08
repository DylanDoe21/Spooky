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
	public class BrainJar : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Brainy in a Jar");
			Tooltip.SetDefault("Summons brainy to float above you"
			+ "\nEvery minute, he will convulsate, causing all of your other minions to explode"
			+ "\nEach explosion will deal massive damage and scale further based on their damage");
			Item.staff[Item.type] = true;
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 200;
			Item.mana = 60;
            Item.DamageType = DamageClass.Summon;
            Item.noMelee = true;
			Item.autoReuse = true;          
            Item.width = 22;
            Item.height = 34;
            Item.useTime = 45;       
            Item.useAnimation = 45;  
            Item.useStyle = ItemUseStyleID.RaiseLamp;
            Item.knockBack = 6;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.buyPrice(gold: 20);   
            Item.UseSound = SoundID.Item1; 
            Item.buffType = ModContent.BuffType<BrainyBuff>();
			Item.shoot = ModContent.ProjectileType<Brainy>();
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
