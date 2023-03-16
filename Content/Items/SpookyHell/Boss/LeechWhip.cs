using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Content.Projectiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell.Boss
{
	public class LeechWhip : ModItem
	{
		public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Leecher");
            Tooltip.SetDefault("Your summons will focus struck enemies"
            + "\nHitting enemies will sometimes drop gore chunks"
			+ "\nPicking up gore chunks will restore a small amount of health");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

		public override void SetDefaults() 
        {
			Item.damage = 60;
			Item.DamageType = DamageClass.SummonMeleeSpeed;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.width = 38;
			Item.height = 44;
			Item.useTime = 35;
			Item.useAnimation = 35;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 2;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.buyPrice(gold: 2);
			Item.UseSound = SoundID.Item152;
			Item.shoot = ModContent.ProjectileType<LeechWhipProj>();
			Item.shootSpeed = 4f;
		}
	}
}