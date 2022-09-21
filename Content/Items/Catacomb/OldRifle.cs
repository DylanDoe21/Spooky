using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
 
namespace Spooky.Content.Items.Catacomb
{
	public class OldRifle : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Undead Hunter's Rifle");
			Tooltip.SetDefault("Shoots out rusty bullets that split on impact");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 55;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 106;           
			Item.height = 30;
			Item.knockBack = 2;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.buyPrice(gold: 5);
		}
	}
}
