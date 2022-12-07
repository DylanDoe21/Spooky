using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles;
using Spooky.Content.Projectiles.SpookyBiome;
 
namespace Spooky.Content.Items.SpookyBiome
{
	public class LeafBlowerMK2 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Leaf Blower MKII");
			Tooltip.SetDefault("Sucks in leaves spawned at your cursor"
			+ "\n'It's not even really a leaf blower anymore is it?'");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 60;           
			Item.height = 28;
			Item.rare = ItemRarityID.LightRed;  
			Item.value = Item.buyPrice(gold: 15);
		}
	}
}
