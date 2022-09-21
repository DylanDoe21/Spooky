using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
 
namespace Spooky.Content.Items.Catacomb
{
	public class ThornStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Rod of Ensnaring");
			Tooltip.SetDefault("Casts a lump of thorns that ensares hit enemies for a short time"
			+ "\nEnsnared enemies will not be able to move, and will be poisoned"
			+ "\nBosses cannot be ensnared, but will take more damage from the thorn clump");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 25;
			Item.mana = 10;
			Item.DamageType = DamageClass.Magic;
			Item.width = 106;           
			Item.height = 30;
			Item.knockBack = 0;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(gold: 1);
		}
	}
}
