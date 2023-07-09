using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

using Spooky.Core;
using Spooky.Content.Projectiles.Cemetery;

namespace Spooky.Content.Items.Cemetery
{
    public class SpiritSword : SwingWeaponBase
    {
        public override int Length => 60;
		public override int TopSize => 17;
		public override float SwingDownSpeed => 13f;
		public override bool CollideWithTiles => true;
        static bool hasHitSomething = false;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<SpiritSlingshot>();
        }

        public override void SetDefaults()
        {
            Item.damage = 28;
			Item.DamageType = DamageClass.Melee;
			Item.autoReuse = true;
            Item.width = 66;
            Item.height = 66;
            Item.useTime = 60;
			Item.useAnimation = 60;
			Item.useStyle = SwingUseStyle;
			Item.knockBack = 5;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 1);
			Item.UseSound = SoundID.DD2_MonkStaffSwing;
        }

        public override void UseAnimation(Player player)
        {
            hasHitSomething = false;
        }

        public override void OnHitTiles(Player player)
        {
            if (!hasHitSomething)
            {
                hasHitSomething = true;

                SpookyPlayer.ScreenShakeAmount = 2;

                SoundEngine.PlaySound(SoundID.Item69, player.Center);

                for (int numProjectiles = 0; numProjectiles < 2; numProjectiles++)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center.X + (player.direction == 1 ? 90 + (Item.scale * 2) : -90 + (-Item.scale * 2)), 
                        player.Center.Y, Main.rand.Next(-2, 2), Main.rand.Next(-7, -5), ModContent.ProjectileType<SpookySkull>(), Item.damage, 1, Main.myPlayer, 0, 0);
                    }
                }
            }
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
		{
            if (!hasHitSomething)
            {
                hasHitSomething = true;

                SpookyPlayer.ScreenShakeAmount = 2;

                SoundEngine.PlaySound(SoundID.Item69, player.Center);

                for (int numProjectiles = 0; numProjectiles < 2; numProjectiles++)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center.X + (player.direction == 1 ? 90 + (Item.scale * 2) : -90 + (-Item.scale * 2)), 
                        player.Center.Y, Main.rand.Next(-2, 2), Main.rand.Next(-7, -5), ModContent.ProjectileType<SpookySkull>(), Item.damage, Item.knockBack, Main.myPlayer, 0, 0);
                    }
                }
            }
        }
    }
}