using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;
using System;

using Spooky.Content.Projectiles.SpookyBiome;
using Spooky.Content.Projectiles.Catacomb;
 
namespace Spooky.Content.Items.Catacomb
{
	public class HarvesterScythe : SwingWeaponBase
	{
		public override int Length => 53;
		public override int TopSize => 15;
		public override float SwingDownSpeed => 15f;
		public override bool CollideWithTiles => false;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Harvester's Scythe");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 35; 
			Item.DamageType = DamageClass.Melee;
			Item.autoReuse = true;
			Item.width = 64;           
			Item.height = 60;         
			Item.useTime = 60;
			Item.useAnimation = 60;
			Item.useStyle = SwingUseStyle;
			Item.knockBack = 5;
			Item.rare = ItemRarityID.Blue;  
			Item.value = Item.buyPrice(gold: 2);
			Item.UseSound = SoundID.DD2_MonkStaffSwing;
		}
	}
}
