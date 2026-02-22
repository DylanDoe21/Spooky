using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.Catacomb;
 
namespace Spooky.Content.Items.Catacomb
{
    public class BoneBow : ModItem
    {
        public static readonly SoundStyle UseSound = new("Spooky/Content/Sounds/CrossbowCharge", SoundType.Sound);

        public override void SetDefaults()
        {
            Item.damage = 12;
			Item.DamageType = DamageClass.Ranged;
			Item.noMelee = true;
			Item.autoReuse = true;
			Item.noUseGraphic = true;
			Item.channel = true;
			Item.width = 20;           
			Item.height = 60;
			Item.useTime = 45;
			Item.useAnimation = 45;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 4;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(gold: 10);
			Item.UseSound = UseSound;
			Item.shoot = ModContent.ProjectileType<BoneBowProj>();
			Item.useAmmo = AmmoID.Arrow;
			Item.shootSpeed = 18f;
		}

		public override bool CanUseItem(Player player)
		{
			return player.ownedProjectileCounts[ModContent.ProjectileType<BoneBowProj>()] < 1;
		}
		
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Projectile.NewProjectile(source, position.X, position.Y, 0, 0, ModContent.ProjectileType<BoneBowProj>(), damage, knockback, player.whoAmI, 0f, 0f);

			return false;
		}
    }
}