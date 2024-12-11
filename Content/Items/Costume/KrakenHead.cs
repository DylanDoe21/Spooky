using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Items.Costume
{
	[AutoloadEquip(EquipType.Head)]
	public class KrakenHead : ModItem, ISpecialHelmetDraw
	{
		public string GlowTexture => "Spooky/Content/Items/Costume/KrakenHead_Glow";

		public override void SetDefaults()
		{
			Item.width = 22;
			Item.height = 26;
			Item.vanity = true;
			Item.rare = ItemRarityID.Quest;
			Item.value = Item.buyPrice(gold: 10);
		}
	}
}