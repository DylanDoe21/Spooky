using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Items.Costume
{
	[AutoloadEquip(EquipType.Head)]
	public class RotGourdMask : ModItem, ISpecialHelmetDraw
	{
		public string HeadTexture => "Spooky/Content/Items/Costume/RotGourdMask_Top";

        public Vector2 Offset => new Vector2(0, 4f);

		public override void SetDefaults()
		{
			Item.width = 26;
			Item.height = 34;
			Item.vanity = true;
			Item.rare = ItemRarityID.Blue;
		}
	}
}