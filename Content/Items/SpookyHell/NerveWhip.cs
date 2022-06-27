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
			+ "\nDamage scaling caps after 10 enemies are hit");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

		public override void SetDefaults() 
        {
			Item.damage = 25;
			Item.DamageType = DamageClass.SummonMeleeSpeed;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.width = 36;
			Item.height = 34;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 2;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(gold: 2);
			Item.UseSound = SoundID.Item152;
			Item.shoot = ModContent.ProjectileType<NerveWhipProj>();
			Item.shootSpeed = 4;
		}
	}
}