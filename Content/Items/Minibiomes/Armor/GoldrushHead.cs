using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

using Spooky.Core;

namespace Spooky.Content.Items.Minibiomes.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class GoldrushHead : ModItem
	{
		public override void SetDefaults() 
		{
			Item.defense = 2;
			Item.width = 30;
			Item.height = 22;
			Item.rare = ItemRarityID.Blue;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ModContent.ItemType<GoldrushBody>() && legs.type == ModContent.ItemType<GoldrushLegs>();
		}
		
		public override void UpdateArmorSet(Player player) 
		{
			player.setBonus = Language.GetTextValue("Mods.Spooky.ArmorSetBonus.GoldrushArmor");
			player.GetModPlayer<SpookyPlayer>().GoldrushSet = true;
		}

		public override void UpdateEquip(Player player) 
		{
			player.pickSpeed -= 0.1f;
		}
	}
}