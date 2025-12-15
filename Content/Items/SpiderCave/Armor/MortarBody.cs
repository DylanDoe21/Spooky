using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Tiles.SpiderCave;

namespace Spooky.Content.Items.SpiderCave.Armor
{
	[AutoloadEquip(EquipType.Body)]
	public class MortarBody : ModItem
	{
		public override void SetStaticDefaults()
        {
			ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<MortarLegs>();
		}

		public override void SetDefaults() 
		{
			Item.defense = 25;
			Item.width = 34;
			Item.height = 22;
			Item.rare = ItemRarityID.Yellow;
		}

		public override void UpdateEquip(Player player) 
		{
            player.GetDamage(DamageClass.Generic) += 0.15f;
			player.huntressAmmoCost90 = true;
			player.endurance += 0.05f;
        }
	}
}