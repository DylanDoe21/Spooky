using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.Catacomb;

namespace Spooky.Content.Items.Catacomb
{
	public class DaffodilBow : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 35;     
			Item.DamageType = DamageClass.Ranged;
			Item.noMelee = true;
			Item.autoReuse = false;
			Item.noUseGraphic = true;
			Item.channel = true;
			Item.width = 26;    
			Item.height = 38;         
			Item.useTime = 32;         
			Item.useAnimation = 32;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 2;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.buyPrice(gold: 20);
			Item.UseSound = SoundID.Item5;
		}
	}
}
