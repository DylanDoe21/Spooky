using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Items.Minibiomes.Desert
{
	public class HallucigeniaSpine : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 26;
			Item.height = 42;
			Item.value = Item.buyPrice(gold: 15);
			Item.rare = ItemRarityID.LightRed;
			Item.accessory = true;
		}

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
        }
	}
}