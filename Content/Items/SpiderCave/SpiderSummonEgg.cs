using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.SpiderCave;
 
namespace Spooky.Content.Items.SpiderCave
{
	public class SpiderSummonEgg : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 16;
			Item.mana = 20;
			Item.DamageType = DamageClass.Summon;  
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.autoReuse = true;    
			Item.width = 26;          
			Item.height = 28;
			Item.useTime = 35;
			Item.useAnimation = 35;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 1;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(gold: 2);
			Item.UseSound = SoundID.Item97;
			//Item.shoot = ModContent.ProjectileType<SpiderSummonEggProj>();
			//Item.shootSpeed = 8f;
		}
	}
}
