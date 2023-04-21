using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Projectiles.Sentient;
using Spooky.Content.Tiles.SpookyHell.Furniture;

namespace Spooky.Content.Items.SpookyHell.Sentient
{
    public class SentientKatana : ModItem, ICauldronOutput
    {
        public override void SetDefaults()
        {
            Item.damage = 25;
			Item.DamageType = DamageClass.Melee;
			Item.autoReuse = true;
            Item.width = 50;
            Item.height = 64;
            Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 2;
            Item.rare = ModContent.RarityType<SentientRarity>();
            Item.value = Item.buyPrice(gold: 15);
            Item.UseSound = SoundID.DD2_MonkStaffSwing;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.NewProjectile(player.GetSource_OnHit(target), Main.MouseWorld.X, Main.MouseWorld.Y,
            0, 0, ModContent.ProjectileType<SentientKatanaSlashSpawner>(), Item.damage, Item.knockBack, player.whoAmI, 0f, 0f);
        }
    }
}