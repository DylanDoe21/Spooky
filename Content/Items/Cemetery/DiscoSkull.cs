using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.UI.Chat;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Projectiles.Cemetery;
 
namespace Spooky.Content.Items.Cemetery
{
	public class DiscoSkull : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 85;
            Item.mana = 20;
			Item.DamageType = DamageClass.Magic;
            Item.noMelee = true;
            Item.channel = true;
            Item.useTurn = true;
			Item.autoReuse = true;
            Item.width = 28;
            Item.height = 30;
            Item.useTime = 80;
			Item.useAnimation = 80;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 3;
			Item.rare = ItemRarityID.Yellow;
			Item.value = Item.buyPrice(platinum: 1);
			Item.UseSound = SoundID.Item164;
			Item.shoot = ModContent.ProjectileType<DiscoPartySkull>();
			Item.shootSpeed = 0f;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
            for (int numProjectiles = 0; numProjectiles < 8; numProjectiles++)
            {
			    Projectile.NewProjectile(source, position, new Vector2(Main.rand.Next(-15, 16), Main.rand.Next(-15, 16)), Item.shoot, damage, knockback, player.whoAmI, 0, Main.rand.Next(0, 8));
            }
			
			return false;
		}
	}
}
