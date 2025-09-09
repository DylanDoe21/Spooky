using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Projectiles.Minibiomes.Ocean;

namespace Spooky.Content.Items.Minibiomes.Ocean
{
    public class EelSushi : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 70;
            Item.mana = 150;
			Item.DamageType = DamageClass.Summon;
            Item.noMelee = true;
            Item.channel = true;
            Item.useTurn = true;
            Item.width = 46;
            Item.height = 48;
            Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 3;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.buyPrice(gold: 25);
            Item.UseSound = SoundID.Item46;
            Item.shoot = ModContent.ProjectileType<ZombieEelHead>();
            Item.shootSpeed = 0f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
            if (player.ownedProjectileCounts[ModContent.ProjectileType<ZombieEelHead>()] <= 0)
			{
                SpawnWorm(ModContent.ProjectileType<ZombieEelHead>(), ModContent.ProjectileType<ZombieEelBody>(), ModContent.ProjectileType<ZombieEelTail>(), 
                new Vector2(player.Center.X, player.Center.Y - 50), player, Item.damage, 0);
            }

            return false;
        }

        public static void SpawnWorm(int head, int body, int tail, Vector2 spawnPos, Player player, int damage, float knockback)
        {
            Projectile.NewProjectile(null, spawnPos, Vector2.Zero, head, damage, knockback, player.whoAmI);
            Projectile.NewProjectile(null, spawnPos, Vector2.Zero, tail, damage, knockback, player.whoAmI);

            for (var i = 0; i < 7; i++)
            {
                Projectile.NewProjectile(null, spawnPos, Vector2.Zero, body, damage, knockback, player.whoAmI, ai0: i);
            }
        }

        public override Vector2? HoldoutOffset()
		{
			return new Vector2(-3, 0);
		}

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<DunkleosteusHide>(), 10)
			.AddIngredient(ItemID.Bone, 25)
            .AddIngredient(ItemID.Ectoplasm, 15)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
    }
}