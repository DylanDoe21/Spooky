using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.SpiderCave;

namespace Spooky.Content.Items.SpiderCave
{
	[AutoloadEquip(EquipType.Shield)]
	public class OrbWeaverShield : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 38;
			Item.height = 44;
			Item.defense = 3;
			Item.accessory = true;
            Item.rare = ItemRarityID.Blue;  
            Item.value = Item.buyPrice(gold: 3);
		}
    }
}
