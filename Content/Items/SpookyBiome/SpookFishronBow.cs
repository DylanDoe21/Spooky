using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome
{
    public class SpookFishronBow : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 90;
			Item.DamageType = DamageClass.Ranged;
			Item.noMelee = true;
			Item.autoReuse = true;
			Item.noUseGraphic = true;
			Item.channel = true;
            Item.width = 28;
            Item.height = 58;
            Item.useTime = 25;
			Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 4;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.buyPrice(gold: 25);
            Item.UseSound = SoundID.Item5;
			Item.shoot = ModContent.ProjectileType<SpookFishronBowProj>();
			Item.useAmmo = AmmoID.Arrow;
			Item.shootSpeed = 0f;
        }

        public override bool CanUseItem(Player player)
		{
			return player.ownedProjectileCounts[ModContent.ProjectileType<SpookFishronBowProj>()] < 1;
		}
		
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Projectile.NewProjectile(source, position.X, position.Y, 0, 0, ModContent.ProjectileType<SpookFishronBowProj>(), damage, knockback, player.whoAmI);

			return false;
		}
    }
}