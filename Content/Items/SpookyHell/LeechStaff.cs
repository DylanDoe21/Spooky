using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Buffs.Minion;
using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.Projectiles.SpookyHell;
 
namespace Spooky.Content.Items.SpookyHell
{
    public class LeechStaff : ModItem
    {
        public override void SetStaticDefaults()
		{
			Item.staff[Item.type] = true;
		}

		public override void SetDefaults()
        {
            Item.damage = 20;
			Item.mana = 10;
            Item.DamageType = DamageClass.Summon;
			Item.noMelee = true;
			Item.autoReuse = true;
            Item.width = 58;
            Item.height = 48;
            Item.useTime = 25;
            Item.useAnimation = 25;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 3;
			Item.rare = ItemRarityID.LightPurple;
			Item.value = Item.buyPrice(gold: 20); 
            Item.UseSound = SoundID.DD2_BetsysWrathShot;
            Item.buffType = ModContent.BuffType<LeechBuff>();
			Item.shoot = ModContent.ProjectileType<Leech>();
            Item.shootSpeed = 8f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
            player.AddBuff(Item.buffType, 2);

			var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
			projectile.originalDamage = Item.damage;

			return false;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<ArteryPiece>(), 10)
			.AddIngredient(ModContent.ItemType<CreepyChunk>(), 15)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
	}
}