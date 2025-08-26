using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Buffs.Minion;
using Spooky.Content.Projectiles.Minibiomes.Christmas;
 
namespace Spooky.Content.Items.Minibiomes.Christmas
{
    public class StockingStaff : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 12;
            Item.mana = 20;
            Item.DamageType = DamageClass.Summon;
            Item.autoReuse = false;
            Item.noMelee = true;
            Item.width = 44;
            Item.height = 52;
            Item.useTime = 28;
            Item.useAnimation = 32;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 1);
            Item.UseSound = SoundID.Item78;
            Item.buffType = ModContent.BuffType<StockingMinionBuff>();
			Item.shoot = ModContent.ProjectileType<StockingMinion>();
            Item.shootSpeed = 5f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
            player.AddBuff(Item.buffType, 2);

			var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI, ai2: Main.rand.Next(0, 7));
			projectile.originalDamage = Item.damage;

			return false;
		}
    }
}