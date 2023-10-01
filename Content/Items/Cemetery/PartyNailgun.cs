using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.UI.Chat;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Projectiles;
using Spooky.Content.Projectiles.Cemetery;
 
namespace Spooky.Content.Items.Cemetery
{
	public class PartyNailgun : ModItem
	{
		private int AmmoCount = 200;
		private int AmmoRegenTimer = 0;

		public static readonly SoundStyle ShootSound = new("Spooky/Content/Sounds/PartyNailgun", SoundType.Sound);

		public override void SetDefaults()
		{
			Item.damage = 25;
			Item.crit = 1;
			Item.DamageType = DamageClass.Generic;
			Item.noMelee = true;
			Item.autoReuse = true;
			Item.width = 66;           
			Item.height = 32;
			Item.useTime = 2;         
			Item.useAnimation = 2;
			Item.useStyle = ItemUseStyleID.Shoot;         
			Item.knockBack = 0;
			Item.ArmorPenetration = 10000;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.buyPrice(gold: 20);
			Item.UseSound = ShootSound;
			Item.shoot = ModContent.ProjectileType<Blank>();
			Item.shootSpeed = 12f;
		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-6, -6);
		}

		public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
            for (var j = 0; j < 10; ++j)
            {
                string text = AmmoCount.ToString();
                Item otherItem = Main.player[Main.myPlayer].inventory[j];

                if (otherItem == Item)
                {
                    ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.CombatText[0].Value, text, position + new Vector2(-30f, 5f) * Main.inventoryScale * 0.8f,
                    Color.Lime, 0f, Vector2.Zero, new Vector2(Main.inventoryScale * 0.8f), -1f, Main.inventoryScale * 0.8f);
                }
            }
        }

        public override void UpdateInventory(Player player)
        {
			AmmoRegenTimer++;
            if (AmmoRegenTimer > 30 && AmmoCount < 200)
			{
                AmmoCount++;
				AmmoRegenTimer = 0;
            }
        }

        public override bool CanUseItem(Player player)
        {
			if (AmmoCount > 0)
			{
                AmmoCount--;
				AmmoRegenTimer = 0;
			}

			if (AmmoCount <= 0)
			{
				return false;
			}

			return true;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 55f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }

			Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(10));

			Projectile.NewProjectile(source, position.X, position.Y - 12, newVelocity.X, newVelocity.Y, ModContent.ProjectileType<PartyNailBolt>(), damage, knockback, player.whoAmI, 0f, 0f);

			return false;
		}
	}
}
