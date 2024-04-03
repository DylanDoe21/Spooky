using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.Sentient;
using Spooky.Content.Tiles.SpookyHell.Furniture;

namespace Spooky.Content.Items.SpookyHell.Sentient
{
    public class SentientChumCaster : ModItem, ICauldronOutput
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.CanFishInLava[Item.type] = true;
        }

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
			float spreadAmount = 75f; // how much the different bobbers are spread out.

			for (int index = 0; index < 2; ++index) 
            {
				Vector2 bobberSpeed = velocity + new Vector2(Main.rand.NextFloat(-spreadAmount, spreadAmount) * 0.05f, Main.rand.NextFloat(-spreadAmount, spreadAmount) * 0.05f);
				Projectile.NewProjectile(source, position, bobberSpeed, type, 0, 0f, player.whoAmI, ai2: index);
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
