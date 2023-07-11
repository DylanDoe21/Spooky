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
            for (int index = 0; index < 2; ++index)
            {
                float SpeedX = velocity.X + Main.rand.NextFloat(-3.75f, 3.75f);
                float SpeedY = velocity.Y + Main.rand.NextFloat(-3.75f, 3.75f);
                Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, type, 0, 0f, player.whoAmI);
            }

            return false;
        }
    }
}
