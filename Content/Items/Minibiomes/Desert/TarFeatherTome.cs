using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.Minibiomes.Desert;
 
namespace Spooky.Content.Items.Minibiomes.Desert
{
    public class TarFeatherTome : ModItem
    {
        int numUses = 0;

		public override void SetDefaults()
        {
            Item.damage = 12;
			Item.mana = 10;
            Item.DamageType = DamageClass.Magic;
			Item.noMelee = true;
			Item.autoReuse = true;
            Item.width = 28;
            Item.height = 30;
            Item.useTime = 12;
            Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 2;
            Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(gold: 2);
            Item.UseSound = SoundID.Item95 with { Pitch = -1.2f };
            Item.shoot = ModContent.ProjectileType<TarTomeBlob>();
			Item.shootSpeed = 8f;
        }
        
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
            int Type = Main.rand.NextBool(4) ? ModContent.ProjectileType<TarTomeFeather>() : ModContent.ProjectileType<TarTomeBlob>();

            Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(8));

			Projectile.NewProjectile(source, position, newVelocity, Type, damage, knockback, player.whoAmI);
			
			return false;
		}
	}
}