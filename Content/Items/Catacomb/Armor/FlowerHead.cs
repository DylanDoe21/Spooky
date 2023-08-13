using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Localization;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Items.Catacomb.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class FlowerHead : ModItem, IExtendedHelmet
	{
		public string ExtensionTexture => "Spooky/Content/Items/Catacomb/Armor/FlowerHead_RealHead";
        public Vector2 ExtensionSpriteOffset(PlayerDrawSet drawInfo) => new Vector2(0, -8f);

		public override void SetDefaults() 
		{
			Item.defense = 8;
			Item.width = 34;
			Item.height = 34;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.buyPrice(gold: 1);
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ModContent.ItemType<FlowerBody>() && legs.type == ModContent.ItemType<FlowerLegs>();
		}
		
		public override void UpdateArmorSet(Player player) 
		{
			player.setBonus = Language.GetTextValue("Mods.Spooky.ArmorSetBonus.FlowerArmor");
			player.lifeRegen += 5;
			player.aggro -= 100;
		}

		public override void UpdateEquip(Player player)
        {
			player.manaCost -= 0.15f;
			player.maxMinions += 2;
			player.maxTurrets += 1;
        }
    }
}