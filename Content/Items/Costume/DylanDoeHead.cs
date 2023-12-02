using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.Items.Costume
{
	[AutoloadEquip(EquipType.Head)]
	public class DylanDoeHead : ModItem, IHelmetGlowmask
	{
		public string GlowmaskTexture => "Spooky/Content/Items/Costume/DylanDoeHead_Glow";

		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 28;
			Item.vanity = true;
			Item.rare = ItemRarityID.Quest;
		}
	}
}