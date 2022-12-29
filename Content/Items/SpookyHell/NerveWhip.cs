using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Content.Projectiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell
{
	public class NerveWhip : ModItem
	{
		public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Exposed Nerve");
            Tooltip.SetDefault("Your summons will focus struck enemies"
            + "\nDeals more damage the more enemies it hits"
			+ "\nDamage scaling caps after ten enemies are hit");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

		public override void SetDefaults() 
        {
			Item.damage = 38;
			Item.DamageType = DamageClass.SummonMeleeSpeed;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.width = 38;
			Item.height = 48;
			Item.useTime = 40;
			Item.useAnimation = 40;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 2;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.buyPrice(gold: 15);
			Item.UseSound = SoundID.Item152;
			Item.shoot = ModContent.ProjectileType<NerveWhipProj>();
			Item.shootSpeed = 4.5f;
		}
	}
}