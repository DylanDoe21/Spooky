using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Items.Costume
{
	[AutoloadEquip(EquipType.Head)]
	public class ZomboidNecromancerHood : ModItem, ISpecialArmorDraw
	{
		public string HeadTexture => "Spooky/Content/Items/Costume/ZomboidNecromancerHoodTop";

		public Vector2 Offset => new Vector2(0, 4f);

		public override void SetDefaults()
		{
			Item.width = 18;
			Item.height = 32;
			Item.vanity = true;
			Item.rare = ItemRarityID.Blue;
		}
	}
}