using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Items.SpiderCave.Armor
{
	[AutoloadEquip(EquipType.Body)]
	public class CreepyCrawlerBody : ModItem
	{
		public override void SetDefaults() 
		{
			Item.defense = 4;
			Item.width = 46;
			Item.height = 20;
			Item.rare = ItemRarityID.Blue;
		}

		public override void UpdateEquip(Player player) 
		{
            player.GetModPlayer<SpookyPlayer>().CreepyCrawlerSpeed = true;
            player.GetCritChance(DamageClass.Generic) += 3;
        }
	}
}