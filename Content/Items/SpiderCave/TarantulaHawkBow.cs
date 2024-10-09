using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.SpiderCave;
 
namespace Spooky.Content.Items.SpiderCave
{
    public class TarantulaHawkBow : ModItem
    {
        public static readonly SoundStyle UseSound = new("Spooky/Content/Sounds/CrossbowCharge", SoundType.Sound);

        public override void SetDefaults()
        {
            Item.damage = 42;
			Item.DamageType = DamageClass.Ranged;
			Item.noMelee = true;
			Item.autoReuse = false;
			Item.noUseGraphic = true;
			Item.channel = true;
			Item.width = 34;           
			Item.height = 68;
			Item.useTime = 45;
			Item.useAnimation = 45;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 4;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.buyPrice(gold: 15);
			Item.UseSound = UseSound;
			Item.shoot = ModContent.ProjectileType<TarantulaHawkBowProj>();
			Item.useAmmo = AmmoID.Arrow;
			Item.shootSpeed = 0f;
		}

		public override bool CanUseItem(Player player)
		{
			return player.ownedProjectileCounts[ModContent.ProjectileType<TarantulaHawkBowProj>()] < 1;
		}
		
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Projectile.NewProjectile(source, position.X, position.Y, 0, 0, ModContent.ProjectileType<TarantulaHawkBowProj>(), damage, knockback, player.whoAmI, 0f, 0f);

			return false;
		}
    }
}