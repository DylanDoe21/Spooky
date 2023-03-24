using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Terraria.Audio;

using Spooky.Core;
using Spooky.Content.Projectiles.Cemetery;

namespace Spooky.Content.Items.Cemetery
{
    public class SpiritSword : SwingWeaponBase
    {
        public override int Length => 50;
		public override int TopSize => 15;
		public override float SwingDownSpeed => 13f;
		public override bool CollideWithTiles => true;
        static bool hasHitSomething = false;
        static bool hasHitEnemies = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spooky Buster");
            Tooltip.SetDefault("Launches explosive skulls upon striking the ground");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 25;
			Item.DamageType = DamageClass.Melee;
			Item.autoReuse = true;
            Item.width = 46;
            Item.height = 46;
            Item.useTime = 75;
			Item.useAnimation = 75;
			Item.useStyle = SwingUseStyle;
			Item.knockBack = 8;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 1);
			Item.UseSound = SoundID.DD2_MonkStaffSwing;
            Item.scale = 1.2f;
        }

        public override void UseAnimation(Player player)
        {
            hasHitSomething = false;
            hasHitEnemies = false;
        }

        public override void OnHitTiles(Player player)
        {
            if (!hasHitSomething)
            {
                hasHitSomething = true;

                SpookyPlayer.ScreenShakeAmount = 5;

                SoundEngine.PlaySound(SoundID.Item69, player.Center);

                for (int numProjectiles = 0; numProjectiles < 2; numProjectiles++)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center.X + (player.direction == 1 ? 80 : -80), 
                        player.Center.Y, Main.rand.Next(-2, 2), Main.rand.Next(-12, -10), ModContent.ProjectileType<SpookySkull>(), Item.damage, 1, Main.myPlayer, 0, 0);
                    }
                }
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            if (!hasHitEnemies)
            {
                hasHitEnemies = true;

                //Projectile.NewProjectile(Item.GetSource_FromThis(), target.Center.X, target.Center.Y, 0, 0,
                //ModContent.ProjectileType<FleshAxeHit>(), Item.damage, 0f, Main.myPlayer, 0, 0);
            }
        }
    }
}