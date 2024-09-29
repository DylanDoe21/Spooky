using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Items.Costume
{
	[AutoloadEquip(EquipType.Head)]
	public class DylanDoeHead : ModItem, IExtendedHelmet
	{
		public string ExtensionTexture => "Spooky/Content/Items/Costume/DylanDoeHead_Head_Flipped";
        public Vector2 ExtensionSpriteOffset(PlayerDrawSet drawInfo) => new Vector2(0, 0);

		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 30;
			Item.vanity = true;
			Item.rare = ItemRarityID.Quest;
			Item.value = Item.buyPrice(gold: 10);
		}
	}
}