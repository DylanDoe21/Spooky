using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Projectiles.Sentient;

namespace Spooky.Content.Items.SpookyHell.Sentient
{
    public class SentientChumCaster : ModItem, ICauldronOutput
    {
        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 44;
            Item.useTime = 8;
            Item.useAnimation = 8;
            Item.fishingPole = 40;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ModContent.RarityType<SentientRarity>();
            Item.value = Item.buyPrice(gold: 12);
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<SentientChumCasterBobber>();
            Item.shootSpeed = 10f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) 
        {
			for (int numBobbers = 0; numBobbers < 2; numBobbers++) 
            {
				Vector2 bobberSpeed = velocity + (player.direction == -1 ? new Vector2(numBobbers * 3, 0) : new Vector2(-(numBobbers * 3), 0));
				Projectile.NewProjectile(source, position, bobberSpeed, type, 0, 0f, player.whoAmI, ai2: numBobbers);
			}

			return false;
		}

        public override void ModifyFishingLine(Projectile bobber, ref Vector2 lineOriginOffset, ref Color lineColor) 
        {
			lineOriginOffset = new Vector2(43, -30);

			if (bobber.type == Item.shoot) 
            {
                if (bobber.ai[2] == 0)
                {
				    lineColor = Color.Red;
                }
                else
                {
                    lineColor = Color.Blue;
                }
			}
		}
    }
}
