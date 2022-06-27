using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Items.SpookyBiome.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class OldWoodHead : ModItem
	{
		public override void SetStaticDefaults() 
		{
			DisplayName.SetDefault("Old Wood Mask");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() 
		{
			Item.defense = 1;
			Item.width = 28;
			Item.height = 26;
			Item.value = Item.buyPrice(silver: 75);
			Item.rare = ItemRarityID.Blue;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ModContent.ItemType<OldWoodBody>() && legs.type == ModContent.ItemType<OldWoodLegs>();
		}
		
		public override void UpdateArmorSet(Player player) 
		{
			player.setBonus = "2 defense";
			player.statDefense += 2;
		}
	}
}
