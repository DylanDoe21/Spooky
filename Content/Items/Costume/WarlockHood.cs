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
		public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("Warlock's Hood");
			// Tooltip.SetDefault("'It doesn't give powers, but it looks cool!'");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() 
		{
			Item.width = 18;
			Item.height = 18;
			Item.vanity = true;
			Item.value = Item.buyPrice(silver: 25);
			Item.rare = ItemRarityID.Blue;
		}
	}
}
