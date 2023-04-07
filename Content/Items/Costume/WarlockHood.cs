using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Core;
using Spooky.Content.Projectiles.SpookyBiome;

namespace Spooky.Content.Items.Costume
{
	[AutoloadEquip(EquipType.Head)]
	public class WarlockHood : ModItem
	{
		public override void SetDefaults() 
		{
			Item.width = 18;
			Item.height = 18;
			Item.vanity = true;
			Item.rare = ItemRarityID.Blue;
		}
	}
}
