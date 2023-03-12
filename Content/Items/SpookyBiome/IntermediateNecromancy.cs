/*
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
	public class IntermediateNecromancy : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Intermediate Necromancy");
			Tooltip.SetDefault("Casts random magical skeleton body parts"
			+ "\nSkeleton hands will accelerate quickly and can pierce through enemies"
			+ "\nHip bones will shatter on enemy hits, creating small damaging bone shards"
			+ "\nSkulls will travel slowly, and can quickly home in on nearby enemies");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 36;           
			Item.height = 34;
			Item.rare = ItemRarityID.LightRed;  
			Item.value = Item.buyPrice(gold: 15);
		}
	}
}
*/