using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Tiles.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome.Armor
{
	[AutoloadEquip(EquipType.Body)]
	public class WizardGangsterBody : ModItem
	{
		public override void SetDefaults() 
		{
			Item.defense = 3;
			Item.width = 38;
			Item.height = 22;
			Item.rare = ItemRarityID.Blue;
		}

		public override void UpdateEquip(Player player) 
		{
			player.GetDamage(DamageClass.Magic).Flat += 5;
		}
	}
}