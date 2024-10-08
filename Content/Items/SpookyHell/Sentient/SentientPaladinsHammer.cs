using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Projectiles.Sentient;

namespace Spooky.Content.Items.SpookyHell.Sentient
{
    public class SentientPaladinsHammer : ModItem, ICauldronOutput
    {
        public override void SetDefaults()
        {
            Item.damage = 380;
            Item.crit = 15;
			Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.noUseGraphic = true;
			Item.autoReuse = true;
            Item.width = 54;
            Item.height = 54;
            Item.useTime = 35;
			Item.useAnimation = 35;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 8;
            Item.rare = ModContent.RarityType<SentientRarity>();
            Item.value = Item.buyPrice(gold: 60);
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<SentientPaladinsHammerProj>();
            Item.shootSpeed = 40f;
        }

        public override bool CanUseItem(Player player)
		{
			if (player.ownedProjectileCounts[Item.shoot] > 0) 
			{
				return false;
			}

			return true;
		}
    }
}