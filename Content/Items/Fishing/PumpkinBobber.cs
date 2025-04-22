using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.Fishing
{
	public class PumpkinBobber : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 18;
			Item.height = 32;
			Item.accessory = true;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(gold: 1);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.fishingSkill += 10;
			player.overrideFishingBobber = ModContent.ProjectileType<PumpkinBobberProj>();
		}
    }

	public class PumpkinBobberProj : ModProjectile
    {
		public override string Texture => "Spooky/Content/Items/Fishing/PumpkinBobber";

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.BobberWooden);
            DrawOriginOffsetY = -8;
        }
    }
}
