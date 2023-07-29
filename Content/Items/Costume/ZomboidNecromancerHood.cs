using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Items.Costume
{
	[AutoloadEquip(EquipType.Head)]
	public class ZomboidNecromancerHood : ModItem, IExtendedHelmet
	{
		public string ExtensionTexture => "Spooky/Content/Items/Costume/ZomboidNecromancerHood_Top";
        public Vector2 ExtensionSpriteOffset(PlayerDrawSet drawInfo) => new Vector2(0, -8f);

		public override void SetDefaults()
		{
			Item.width = 18;
			Item.height = 32;
			Item.vanity = true;
			Item.rare = ItemRarityID.Blue;
		}
	}
}